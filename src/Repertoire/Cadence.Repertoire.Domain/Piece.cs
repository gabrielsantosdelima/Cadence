using Cadence.Repertoire.Domain.Common;
using Cadence.Repertoire.Domain.Enums;
using Cadence.Repertoire.Domain.Events;
using Cadence.Repertoire.Domain.Ids;
using Cadence.Repertoire.Domain.ValueObjects;

namespace Cadence.Repertoire.Domain
{
    public sealed class Piece : AggregateRoot
    {
        public PieceId Id { get; private set; }
        public PieceTitle Title { get; private set; }
        public string? Composer { get; private set; }
        public GenreEnum Genre { get; private set; }
        public DifficultyEnum Difficulty { get; private set; }
        public MusicalKey? Key { get; private set; }
        public Uri? ReferenceUrl { get; private set; }
        public LearningStatusEnum Status { get; private set; }
        public PracticeRecord Record { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }

        private Piece()
        {
            Title = null!;
            Record = null!;
        }

        public static Result<Piece> Create(
            PieceTitle title,
            string? composer,
            GenreEnum genre,
            DifficultyEnum difficulty,
            MusicalKey? key,
            Uri? referenceUrl)
        {
            Result validationResult = ValidateCataloguingData(composer, referenceUrl);
            if (validationResult.IsFailure)
                return Result.Failure<Piece>(validationResult.Error);

            DateTime nowUtc = DateTime.UtcNow;

            var piece = new Piece
            {
                Id = PieceId.New(),
                Title = title,
                Composer = composer,
                Genre = genre,
                Difficulty = difficulty,
                Key = key,
                ReferenceUrl = referenceUrl,
                Status = LearningStatusEnum.Backlog,
                Record = PracticeRecord.Empty,
                CreatedAtUtc = nowUtc,
                UpdatedAtUtc = nowUtc
            };

            return Result.Success(piece);
        }

        public Result UpdateDetails(
            PieceTitle title,
            string? composer,
            GenreEnum genre,
            DifficultyEnum difficulty,
            MusicalKey? key,
            Uri? referenceUrl)
        {
            Result validationResult = ValidateCataloguingData(composer, referenceUrl);
            if (validationResult.IsFailure)
                return validationResult;

            Title = title;
            Composer = composer;
            Genre = genre;
            Difficulty = difficulty;
            Key = key;
            ReferenceUrl = referenceUrl;
            UpdatedAtUtc = DateTime.UtcNow;

            return Result.Success();
        }

        public Result ChangeStatus(LearningStatusEnum target)
        {
            if (!IsManualTransitionAllowed(Status, target))
                return Result.Failure(new Error(
                    "Piece.InvalidStatusTransition",
                    $"Cannot manually change status from {Status} to {target}"));

            if (target == LearningStatusEnum.Mastered && Record.SessionCount == 0)
                return Result.Failure(new Error(
                    "Piece.CannotMasterWithoutPractice",
                    "A piece cannot be mastered without a recorded practice session"));

            Status = target;
            UpdatedAtUtc = DateTime.UtcNow;

            return Result.Success();
        }

        public Result RegisterPractice(int minutes, int rating, DateTime occurredAtUtc)
        {
            LearningStatusEnum previousStatus = Status;

            Record = Record.Register(minutes, rating, occurredAtUtc);

            if (previousStatus is LearningStatusEnum.Backlog or LearningStatusEnum.Shelved)
                Status = LearningStatusEnum.Learning;

            UpdatedAtUtc = DateTime.UtcNow;

            if (Status != previousStatus)
                Raise(new PiecePromoted(Id, previousStatus, Status, occurredAtUtc));

            if (previousStatus != LearningStatusEnum.Mastered && Status == LearningStatusEnum.Mastered)
                Raise(new PieceMastered(Id, Record.TotalMinutes, occurredAtUtc));

            return Result.Success();
        }

        private static bool IsManualTransitionAllowed(LearningStatusEnum current, LearningStatusEnum target)
        {
            if (target == LearningStatusEnum.Shelved)
                return true;

            return (current, target) switch
            {
                (LearningStatusEnum.Learning, LearningStatusEnum.Polishing) => true,
                (LearningStatusEnum.Polishing, LearningStatusEnum.Mastered) => true,
                (LearningStatusEnum.Shelved, LearningStatusEnum.Learning) => true,
                _ => false
            };
        }

        private static Result ValidateCataloguingData(string? composer, Uri? referenceUrl)
        {
            if (composer is not null && composer.Length > 120)
                return Result.Failure(new Error(
                    "Piece.ComposerTooLong",
                    "Composer must be at most 120 characters"));

            if (referenceUrl is not null && !referenceUrl.IsAbsoluteUri)
                return Result.Failure(new Error(
                    "Piece.ReferenceUrlNotAbsolute",
                    "ReferenceUrl must be an absolute URL"));

            return Result.Success();
        }
    }
}

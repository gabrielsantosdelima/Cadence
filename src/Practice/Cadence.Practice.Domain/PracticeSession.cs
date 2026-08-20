using Cadence.Practice.Domain.Common;
using Cadence.Practice.Domain.Enums;
using Cadence.Practice.Domain.Events;
using Cadence.Practice.Domain.Ids;
using Cadence.Practice.Domain.ValueObjects;

namespace Cadence.Practice.Domain
{
    public sealed class PracticeSession : AggregateRoot
    {
        public SessionId Id { get; private set; }
        public PieceReference Piece { get; private set; }
        public DateTime StartedAtUtc { get; private set; }
        public PracticeDuration Duration { get; private set; }
        public Tempo? Tempo { get; private set; }
        public PracticeFocus Focus { get; private set; }
        public QualityRating Quality { get; private set; }
        public string? Notes { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        private PracticeSession()
        {
            Piece = null!;
            Duration = null!;
            Quality = null!;
        }

        public static Result<PracticeSession> Register(
            PieceReference piece,
            DateTime startedAtUtc,
            PracticeDuration duration,
            Tempo? tempo,
            PracticeFocus focus,
            QualityRating quality,
            string? notes,
            DateTime nowUtc)
        {
            if (startedAtUtc > nowUtc)
                return Result.Failure<PracticeSession>(new Error(
                    "PracticeSession.StartedInFuture",
                    "StartedAtUtc cannot be in the future"));

            if (startedAtUtc + duration.Value > nowUtc)
                return Result.Failure<PracticeSession>(new Error(
                    "PracticeSession.NotYetFinished",
                    "A session that has not finished cannot be logged"));

            var session = new PracticeSession
            {
                Id = SessionId.New(),
                Piece = piece,
                StartedAtUtc = startedAtUtc,
                Duration = duration,
                Tempo = tempo,
                Focus = focus,
                Quality = quality,
                Notes = notes,
                CreatedAtUtc = nowUtc
            };

            session.Raise(new PracticeWasRegistered(
                session.Id,
                piece.PieceId,
                startedAtUtc,
                duration.Minutes,
                quality.Value,
                focus));

            return Result.Success(session);
        }
    }
}

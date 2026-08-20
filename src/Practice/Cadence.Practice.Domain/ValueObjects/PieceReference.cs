using Cadence.Practice.Domain.Ids;

namespace Cadence.Practice.Domain.ValueObjects
{
    public sealed class PieceReference
    {
        public PieceId PieceId { get; }
        public string Title { get; }

        private PieceReference(PieceId pieceId, string title)
        {
            PieceId = pieceId;
            Title = title;
        }

        public static Result<PieceReference> Create(PieceId pieceId, string title)
        {
            string trimmedTitle = title.Trim();

            if (string.IsNullOrEmpty(trimmedTitle))
                return Result.Failure<PieceReference>(
                    new Error(
                        "PieceReference.TitleRequired",
                        "PieceReference title must not be blank"
                    )
                );

            return Result.Success(new PieceReference(pieceId, trimmedTitle));
        }
    }
}

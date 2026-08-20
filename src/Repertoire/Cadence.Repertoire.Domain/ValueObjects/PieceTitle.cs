namespace Cadence.Repertoire.Domain.ValueObjects
{
    public sealed class PieceTitle
    {
        public string Value { get; }

        private PieceTitle(string value)
        {
            Value = value;
        }

        public static Result<PieceTitle> Create(string value)
        {
            string trimValue = value.Trim();

            if (string.IsNullOrEmpty(trimValue) || trimValue.Length > 120)
                return Result.Failure<PieceTitle>(
                    new Error
                    (
                        "PieceTitle.Invalid",
                        "PieceTitle must be between 1 and 120 characters"
                    )
                );

            return Result.Success(new PieceTitle(trimValue));
        }
    }
}

namespace Cadence.Practice.Domain.ValueObjects
{
    public sealed class QualityRating
    {
        public int Value { get; }

        public string Description => Value switch
        {
            1 => "Poor",
            2 => "Below Average",
            3 => "Average",
            4 => "Good",
            5 => "Excellent",
            _ => throw new InvalidOperationException("Unreachable: QualityRating is validated at construction")
        };

        private QualityRating(int value)
        {
            Value = value;
        }

        public static Result<QualityRating> Create(int value)
        {
            if (value is < 1 or > 5)
                return Result.Failure<QualityRating>(
                    new Error(
                        "QualityRating.Invalid",
                        "QualityRating must be between 1 and 5"
                    )
                );

            return Result.Success(new QualityRating(value));
        }
    }
}

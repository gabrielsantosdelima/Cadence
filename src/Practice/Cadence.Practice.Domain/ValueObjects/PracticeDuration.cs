namespace Cadence.Practice.Domain.ValueObjects
{
    public sealed class PracticeDuration
    {
        public int Minutes { get; }
        public TimeSpan Value => TimeSpan.FromMinutes(Minutes);

        private PracticeDuration(int minutes)
        {
            Minutes = minutes;
        }

        public static Result<PracticeDuration> Create(int minutes)
        {
            if (minutes is < 1 or > 600)
                return Result.Failure<PracticeDuration>(
                    new Error(
                        "PracticeDuration.Invalid",
                        "PracticeDuration must be between 1 and 600 minutes"
                    )
                );

            return Result.Success(new PracticeDuration(minutes));
        }
    }
}

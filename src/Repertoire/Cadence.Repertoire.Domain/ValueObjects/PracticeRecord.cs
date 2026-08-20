namespace Cadence.Repertoire.Domain.ValueObjects
{
    public sealed class PracticeRecord
    {
        public static readonly PracticeRecord Empty = new(0, 0, null, null);

        public int TotalMinutes { get; }
        public int SessionCount { get; }
        public DateTime? LastPracticedAtUtc { get; }
        public decimal? AverageQuality { get; }

        private PracticeRecord(int totalMinutes, int sessionCount, DateTime? lastPracticedAtUtc, decimal? averageQuality)
        {
            TotalMinutes = totalMinutes;
            SessionCount = sessionCount;
            LastPracticedAtUtc = lastPracticedAtUtc;
            AverageQuality = averageQuality;
        }

        public PracticeRecord Register(int minutes, int rating, DateTime occurredAtUtc)
        {
            int newSessionCount = SessionCount + 1;
            decimal currentAverage = AverageQuality ?? 0m;
            decimal newAverage = Math.Round(currentAverage + (rating - currentAverage) / newSessionCount, 1);
            DateTime? newLastPracticedAtUtc = LastPracticedAtUtc is null || occurredAtUtc > LastPracticedAtUtc
                ? occurredAtUtc
                : LastPracticedAtUtc;

            return new PracticeRecord(TotalMinutes + minutes, newSessionCount, newLastPracticedAtUtc, newAverage);
        }
    }
}

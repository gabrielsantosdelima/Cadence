namespace Cadence.Practice.Domain.ValueObjects
{
    public sealed class Tempo
    {
        public int Bpm { get; }

        private Tempo(int bpm)
        {
            Bpm = bpm;
        }

        public static Result<Tempo> Create(int bpm)
        {
            if (bpm is < 20 or > 300)
                return Result.Failure<Tempo>(
                    new Error(
                        "Tempo.Invalid",
                        "Tempo must be between 20 and 300 BPM"
                    )
                );

            return Result.Success(new Tempo(bpm));
        }
    }
}

using Cadence.Practice.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cadence.Practice.Infrastructure.Converters
{
    public sealed class TempoConverter : ValueConverter<Tempo?, int?>
    {
        public TempoConverter()
            : base(
                tempo => tempo == null ? null : tempo.Bpm,
                value => value == null ? null : Tempo.Create(value.Value).Value)
        {
        }
    }
}

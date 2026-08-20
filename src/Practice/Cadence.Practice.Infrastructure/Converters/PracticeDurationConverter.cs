using Cadence.Practice.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cadence.Practice.Infrastructure.Converters
{
    public sealed class PracticeDurationConverter : ValueConverter<PracticeDuration, int>
    {
        public PracticeDurationConverter()
            : base(duration => duration.Minutes, value => PracticeDuration.Create(value).Value)
        {
        }
    }
}

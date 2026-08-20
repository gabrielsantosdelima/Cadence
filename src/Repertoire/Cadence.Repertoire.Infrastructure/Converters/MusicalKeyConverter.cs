using Cadence.Repertoire.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cadence.Repertoire.Infrastructure.Converters
{
    public sealed class MusicalKeyConverter : ValueConverter<MusicalKey?, string?>
    {
        public MusicalKeyConverter()
            : base(
                musicalKey => musicalKey == null ? null : musicalKey.Value,
                value => value == null ? null : MusicalKey.Create(value).Value)
        {
        }
    }
}

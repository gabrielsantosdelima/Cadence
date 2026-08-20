using Cadence.Practice.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cadence.Practice.Infrastructure.Converters
{
    public sealed class QualityRatingConverter : ValueConverter<QualityRating, int>
    {
        public QualityRatingConverter()
            : base(quality => quality.Value, value => QualityRating.Create(value).Value)
        {
        }
    }
}

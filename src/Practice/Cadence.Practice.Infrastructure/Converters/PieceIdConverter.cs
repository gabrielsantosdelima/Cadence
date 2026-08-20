using Cadence.Practice.Domain.Ids;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cadence.Practice.Infrastructure.Converters
{
    public sealed class PieceIdConverter : ValueConverter<PieceId, Guid>
    {
        public PieceIdConverter()
            : base(pieceId => pieceId.Value, value => PieceId.From(value))
        {
        }
    }

    public sealed class PieceIdComparer : ValueComparer<PieceId>
    {
        public PieceIdComparer()
            : base(
                (left, right) => left.Value == right.Value,
                pieceId => pieceId.Value.GetHashCode(),
                pieceId => PieceId.From(pieceId.Value))
        {
        }
    }
}

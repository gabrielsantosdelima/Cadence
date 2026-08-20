using Cadence.Repertoire.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cadence.Repertoire.Infrastructure.Converters
{
    public sealed class PieceTitleConverter : ValueConverter<PieceTitle, string>
    {
        public PieceTitleConverter()
            : base(pieceTitle => pieceTitle.Value, value => PieceTitle.Create(value).Value)
        {
        }
    }
}

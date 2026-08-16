using Cadence.Repertoire.Domain.Enums;
using Cadence.Repertoire.Domain.Ids;

namespace Cadence.Repertoire.Domain
{
    public interface IPieceRepository
    {
        Task<Piece?> GetAsync(PieceId id, CancellationToken cancellationToken);
        Task<IReadOnlyList<Piece>> ListAsync(LearningStatusEnum? status, GenreEnum? genre, CancellationToken cancellationToken);
        void Add(Piece piece);
        void Remove(Piece piece);
    }
}

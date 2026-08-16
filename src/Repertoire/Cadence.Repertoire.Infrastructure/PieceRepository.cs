using Cadence.Repertoire.Domain;
using Cadence.Repertoire.Domain.Enums;
using Cadence.Repertoire.Domain.Ids;
using Microsoft.EntityFrameworkCore;

namespace Cadence.Repertoire.Infrastructure
{
    public sealed class PieceRepository : IPieceRepository
    {
        private readonly RepertoireDbContext _dbContext;

        public PieceRepository(RepertoireDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Piece?> GetAsync(PieceId id, CancellationToken cancellationToken)
        {
            return _dbContext.Pieces.SingleOrDefaultAsync(piece => piece.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Piece>> ListAsync(LearningStatusEnum? status, GenreEnum? genre, CancellationToken cancellationToken)
        {
            IQueryable<Piece> query = _dbContext.Pieces.AsNoTracking();

            if (status is not null)
                query = query.Where(piece => piece.Status == status);

            if (genre is not null)
                query = query.Where(piece => piece.Genre == genre);

            return await query.ToListAsync(cancellationToken);
        }

        public void Add(Piece piece)
        {
            _dbContext.Pieces.Add(piece);
        }

        public void Remove(Piece piece)
        {
            _dbContext.Pieces.Remove(piece);
        }
    }
}

using Cadence.Practice.Domain;
using Cadence.Practice.Domain.Ids;
using Microsoft.EntityFrameworkCore;

namespace Cadence.Practice.Infrastructure
{
    public sealed class SessionRepository : ISessionRepository
    {
        private readonly PracticeDbContext _dbContext;

        public SessionRepository(PracticeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<PracticeSession?> GetAsync(SessionId id, CancellationToken cancellationToken)
        {
            return _dbContext.Sessions.SingleOrDefaultAsync(session => session.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<PracticeSession>> ListAsync(PieceId? pieceId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
        {
            IQueryable<PracticeSession> query = _dbContext.Sessions.AsNoTracking();

            if (pieceId is not null)
                query = query.Where(session => session.Piece.PieceId == pieceId);

            if (from is not null)
                query = query.Where(session => session.StartedAtUtc >= from);

            if (to is not null)
                query = query.Where(session => session.StartedAtUtc <= to);

            return await query
                .OrderByDescending(session => session.StartedAtUtc)
                .ToListAsync(cancellationToken);
        }

        public void Add(PracticeSession session)
        {
            _dbContext.Sessions.Add(session);
        }
    }
}

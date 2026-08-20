using Cadence.Practice.Domain.Ids;

namespace Cadence.Practice.Domain
{
    public interface ISessionRepository
    {
        Task<PracticeSession?> GetAsync(SessionId id, CancellationToken cancellationToken);
        Task<IReadOnlyList<PracticeSession>> ListAsync(PieceId? pieceId, DateTime? from, DateTime? to, CancellationToken cancellationToken);
        void Add(PracticeSession session);
    }
}

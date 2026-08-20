using Cadence.Repertoire.Domain.Ids;

namespace Cadence.Repertoire.Domain.Events
{
    public sealed record PieceMastered(PieceId Id, int TotalMinutes, DateTime OccurredAtUtc) : IDomainEvent;
}

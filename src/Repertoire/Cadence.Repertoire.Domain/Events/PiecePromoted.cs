using Cadence.Repertoire.Domain.Enums;
using Cadence.Repertoire.Domain.Ids;

namespace Cadence.Repertoire.Domain.Events
{
    public sealed record PiecePromoted(PieceId Id, LearningStatusEnum From, LearningStatusEnum To, DateTime OccurredAtUtc) : IDomainEvent;
}

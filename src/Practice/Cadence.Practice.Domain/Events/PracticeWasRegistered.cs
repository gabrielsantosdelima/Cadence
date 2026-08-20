using Cadence.Practice.Domain.Enums;
using Cadence.Practice.Domain.Ids;

namespace Cadence.Practice.Domain.Events
{
    public sealed record PracticeWasRegistered(
        SessionId SessionId,
        PieceId PieceId,
        DateTime StartedAtUtc,
        int Minutes,
        int Rating,
        PracticeFocus Focus) : IDomainEvent;
}

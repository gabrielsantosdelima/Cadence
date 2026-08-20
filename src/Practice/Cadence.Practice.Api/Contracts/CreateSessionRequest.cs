using Cadence.Practice.Domain.Enums;

namespace Cadence.Practice.Api.Contracts
{
    public sealed record CreateSessionRequest(
        Guid PieceId,
        string PieceTitle,
        DateTime StartedAtUtc,
        int DurationMinutes,
        int? TempoBpm,
        PracticeFocus Focus,
        int Quality,
        string? Notes);
}

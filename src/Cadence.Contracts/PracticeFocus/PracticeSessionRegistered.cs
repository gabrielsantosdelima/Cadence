namespace Cadence.Contracts.PracticeFocus
{
    public record PracticeSessionRegistered
    (
        Guid SessionId,
        Guid PieceId,
        DateTime StartedAtUtc,
        int DurationMinutes,
        int QualityRating,
        PracticeFocusEnum Focus,
        DateTime OrcurredAtUtc
    );
}

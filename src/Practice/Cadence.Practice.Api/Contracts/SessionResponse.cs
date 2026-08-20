using Cadence.Practice.Domain;

namespace Cadence.Practice.Api.Contracts
{
    public sealed record SessionResponse(
        Guid Id,
        Guid PieceId,
        string PieceTitle,
        DateTime StartedAtUtc,
        int DurationMinutes,
        int? TempoBpm,
        string Focus,
        int Quality,
        string? Notes,
        DateTime CreatedAtUtc)
    {
        public static SessionResponse From(PracticeSession session)
        {
            return new SessionResponse(
                session.Id.Value,
                session.Piece.PieceId.Value,
                session.Piece.Title,
                session.StartedAtUtc,
                session.Duration.Minutes,
                session.Tempo?.Bpm,
                session.Focus.ToString(),
                session.Quality.Value,
                session.Notes,
                session.CreatedAtUtc);
        }
    }
}

using Cadence.Repertoire.Domain;

namespace Cadence.Repertoire.Api.Contracts
{
    public sealed record PieceResponse(
        Guid Id,
        string Title,
        string? Composer,
        string Genre,
        string Difficulty,
        string? Key,
        string? ReferenceUrl,
        string Status,
        PracticeRecordResponse Record,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc)
    {
        public static PieceResponse From(Piece piece)
        {
            return new PieceResponse(
                piece.Id.Value,
                piece.Title.Value,
                piece.Composer,
                piece.Genre.ToString(),
                piece.Difficulty.ToString(),
                piece.Key?.Value,
                piece.ReferenceUrl?.AbsoluteUri,
                piece.Status.ToString(),
                PracticeRecordResponse.From(piece.Record),
                piece.CreatedAtUtc,
                piece.UpdatedAtUtc);
        }
    }

    public sealed record PracticeRecordResponse(
        int TotalMinutes,
        int SessionCount,
        DateTime? LastPracticedAtUtc,
        decimal? AverageQuality)
    {
        public static PracticeRecordResponse From(Domain.ValueObjects.PracticeRecord record)
        {
            return new PracticeRecordResponse(
                record.TotalMinutes,
                record.SessionCount,
                record.LastPracticedAtUtc,
                record.AverageQuality);
        }
    }
}

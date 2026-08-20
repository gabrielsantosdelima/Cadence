using Cadence.Repertoire.Domain.Enums;

namespace Cadence.Repertoire.Api.Contracts
{
    public sealed record UpdatePieceRequest(
        string Title,
        string? Composer,
        GenreEnum Genre,
        DifficultyEnum Difficulty,
        string? Key,
        string? ReferenceUrl);
}

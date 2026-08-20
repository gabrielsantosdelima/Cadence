using Cadence.Repertoire.Api.Contracts;
using Cadence.Repertoire.Domain;
using Cadence.Repertoire.Domain.Enums;
using Cadence.Repertoire.Domain.Ids;
using Cadence.Repertoire.Domain.ValueObjects;

namespace Cadence.Repertoire.Api.Endpoints
{
    public static class PieceEndpoints
    {
        public static IEndpointRouteBuilder MapPieceEndpoints(this IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/pieces");

            group.MapPost("/", CreatePiece);
            group.MapGet("/", ListPieces);
            group.MapGet("/{id:guid}", GetPiece);
            group.MapPut("/{id:guid}", UpdatePiece);
            group.MapPatch("/{id:guid}/status", ChangePieceStatus);
            group.MapDelete("/{id:guid}", DeletePiece);

            return app;
        }

        private static async Task<IResult> CreatePiece(
            CreatePieceRequest request,
            IPieceRepository pieceRepository,
            IUnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            Result<PieceTitle> titleResult = PieceTitle.Create(request.Title);
            if (titleResult.IsFailure)
                return titleResult.ToProblem();

            if (!TryParseReferenceUrl(request.ReferenceUrl, out Uri? referenceUrl, out IResult? referenceUrlError))
                return referenceUrlError!;

            if (!TryParseMusicalKey(request.Key, out MusicalKey? musicalKey, out IResult? musicalKeyError))
                return musicalKeyError!;

            Result<Piece> pieceResult = Piece.Create(
                titleResult.Value,
                request.Composer,
                request.Genre,
                request.Difficulty,
                musicalKey,
                referenceUrl);

            if (pieceResult.IsFailure)
                return pieceResult.ToProblem();

            Piece piece = pieceResult.Value;
            pieceRepository.Add(piece);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Results.Created($"/pieces/{piece.Id}", PieceResponse.From(piece));
        }

        private static async Task<IResult> ListPieces(
            LearningStatusEnum? status,
            GenreEnum? genre,
            IPieceRepository pieceRepository,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<Piece> pieces = await pieceRepository.ListAsync(status, genre, cancellationToken);
            return Results.Ok(pieces.Select(PieceResponse.From));
        }

        private static async Task<IResult> GetPiece(
            Guid id,
            IPieceRepository pieceRepository,
            CancellationToken cancellationToken)
        {
            Piece? piece = await pieceRepository.GetAsync(PieceId.From(id), cancellationToken);
            return piece is null
                ? Results.NotFound()
                : Results.Ok(PieceResponse.From(piece));
        }

        private static async Task<IResult> UpdatePiece(
            Guid id,
            UpdatePieceRequest request,
            IPieceRepository pieceRepository,
            IUnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            Piece? piece = await pieceRepository.GetAsync(PieceId.From(id), cancellationToken);
            if (piece is null)
                return Results.NotFound();

            Result<PieceTitle> titleResult = PieceTitle.Create(request.Title);
            if (titleResult.IsFailure)
                return titleResult.ToProblem();

            if (!TryParseReferenceUrl(request.ReferenceUrl, out Uri? referenceUrl, out IResult? referenceUrlError))
                return referenceUrlError!;

            if (!TryParseMusicalKey(request.Key, out MusicalKey? musicalKey, out IResult? musicalKeyError))
                return musicalKeyError!;

            Result updateResult = piece.UpdateDetails(
                titleResult.Value,
                request.Composer,
                request.Genre,
                request.Difficulty,
                musicalKey,
                referenceUrl);

            if (updateResult.IsFailure)
                return updateResult.ToProblem();

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Results.Ok(PieceResponse.From(piece));
        }

        private static async Task<IResult> ChangePieceStatus(
            Guid id,
            ChangeStatusRequest request,
            IPieceRepository pieceRepository,
            IUnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            if (!Enum.TryParse(request.Status, ignoreCase: true, out LearningStatusEnum targetStatus))
                return Results.Problem(
                    title: "ChangeStatus.UnknownStatus",
                    detail: $"'{request.Status}' is not a recognized learning status",
                    statusCode: StatusCodes.Status400BadRequest);

            Piece? piece = await pieceRepository.GetAsync(PieceId.From(id), cancellationToken);
            if (piece is null)
                return Results.NotFound();

            Result changeStatusResult = piece.ChangeStatus(targetStatus);
            if (changeStatusResult.IsFailure)
                return changeStatusResult.ToProblem(StatusCodes.Status409Conflict);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Results.Ok(PieceResponse.From(piece));
        }

        private static async Task<IResult> DeletePiece(
            Guid id,
            IPieceRepository pieceRepository,
            IUnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            Piece? piece = await pieceRepository.GetAsync(PieceId.From(id), cancellationToken);
            if (piece is null)
                return Results.NotFound();

            pieceRepository.Remove(piece);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Results.NoContent();
        }

        private static bool TryParseReferenceUrl(string? rawReferenceUrl, out Uri? referenceUrl, out IResult? error)
        {
            if (rawReferenceUrl is null)
            {
                referenceUrl = null;
                error = null;
                return true;
            }

            if (!Uri.TryCreate(rawReferenceUrl, UriKind.RelativeOrAbsolute, out referenceUrl))
            {
                error = Results.Problem(
                    title: "ReferenceUrl.Malformed",
                    detail: "ReferenceUrl must be a well-formed URL",
                    statusCode: StatusCodes.Status400BadRequest);
                return false;
            }

            error = null;
            return true;
        }

        private static bool TryParseMusicalKey(string? rawKey, out MusicalKey? musicalKey, out IResult? error)
        {
            if (rawKey is null)
            {
                musicalKey = null;
                error = null;
                return true;
            }

            Result<MusicalKey> keyResult = MusicalKey.Create(rawKey);
            if (keyResult.IsFailure)
            {
                musicalKey = null;
                error = keyResult.ToProblem();
                return false;
            }

            musicalKey = keyResult.Value;
            error = null;
            return true;
        }
    }
}

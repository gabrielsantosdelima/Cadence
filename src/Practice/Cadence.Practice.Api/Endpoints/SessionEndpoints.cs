using Cadence.Practice.Api.Contracts;
using Cadence.Practice.Api.Handlers;
using Cadence.Practice.Domain;
using Cadence.Practice.Domain.Events;
using Cadence.Practice.Domain.Ids;
using Cadence.Practice.Domain.ValueObjects;
using Cadence.Practice.Infrastructure;
using MassTransit;

namespace Cadence.Practice.Api.Endpoints
{
    public static class SessionEndpoints
    {
        public static IEndpointRouteBuilder MapSessionEndpoints(this IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/sessions");

            group.MapPost("/", CreateSession);
            group.MapGet("/", ListSessions);
            group.MapGet("/{id:guid}", GetSession);

            return app;
        }

        private static async Task<IResult> CreateSession(
            CreateSessionRequest request,
            ISessionRepository sessionRepository,
            PracticeDbContext dbContext,
            IPublishEndpoint publishEndpoint,
            ILogger<PracticeSession> logger,
            CancellationToken cancellationToken)
        {
            Result<PieceReference> pieceReferenceResult = PieceReference.Create(
                PieceId.From(request.PieceId),
                request.PieceTitle);
            if (pieceReferenceResult.IsFailure)
                return pieceReferenceResult.ToProblem();

            Result<PracticeDuration> durationResult = PracticeDuration.Create(request.DurationMinutes);
            if (durationResult.IsFailure)
                return durationResult.ToProblem();

            Tempo? tempo = null;
            if (request.TempoBpm is not null)
            {
                Result<Tempo> tempoResult = Tempo.Create(request.TempoBpm.Value);
                if (tempoResult.IsFailure)
                    return tempoResult.ToProblem();
                tempo = tempoResult.Value;
            }

            Result<QualityRating> qualityResult = QualityRating.Create(request.Quality);
            if (qualityResult.IsFailure)
                return qualityResult.ToProblem();

            Result<PracticeSession> sessionResult = PracticeSession.Register(
                pieceReferenceResult.Value,
                request.StartedAtUtc,
                durationResult.Value,
                tempo,
                request.Focus,
                qualityResult.Value,
                request.Notes,
                DateTime.UtcNow);

            if (sessionResult.IsFailure)
                return sessionResult.ToProblem();

            PracticeSession session = sessionResult.Value;
            sessionRepository.Add(session);
            await dbContext.SaveChangesAsync(cancellationToken);

            foreach (PracticeWasRegistered domainEvent in session.DomainEvents.OfType<PracticeWasRegistered>())
                await PracticeWasRegisteredHandler.PublishAsync(domainEvent, publishEndpoint, logger, cancellationToken);

            session.ClearDomainEvents();

            return Results.Created($"/sessions/{session.Id}", SessionResponse.From(session));
        }

        private static async Task<IResult> ListSessions(
            Guid? pieceId,
            DateTime? from,
            DateTime? to,
            ISessionRepository sessionRepository,
            CancellationToken cancellationToken)
        {
            IReadOnlyList<PracticeSession> sessions = await sessionRepository.ListAsync(
                pieceId is null ? null : PieceId.From(pieceId.Value),
                from,
                to,
                cancellationToken);

            return Results.Ok(sessions.Select(SessionResponse.From));
        }

        private static async Task<IResult> GetSession(
            Guid id,
            ISessionRepository sessionRepository,
            CancellationToken cancellationToken)
        {
            PracticeSession? session = await sessionRepository.GetAsync(SessionId.From(id), cancellationToken);
            return session is null
                ? Results.NotFound()
                : Results.Ok(SessionResponse.From(session));
        }
    }
}

using Cadence.Contracts.PracticeFocus;
using Cadence.Repertoire.Domain;
using Cadence.Repertoire.Domain.Ids;
using Cadence.Repertoire.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cadence.Repertoire.Api.Consumers
{
    public sealed class PracticeSessionRegisteredConsumer : IConsumer<PracticeSessionRegistered>
    {
        private readonly IPieceRepository _pieceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RepertoireDbContext _dbContext;
        private readonly ILogger<PracticeSessionRegisteredConsumer> _logger;

        public PracticeSessionRegisteredConsumer(
            IPieceRepository pieceRepository,
            IUnitOfWork unitOfWork,
            RepertoireDbContext dbContext,
            ILogger<PracticeSessionRegisteredConsumer> logger)
        {
            _pieceRepository = pieceRepository;
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PracticeSessionRegistered> context)
        {
            CancellationToken cancellationToken = context.CancellationToken;
            Guid messageId = context.MessageId!.Value;

            bool alreadyProcessed = await _dbContext.ProcessedMessages
                .AnyAsync(processedMessage => processedMessage.MessageId == messageId, cancellationToken);

            if (alreadyProcessed)
                return;

            PracticeSessionRegistered integrationEvent = context.Message;
            PieceId pieceId = PieceId.From(integrationEvent.PieceId);

            Piece? piece = await _pieceRepository.GetAsync(pieceId, cancellationToken);
            if (piece is null)
            {
                _logger.LogWarning(
                    "Ignoring PracticeSessionRegistered for unknown PieceId {PieceId} and SessionId {SessionId}",
                    integrationEvent.PieceId,
                    integrationEvent.SessionId);
                return;
            }

            piece.RegisterPractice(
                integrationEvent.DurationMinutes,
                integrationEvent.QualityRating,
                integrationEvent.OrcurredAtUtc);

            _dbContext.ProcessedMessages.Add(
                new ProcessedMessage(messageId, nameof(PracticeSessionRegisteredConsumer), DateTime.UtcNow));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Registered practice for PieceId {PieceId} from SessionId {SessionId} (MessageId {MessageId})",
                integrationEvent.PieceId,
                integrationEvent.SessionId,
                messageId);
        }
    }
}

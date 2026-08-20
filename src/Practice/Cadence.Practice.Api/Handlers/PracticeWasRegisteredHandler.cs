using Cadence.Contracts.PracticeFocus;
using Cadence.Practice.Domain.Events;
using MassTransit;

namespace Cadence.Practice.Api.Handlers
{
    public static class PracticeWasRegisteredHandler
    {
        public static async Task PublishAsync(
            PracticeWasRegistered domainEvent,
            IPublishEndpoint publishEndpoint,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            PracticeFocusEnum focus = (PracticeFocusEnum)domainEvent.Focus;

            var integrationEvent = new PracticeSessionRegistered(
                domainEvent.SessionId.Value,
                domainEvent.PieceId.Value,
                domainEvent.StartedAtUtc,
                domainEvent.Minutes,
                domainEvent.Rating,
                focus,
                DateTime.UtcNow);

            Guid messageId = NewId.NextGuid();

            await publishEndpoint.Publish(integrationEvent, publishContext =>
            {
                publishContext.MessageId = messageId;
            }, cancellationToken);

            logger.LogInformation(
                "Published PracticeSessionRegistered for PieceId {PieceId} and SessionId {SessionId} (MessageId {MessageId})",
                integrationEvent.PieceId,
                integrationEvent.SessionId,
                messageId);
        }
    }
}

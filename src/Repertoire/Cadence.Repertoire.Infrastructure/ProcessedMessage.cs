namespace Cadence.Repertoire.Infrastructure
{
    public sealed class ProcessedMessage
    {
        public Guid MessageId { get; private set; }
        public string ConsumerName { get; private set; }
        public DateTime ProcessedAtUtc { get; private set; }

        private ProcessedMessage()
        {
            ConsumerName = null!;
        }

        public ProcessedMessage(Guid messageId, string consumerName, DateTime processedAtUtc)
        {
            MessageId = messageId;
            ConsumerName = consumerName;
            ProcessedAtUtc = processedAtUtc;
        }
    }
}

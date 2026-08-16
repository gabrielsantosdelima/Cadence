using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadence.Repertoire.Infrastructure.Configurations
{
    public sealed class ProcessedMessageConfiguration : IEntityTypeConfiguration<ProcessedMessage>
    {
        public void Configure(EntityTypeBuilder<ProcessedMessage> builder)
        {
            builder.HasKey(processedMessage => processedMessage.MessageId);

            builder.Property(processedMessage => processedMessage.ConsumerName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(processedMessage => processedMessage.ProcessedAtUtc)
                .IsRequired();

            builder.HasIndex(processedMessage => new { processedMessage.MessageId, processedMessage.ConsumerName })
                .IsUnique();
        }
    }
}

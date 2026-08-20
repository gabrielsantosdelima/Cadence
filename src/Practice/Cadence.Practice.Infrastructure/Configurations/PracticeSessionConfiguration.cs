using Cadence.Practice.Domain;
using Cadence.Practice.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadence.Practice.Infrastructure.Configurations
{
    public sealed class PracticeSessionConfiguration : IEntityTypeConfiguration<PracticeSession>
    {
        public void Configure(EntityTypeBuilder<PracticeSession> builder)
        {
            builder.HasKey(session => session.Id);
            builder.Property(session => session.Id)
                .ValueGeneratedNever();

            builder.ComplexProperty(session => session.Piece, pieceBuilder =>
            {
                pieceBuilder.Property(reference => reference.PieceId)
                    .HasColumnName("PieceId")
                    .IsRequired();
                pieceBuilder.Property(reference => reference.Title)
                    .HasColumnName("PieceTitle")
                    .HasMaxLength(120)
                    .IsRequired();
            });

            builder.Property(session => session.StartedAtUtc)
                .IsRequired();

            builder.Property(session => session.Duration)
                .HasConversion(new PracticeDurationConverter())
                .IsRequired();

            builder.Property(session => session.Tempo)
                .HasConversion(new TempoConverter());

            builder.Property(session => session.Focus)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(session => session.Quality)
                .HasConversion(new QualityRatingConverter())
                .IsRequired();

            builder.Property(session => session.Notes)
                .HasMaxLength(500);

            builder.Property(session => session.CreatedAtUtc)
                .IsRequired();

            builder.HasIndex(session => session.StartedAtUtc);
        }
    }
}

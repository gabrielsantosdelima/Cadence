using Cadence.Repertoire.Domain;
using Cadence.Repertoire.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cadence.Repertoire.Infrastructure.Configurations
{
    public sealed class PieceConfiguration : IEntityTypeConfiguration<Piece>
    {
        public void Configure(EntityTypeBuilder<Piece> builder)
        {
            builder.HasKey(piece => piece.Id);
            builder.Property(piece => piece.Id)
                .ValueGeneratedNever();

            builder.Property(piece => piece.Title)
                .HasConversion(new PieceTitleConverter())
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(piece => piece.Composer)
                .HasMaxLength(120);

            builder.Property(piece => piece.Genre)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(piece => piece.Difficulty)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(piece => piece.Key)
                .HasConversion(new MusicalKeyConverter())
                .HasMaxLength(20);

            builder.Property(piece => piece.ReferenceUrl)
                .HasConversion(
                    referenceUrl => referenceUrl == null ? null : referenceUrl.AbsoluteUri,
                    value => value == null ? null : new Uri(value));

            builder.Property(piece => piece.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.ComplexProperty(piece => piece.Record, recordBuilder =>
            {
                recordBuilder.Property(record => record.TotalMinutes)
                    .HasColumnName("TotalMinutes")
                    .IsRequired();
                recordBuilder.Property(record => record.SessionCount)
                    .HasColumnName("SessionCount")
                    .IsRequired();
                recordBuilder.Property(record => record.LastPracticedAtUtc)
                    .HasColumnName("LastPracticedAtUtc");
                recordBuilder.Property(record => record.AverageQuality)
                    .HasColumnName("AverageQuality");
            });

            builder.Property(piece => piece.CreatedAtUtc).IsRequired();
            builder.Property(piece => piece.UpdatedAtUtc).IsRequired();

            builder.HasIndex(piece => piece.Status);
            builder.HasIndex(piece => piece.Genre);
        }
    }
}

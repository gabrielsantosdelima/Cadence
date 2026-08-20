using Cadence.Practice.Domain;
using Cadence.Practice.Domain.Ids;
using Cadence.Practice.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;

namespace Cadence.Practice.Infrastructure
{
    public class PracticeDbContext : DbContext
    {
        public PracticeDbContext(DbContextOptions<PracticeDbContext> options)
            : base(options)
        {
        }

        public DbSet<PracticeSession> Sessions => Set<PracticeSession>();

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<SessionId>()
                .HaveConversion<SessionIdConverter, SessionIdComparer>();

            configurationBuilder.Properties<PieceId>()
                .HaveConversion<PieceIdConverter, PieceIdComparer>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PracticeDbContext).Assembly);
        }
    }
}

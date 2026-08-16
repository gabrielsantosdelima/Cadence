using Cadence.Repertoire.Domain;
using Cadence.Repertoire.Domain.Common;
using Cadence.Repertoire.Domain.Events;
using Cadence.Repertoire.Domain.Ids;
using Cadence.Repertoire.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;

namespace Cadence.Repertoire.Infrastructure
{
    public class RepertoireDbContext : DbContext, IUnitOfWork
    {
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public RepertoireDbContext(DbContextOptions<RepertoireDbContext> options, IDomainEventDispatcher domainEventDispatcher)
            : base(options)
        {
            _domainEventDispatcher = domainEventDispatcher;
        }

        public DbSet<Piece> Pieces => Set<Piece>();
        public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<PieceId>()
                .HaveConversion<PieceIdConverter, PieceIdComparer>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepertoireDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            List<AggregateRoot> aggregatesWithEvents = ChangeTracker.Entries<AggregateRoot>()
                .Select(entry => entry.Entity)
                .Where(aggregate => aggregate.DomainEvents.Count > 0)
                .ToList();

            List<IDomainEvent> domainEvents = aggregatesWithEvents
                .SelectMany(aggregate => aggregate.DomainEvents)
                .ToList();

            int result = await base.SaveChangesAsync(cancellationToken);

            foreach (AggregateRoot aggregate in aggregatesWithEvents)
                aggregate.ClearDomainEvents();

            await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);

            return result;
        }
    }
}

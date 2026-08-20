using Cadence.Repertoire.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Cadence.Repertoire.Infrastructure
{
    public sealed class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken)
        {
            foreach (IDomainEvent domainEvent in domainEvents)
            {
                Type handlerInterfaceType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                System.Reflection.MethodInfo handleMethod = handlerInterfaceType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))!;

                foreach (object? handler in (IEnumerable<object?>)_serviceProvider.GetServices(handlerInterfaceType))
                {
                    if (handler is null)
                        continue;

                    await (Task)handleMethod.Invoke(handler, [domainEvent, cancellationToken])!;
                }
            }
        }
    }
}

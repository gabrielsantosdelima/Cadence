using MassTransit;

namespace Cadence.Practice.Api.Messaging
{
    public sealed class CorrelationIdPublishFilter<T> : IFilter<PublishContext<T>> where T : class
    {
        private const string CorrelationIdHeaderName = "X-Correlation-ID";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationIdPublishFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope(nameof(CorrelationIdPublishFilter<T>));
        }

        public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            if (_httpContextAccessor.HttpContext?.Items[CorrelationIdHeaderName] is string correlationId)
            {
                context.Headers.Set(CorrelationIdHeaderName, correlationId);
            }

            return next.Send(context);
        }
    }
}

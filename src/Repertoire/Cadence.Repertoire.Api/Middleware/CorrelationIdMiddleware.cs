namespace Cadence.Repertoire.Api.Middleware
{
    public sealed class CorrelationIdMiddleware
    {
        private const string CorrelationIdHeaderName = "X-Correlation-ID";
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId)
                || string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.CreateVersion7().ToString();
            }

            context.Response.OnStarting(() =>
            {
                context.Response.Headers[CorrelationIdHeaderName] = correlationId.ToString();
                return Task.CompletedTask;
            });

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId.ToString()
            }))
            {
                await _next(context);
            }
        }
    }
}

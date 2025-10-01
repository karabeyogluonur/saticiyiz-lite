using Serilog.Context;

namespace SL.Web.Mvc.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CorrelationIdHeaderName = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string correlationId = context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault()
                              ?? context.TraceIdentifier;

            context.TraceIdentifier = correlationId;
            context.Response.Headers.Add(CorrelationIdHeaderName, correlationId);

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}

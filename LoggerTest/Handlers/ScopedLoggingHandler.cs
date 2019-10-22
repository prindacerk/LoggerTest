using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LoggerTest.Handlers
{
    public class ScopedLoggingHandler : ExceptionHandler
    {
        private readonly ILogger _logger;
        const string CorrelationIdHeaderName = "CorrelationID";
        private readonly RequestDelegate _next;

        public ScopedLoggingHandler(RequestDelegate next, ILogger<ScopedLoggingHandler> scopeLogger)
        {
            this._next = next ?? throw new System.ArgumentNullException(nameof(next));
            _logger = scopeLogger ?? throw new ArgumentNullException(nameof(scopeLogger));
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null) throw new System.ArgumentNullException(nameof(context));

            var correlationId = GetOrAddCorrelationHeader(context);

            var loggerState = new Dictionary<string, object>
            {
                ["CorrelationID"] = correlationId,
                ["Host"] = context?.Request?.Host,
                ["Protocol"] = context?.Request?.Protocol,
                ["IPAddress"] = context?.Connection?.RemoteIpAddress?.MapToIPv4().ToString(),
                ["Agent"] = context?.Request?.Headers["User-Agent"],
                ["Header"] = context?.Request?.Headers != null ? JsonConvert.SerializeObject(context?.Request?.Headers?.ToDictionary(h => h.Key, h => h.Value.ToString())) : null,
                //Add any number of properties to be logged under a single scope
            };

            try
            {
                using (_logger.BeginScope(loggerState))
                {
                    await _next(context);
                }
            }
            catch (Exception ex) when (HandleExceptionAsync(context, ex, _logger))
            {
            }
        }

        private string GetOrAddCorrelationHeader(HttpContext context)
        {
            if (context == null) throw new System.ArgumentNullException(nameof(context));

            if (string.IsNullOrWhiteSpace(context.Request.Headers[CorrelationIdHeaderName]))
                context.Request.Headers[CorrelationIdHeaderName] = Guid.NewGuid().ToString();

            return context.Request.Headers[CorrelationIdHeaderName];
        }
    }
}

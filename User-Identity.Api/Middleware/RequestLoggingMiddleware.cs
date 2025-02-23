using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace User_Identity.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["RequestId"] = httpContext.TraceIdentifier,
                ["TraceId"] = Activity.Current?.Id ?? "NA",

            }))
            {
                
                _logger.LogInformation("Handling request: {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
                await _next(httpContext);
                _logger.LogInformation("Finished handling request.");
            }
        }
    }
}

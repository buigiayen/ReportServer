using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;

namespace ServerSide.ServerExceptions
{
    public class AddException
    {
        private readonly RequestDelegate _next;
        private ILogger<AddException> _ILogger;
        public AddException(RequestDelegate next, ILogger<AddException> ILogger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _ILogger = ILogger;
        }


        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            var errorStatusCode = StatusCodes.Status500InternalServerError;
            if (exception is UnauthorizedAccessException) errorStatusCode = StatusCodes.Status401Unauthorized;
            if (exception is ArgumentNullException) errorStatusCode = StatusCodes.Status400BadRequest;
            if (exception is OutOfMemoryException) errorStatusCode = StatusCodes.Status500InternalServerError;
            if (exception is NotSupportedException) errorStatusCode = StatusCodes.Status502BadGateway;
            if (exception is FormatException) errorStatusCode = StatusCodes.Status400BadRequest;
            if (exception is ArgumentException) errorStatusCode = StatusCodes.Status400BadRequest;
            if (exception is InvalidOperationException) errorStatusCode = StatusCodes.Status400BadRequest;

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = errorStatusCode;
            var errorDetailsJson = new ProblemDetails
            {
                Detail = exception.Message,
                Instance = AppDomain.CurrentDomain.FriendlyName,
                Status = errorStatusCode,
                Title = Guid.NewGuid().ToString(),
                Type = context.Request.Method,
                
               
            };
            _ILogger.LogError(JsonConvert.SerializeObject(errorDetailsJson));
            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorDetailsJson));
        }
    }
}

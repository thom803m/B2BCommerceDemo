using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace B2BCommerceDemo.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                HttpRequestException => StatusCodes.Status502BadGateway,

                DbUpdateException => StatusCodes.Status409Conflict,

                KeyNotFoundException => StatusCodes.Status404NotFound,

                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,

                InvalidOperationException => StatusCodes.Status400BadRequest,

                ArgumentException => StatusCodes.Status400BadRequest,

                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var problem = new ProblemDetails
            {
                Status = statusCode,

                Title = statusCode switch
                {
                    502 => "External service error",
                    409 => "Conflict",
                    404 => "Resource not found",
                    401 => "Unauthorized",
                    400 => "Bad request",
                    _ => "Internal server error"
                },

                Detail = statusCode switch
                {
                    500 => "An unexpected error occurred.",
                    502 => "An external service request failed.",
                    _ => exception.Message
                }
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}


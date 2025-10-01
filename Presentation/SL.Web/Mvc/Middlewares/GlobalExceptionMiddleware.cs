using Microsoft.AspNetCore.Mvc;
using SL.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace SL.Web.Mvc.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            string correlationId = context.TraceIdentifier;
            ProblemDetails problemDetails = CreateProblemDetails(exception, correlationId);

            _logger.LogError(exception, 
                "Unhandled exception occurred. CorrelationId: {CorrelationId}", 
                correlationId);

            context.Response.StatusCode = problemDetails.Status ?? 500;
            context.Response.ContentType = "application/json";

            string json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        private ProblemDetails CreateProblemDetails(Exception exception, string correlationId)
        {
            return exception switch
            {
                ValidationException validationEx => new ProblemDetails
                {
                    Status = 400,
                    Title = "Validation Error",
                    Detail = validationEx.UserMessage,
                    Instance = correlationId,
                    Extensions = { ["errorCode"] = validationEx.ErrorCode }
                },
                BusinessException businessEx => new ProblemDetails
                {
                    Status = 400,
                    Title = "Business Error",
                    Detail = businessEx.UserMessage,
                    Instance = correlationId,
                    Extensions = { ["errorCode"] = businessEx.ErrorCode }
                },
                TechnicalException technicalEx => new ProblemDetails
                {
                    Status = 500,
                    Title = "Technical Error",
                    Detail = technicalEx.UserMessage,
                    Instance = correlationId,
                    Extensions = { ["errorCode"] = technicalEx.ErrorCode }
                },
                _ => new ProblemDetails
                {
                    Status = 500,
                    Title = "Internal Server Error",
                    Detail = _environment.IsDevelopment() ? exception.Message : "An error occurred",
                    Instance = correlationId
                }
            };
        }
    }
}

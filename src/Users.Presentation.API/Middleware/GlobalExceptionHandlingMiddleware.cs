using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using Users.Application.Dto;

namespace Users.Presentation.API.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message;

            switch (exception)
            {
                case ArgumentException argumentException:
                    status = HttpStatusCode.BadRequest;
                    message = argumentException.Message;

                    break;

                case KeyNotFoundException keyNotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = keyNotFoundException.Message;

                    break;

                case UnauthorizedAccessException unauthorizedAccessException:
                    status = HttpStatusCode.Unauthorized;
                    message = unauthorizedAccessException.Message;

                    break;

                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "Error. " + exception.Message;

                    break;
            }

            _logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            var errorResponse = new ResponseDto
            {
                IsSuccess =false,
                ErrorMessages = { message },
            };

            var json = JsonSerializer.Serialize(errorResponse);

            await context.Response.WriteAsync(json);
        }
    }
}

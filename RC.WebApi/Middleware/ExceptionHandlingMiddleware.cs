using RC.Domain.Exceptions;
using RC.Shared.Models.Results;

namespace RC.WebApi.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;
        
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

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var (statusCode, message) = ex switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, ex.Message),
                ConflictException => (StatusCodes.Status409Conflict, ex.Message),
                BusinessRuleException => (StatusCodes.Status422UnprocessableEntity, ex.Message),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "Internal Error.")
            };

            var response = ApiResponse<object>.Fail(message);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}

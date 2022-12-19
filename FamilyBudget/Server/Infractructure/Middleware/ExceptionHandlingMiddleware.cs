using FamilyBudget.Server.Exceptions;
using System.Net;

namespace FamilyBudget.Server.Infractructure.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ResourceNotFoundException exception)
            {
                _logger.LogError(exception, exception.Message);

                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

                await HandleExceptionAsync(httpContext, exception.Message);
            }
            catch (UnauthorizedException exception)
            {
                _logger.LogError(exception, exception.Message);

                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                await HandleExceptionAsync(httpContext, exception.Message);
            }
            catch (BadRequestException exception)
            {
                _logger.LogError(exception, exception.Message);

                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                WriteDefaultResponseContentType(httpContext);

                await httpContext.Response.WriteAsync(new BadRequestDetails
                {
                    StatusCode = httpContext.Response.StatusCode,
                    Errors = exception.Errors
                }.ToString());
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await HandleExceptionAsync(httpContext);
            }
        }

        private Task HandleExceptionAsync(HttpContext httpContext, string message = null)
        {
            WriteDefaultResponseContentType(httpContext);

            return httpContext.Response.WriteAsync(new ErrorDetails
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = message ?? "Internal server error"
            }.ToString());
        }

        private void WriteDefaultResponseContentType(HttpContext context)
        {
            context.Response.ContentType = "application/json";
        }
    }
}

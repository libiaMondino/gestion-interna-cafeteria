using Application.Exceptions;
namespace Presentation.Middlewares
{
    public class GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger
        )
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch(Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            string message;

            switch(exception)
            {
                case InvalidCredentialsException: // Cuando falla autenticación
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = exception.Message;
                    break;
                default:
                    logger.LogError(exception, "Error no controlado");
                    statusCode = StatusCodes.Status500InternalServerError;
                    message = "Ocurrió un error inesperado";
                    break;
            }

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new { status = statusCode, error = message });
        }
    }
}

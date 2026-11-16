using Application.Exceptions;
using Domain.Exceptions;
using System.Security.Authentication;

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
            catch (OperationCanceledException)
                when (context.RequestAborted.IsCancellationRequested)
            {
                // La solicitud fue cancelada por el cliente.
                return;
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            if (context.Response.HasStarted)
            {
                logger.LogError(
                    exception,
                    "La respuesta ya había comenzado y ocurrió un error."
                );

                throw exception;
            }

            int statusCode;
            string message;

            switch (exception)
            {
                case InvalidCredentialsException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = exception.Message;
                    break;

                case InvalidCredentialException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = "Credenciales inválidas.";
                    break;

                case BusinessRuleException:
                    statusCode = StatusCodes.Status409Conflict;
                    message = exception.Message;
                    break;

                case ArgumentException:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = exception.Message;
                    break;

                case KeyNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    message = exception.Message;
                    break;

                case InvalidOperationException:
                    statusCode = StatusCodes.Status409Conflict;
                    message = exception.Message;
                    break;

                default:
                    logger.LogError(exception, "Error no controlado");

                    statusCode = StatusCodes.Status500InternalServerError;
                    message = "Ocurrió un error inesperado.";
                    break;
            }

            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(new
            {
                status = statusCode,
                error = message
            });
        }
    }
}

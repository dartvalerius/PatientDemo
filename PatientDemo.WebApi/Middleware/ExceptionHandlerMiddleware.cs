using System.Net;
using System.Text.Json;
using FluentValidation;
using PatientDemo.Application.Exceptions;

namespace PatientDemo.WebApi.Middleware;

/// <summary>
/// Обработчик возникших исключений при обработке запроса
/// </summary>
public class ExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            await HandlerExceptionAsync(context, e);
        }
    }

    private Task HandlerExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        switch (exception)
        {
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(validationException.Errors.Select(x => x.ErrorMessage));
                break;
            case NotFoundException notFoundException:
                code = HttpStatusCode.NotFound;
                result = JsonSerializer.Serialize(notFoundException.Message);
                break;

            case FormatException formatException:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(formatException.Message);
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        if (string.IsNullOrEmpty(result))
            result = JsonSerializer.Serialize(exception.Message);

        return context.Response.WriteAsync(result);
    }
}
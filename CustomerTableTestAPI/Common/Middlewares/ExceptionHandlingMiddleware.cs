using System.Net;
using System.Text.Json;
using CustomerTableTest.Common.Responses;
using CustomerTableTest.Common.Exceptions;
using FluentValidation;

namespace CustomerTableTestAPI.Common.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException vex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var errs = vex.Errors
                .Select(e => new ErrorItem { Code = e.ErrorCode, Field = e.PropertyName, Description = e.ErrorMessage })
                .ToList();

            await Write(context, BaseResponse<object>.Fail("Validation failed", errs));
        }
        catch (NotFoundException nf)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await Write(context, BaseResponse<object>.Fail(nf.Message));
        }
        catch (ForbiddenException fb)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await Write(context, BaseResponse<object>.Fail(fb.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await Write(context, BaseResponse<object>.Fail("Unexpected error occurred"));
        }
    }

    private static Task Write(HttpContext context, object payload)
    {
        context.Response.ContentType = "application/json";
        var json = JsonSerializer.Serialize(payload);
        return context.Response.WriteAsync(json);
    }
}


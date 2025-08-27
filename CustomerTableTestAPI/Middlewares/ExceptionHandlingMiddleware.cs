// ExceptionHandlingMiddleware.cs
using CustomerTableTest.Common.Exceptions;
using CustomerTableTest.Common.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
// ...

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (FluentValidation.ValidationException vex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            var errs = vex.Errors.Select(e => new ErrorItem { Code = e.ErrorCode, Field = e.PropertyName, Description = e.ErrorMessage }).ToList();
            await Write(context, BaseResponse<object>.Fail("Validation failed", errs));
        }
        catch (DbUpdateException dbex)
        {
            _logger.LogError(dbex, "DB update error");
            context.Response.StatusCode = StatusCodes.Status409Conflict; // أو 500 حسب رؤيتك
            var msg = _env.IsDevelopment() ? $"DB error: {dbex.GetBaseException().Message}" : "Database error";
            await Write(context, BaseResponse<object>.Fail(msg));
        }
        catch (NotFoundException nf)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await Write(context, BaseResponse<object>.Fail(nf.Message));
        }
        catch (ForbiddenException fb)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await Write(context, BaseResponse<object>.Fail(fb.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var msg = _env.IsDevelopment() ? ex.GetBaseException().Message : "Unexpected error occurred";
            await Write(context, BaseResponse<object>.Fail(msg));
        }
    }

    private static Task Write(HttpContext context, object payload)
    {
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(payload);
    }
}

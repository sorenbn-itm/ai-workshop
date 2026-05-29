using Microsoft.AspNetCore.Mvc.Filters;

namespace CleanCQRSPOC.Presentation.Filters;

public class LoggingFilter(ILogger<LoggingFilter> logger) : IAsyncActionFilter
{
    private readonly ILogger<LoggingFilter> _logger = logger;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var method = context.HttpContext.Request.Method;
        var path = context.HttpContext.Request.Path;
        var arguments = string.Join(", ", context.ActionArguments.Select(kv => $"{kv.Key}={kv.Value}"));
        _logger.LogInformation("Request: {Method} {Path} Arguments: {Arguments}", method, path, arguments);

        var executedContext = await next();

        var statusCode = executedContext.HttpContext.Response.StatusCode;
        _logger.LogInformation("Response: {StatusCode}", statusCode);
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CloudCiApi.Middleware;

public class ApiKeyMiddleware
{
    // The header callers must send.
    private const string HeaderName = "X-Api-Key";

    // The configuration key bound from appsettings / env vars / secrets.
    private const string ConfigKey = "ApiKey:Value";

    private readonly RequestDelegate _next;
    private readonly string? _expectedKey;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _expectedKey = configuration[ConfigKey];
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // No header at all: 401 Unauthorized + WWW-Authenticate: ApiKey.
        if (!context.Request.Headers.TryGetValue(HeaderName, out var providedKey)
            || string.IsNullOrWhiteSpace(providedKey))
        {
            context.Response.Headers.Append("WWW-Authenticate", "ApiKey");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("API key is missing.");
            return;
        }

        // Header present but doesn't match: 403 Forbidden.
        if (string.IsNullOrEmpty(_expectedKey)
            || !string.Equals(providedKey, _expectedKey, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("API key is invalid.");
            return;
        }

        // Happy path: hand off to the next middleware (eventually the controller).
        await _next(context);
    }
}

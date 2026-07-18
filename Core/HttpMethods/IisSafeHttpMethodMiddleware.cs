using AspNetHttpMethods = Microsoft.AspNetCore.Http.HttpMethods;

namespace verii_metivon_api.Core.HttpMethods;

/// <summary>
/// Restores tunneled POST requests to their native HTTP method before routing.
/// Supported transports:
/// POST /resource/update -> PUT /resource
/// POST /resource/delete -> DELETE /resource
/// X-HTTP-Method-Override: PUT|PATCH|DELETE
/// </summary>
public sealed class IisSafeHttpMethodMiddleware(RequestDelegate next)
{
    public const string OriginalMethodItem = "Metivon.OriginalHttpMethod";
    public const string OriginalPathItem = "Metivon.OriginalRequestPath";

    public async Task InvokeAsync(HttpContext context)
    {
        if (AspNetHttpMethods.IsPost(context.Request.Method)
            && context.Request.Path.StartsWithSegments("/api"))
        {
            var originalPath = context.Request.Path;
            if (TryResolveSuffix(originalPath, out var path, out var method))
            {
                Apply(context, path, method, originalPath);
            }
            else if (TryResolveHeader(context.Request, out method))
            {
                Apply(context, originalPath, method, originalPath);
            }
        }

        await next(context);
    }

    private static void Apply(HttpContext context, PathString path, string method, PathString originalPath)
    {
        context.Items[OriginalMethodItem] = context.Request.Method;
        context.Items[OriginalPathItem] = originalPath.Value;
        context.Request.Method = method;
        context.Request.Path = path;
    }

    internal static bool TryResolveSuffix(
        PathString requestPath,
        out PathString normalizedPath,
        out string normalizedMethod)
    {
        var path = requestPath.Value ?? string.Empty;
        normalizedPath = requestPath;
        normalizedMethod = AspNetHttpMethods.Post;

        if (TryStripSuffix(path, "/update", out normalizedPath))
        {
            normalizedMethod = AspNetHttpMethods.Put;
            return true;
        }

        if (TryStripSuffix(path, "/delete", out normalizedPath))
        {
            normalizedMethod = AspNetHttpMethods.Delete;
            return true;
        }

        return false;
    }

    private static bool TryStripSuffix(string path, string suffix, out PathString normalizedPath)
    {
        normalizedPath = new PathString(path);
        if (!path.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var stripped = path[..^suffix.Length].TrimEnd('/');
        if (string.IsNullOrWhiteSpace(stripped) || string.Equals(stripped, "/api", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        normalizedPath = new PathString(stripped);
        return true;
    }

    private static bool TryResolveHeader(HttpRequest request, out string method)
    {
        method = AspNetHttpMethods.Post;
        if (!request.Headers.TryGetValue("X-HTTP-Method-Override", out var values))
        {
            return false;
        }

        var candidate = values.ToString().Trim().ToUpperInvariant();
        if (candidate is not ("PUT" or "PATCH" or "DELETE"))
        {
            return false;
        }

        method = candidate;
        return true;
    }
}

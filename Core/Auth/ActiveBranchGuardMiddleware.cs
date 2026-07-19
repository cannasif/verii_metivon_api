using System.Security.Claims;
using System.Text.Json;

namespace verii_metivon_api.Core.Auth;

/// <summary>
/// Keeps branch-scoped requests bound to the branch selected at login.
/// The UI may display the active branch, but it cannot authorize a different one.
/// </summary>
public sealed class ActiveBranchGuardMiddleware(RequestDelegate next)
{
    private static readonly string[] ExcludedPrefixes = ["/api/auth", "/api/branches", "/health", "/swagger"];

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true || IsExcluded(context.Request.Path))
        {
            await next(context);
            return;
        }

        if (!long.TryParse(context.User.FindFirstValue("branchId"), out var activeBranchId) || activeBranchId <= 0)
        {
            await RejectAsync(context, "The active branch could not be resolved from the session.");
            return;
        }

        var activeBranchCode = context.User.FindFirstValue("branchCode");
        var requestedBranchCode = context.Request.Headers["X-Branch-Code"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(activeBranchCode) && !string.IsNullOrWhiteSpace(requestedBranchCode) &&
            !string.Equals(activeBranchCode, requestedBranchCode, StringComparison.OrdinalIgnoreCase))
        {
            await RejectAsync(context, "The requested branch does not match the active session branch.");
            return;
        }

        if (TryReadBranchId(context.Request.Query["branchId"].FirstOrDefault(), out var queryBranchId) && queryBranchId != activeBranchId)
        {
            await RejectAsync(context, "The requested branch does not match the active session branch.");
            return;
        }

        if (IsJsonMutation(context.Request) && (await ReadBodyBranchIdsAsync(context.Request, context.RequestAborted)).Any(branchId => branchId != activeBranchId))
        {
            await RejectAsync(context, "The submitted branch does not match the active session branch.");
            return;
        }

        context.Items["ActiveBranchId"] = activeBranchId;
        context.Items["ActiveBranchCode"] = activeBranchCode;
        await next(context);
    }

    private static bool IsExcluded(PathString path) => ExcludedPrefixes.Any(prefix => path.StartsWithSegments(prefix));

    private static bool IsJsonMutation(HttpRequest request) =>
        Microsoft.AspNetCore.Http.HttpMethods.IsPost(request.Method) ||
        Microsoft.AspNetCore.Http.HttpMethods.IsPut(request.Method) ||
        Microsoft.AspNetCore.Http.HttpMethods.IsPatch(request.Method)
            ? request.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true
            : false;

    private static bool TryReadBranchId(string? value, out long branchId) => long.TryParse(value, out branchId) && branchId > 0;

    private static async Task<IReadOnlyList<long>> ReadBodyBranchIdsAsync(HttpRequest request, CancellationToken cancellationToken)
    {
        request.EnableBuffering();
        try
        {
            using var document = await JsonDocument.ParseAsync(request.Body, cancellationToken: cancellationToken);
            var branchIds = new List<long>();
            CollectBranchIds(document.RootElement, branchIds);
            return branchIds;
        }
        catch (JsonException)
        {
            return [];
        }
        finally
        {
            request.Body.Position = 0;
        }
    }

    private static void CollectBranchIds(JsonElement element, ICollection<long> branchIds)
    {
        if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray()) CollectBranchIds(item, branchIds);
            return;
        }
        if (element.ValueKind != JsonValueKind.Object) return;
        foreach (var property in element.EnumerateObject())
        {
            if (property.Name.Equals("branchId", StringComparison.OrdinalIgnoreCase))
            {
                if (property.Value.ValueKind == JsonValueKind.Number && property.Value.TryGetInt64(out var numeric) && numeric > 0) branchIds.Add(numeric);
                else if (property.Value.ValueKind == JsonValueKind.String && long.TryParse(property.Value.GetString(), out numeric) && numeric > 0) branchIds.Add(numeric);
            }
            CollectBranchIds(property.Value, branchIds);
        }
    }

    private static async Task RejectAsync(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(ApiResponse<object>.Error(message, StatusCodes.Status403Forbidden));
    }
}

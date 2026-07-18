using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using verii_metivon_api.Core.Auth;

namespace verii_metivon_api.Modules.AccessControl.Authorization;

/// <summary>
/// Default-deny permission bridge for ERP controllers that predate declarative
/// PermissionAuthorize attributes. New endpoints should use the attribute;
/// this filter prevents an authenticated user from bypassing the web menu by
/// calling a mapped ERP API directly.
/// </summary>
public sealed class ErpPermissionAuthorizationFilter(IAuthorizationService authorization) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() is not null) return;
        if (context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<PermissionAuthorizeAttribute>() is not null) return;
        if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;

        var actionName = (context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo.Name;
        var code = ResolvePermission(context.HttpContext.Request, actionName);
        if (code is null) return;

        var result = await authorization.AuthorizeAsync(context.HttpContext.User, null, PermissionPolicy.Name(code));
        if (!result.Succeeded)
        {
            context.Result = new ObjectResult(ApiResponse<object>.Error("You do not have permission to perform this operation.", 403, "ACCESS_PERMISSION_DENIED"))
            { StatusCode = StatusCodes.Status403Forbidden };
        }
    }

    internal static string? ResolvePermission(HttpRequest request, string? actionName = null)
    {
        var segments = request.Path.Value?.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries) ?? [];
        if (segments.Length < 2 || !segments[0].Equals("api", StringComparison.OrdinalIgnoreCase)) return null;
        var path = string.Join('/', segments.Skip(1)).ToLowerInvariant();
        var resource = ResolveResource(path);
        if (resource is null) return null;

        var method = request.Method.ToUpperInvariant();
        // Paged endpoint conventions intentionally expose former GET list actions as
        // POST + JSON. The CLR action name remains stable, so it is the authoritative
        // signal that a POST is a read and must require `.view`, not `.create`.
        var isReadAction = actionName is not null &&
            (actionName.StartsWith("Get", StringComparison.OrdinalIgnoreCase) ||
             actionName.StartsWith("List", StringComparison.OrdinalIgnoreCase) ||
             actionName.StartsWith("Search", StringComparison.OrdinalIgnoreCase) ||
             actionName.StartsWith("Query", StringComparison.OrdinalIgnoreCase) ||
             actionName.StartsWith("Preview", StringComparison.OrdinalIgnoreCase) ||
             actionName is "Accounts" or "Journals" or "Labels");
        var isQuery = path.EndsWith("/query", StringComparison.OrdinalIgnoreCase) ||
                      path.EndsWith("/paged", StringComparison.OrdinalIgnoreCase) ||
                      method == HttpMethods.Get || isReadAction;
        var action = isQuery ? "view" : method switch
        {
            "DELETE" => "delete",
            "PUT" or "PATCH" => "update",
            "POST" when IsCollectionCreate(path) => "create",
            "POST" => "update",
            _ => "view"
        };
        return $"{resource}.{action}";
    }

    private static bool IsCollectionCreate(string path) => path.Count(x => x == '/') == 0 ||
        path is "accounting/accounts" or "accounting/journals" or "pricing/price-lists";

    private static string? ResolveResource(string path)
    {
        if (path.StartsWith("business-partners")) return "accounts.account-management";
        if (path.StartsWith("products")) return "products.product-management";
        if (path.StartsWith("warehouses")) return path.Contains("locations") ? "warehouses.storage-locations" : "warehouses.warehouse-management";
        if (path.StartsWith("inventory/balances") || path.StartsWith("inventory/dashboard")) return "inventory.balances";
        if (path.StartsWith("inventory")) return "inventory.transactions";
        if (path.StartsWith("purchase-orders")) return "procurement.purchase-orders";
        if (path.StartsWith("goods-receipts")) return "receiving.goods-receipts";
        if (path.StartsWith("transfer-orders")) return "transfers.transfer-orders";
        if (path.StartsWith("sales-orders")) return "sales.sales-orders";
        if (path.StartsWith("pricing")) return "pricing.price-lists";
        if (path.StartsWith("shipments")) return "shipping.shipments";
        if (path.StartsWith("inventory-counts")) return "counting.inventory-counts";
        if (path.StartsWith("e-documents")) return "e-documents.e-documents";
        if (path.StartsWith("accounting/accounts")) return "accounting.ledger-accounts";
        if (path.StartsWith("accounting/journals")) return "accounting.journal-entries";
        if (path.StartsWith("accounting/definitions/fiscal-periods")) return "accounting.fiscal-periods";
        if (path.StartsWith("accounting/definitions/inventory-posting-profiles")) return "accounting.inventory-posting-profiles";
        if (path.StartsWith("import-dossiers")) return "landed-costs.import-dossiers";
        if (path.StartsWith("trade-dossiers")) return "trade-operations.trade-dossiers";
        if (path.StartsWith("parameters/products")) return "products.parameters";
        if (path.StartsWith("parameters/warehouses")) return "warehouses.parameters";
        if (path.StartsWith("parameters/procurement")) return "procurement.parameters";
        if (path.StartsWith("parameters/receiving")) return "receiving.parameters";
        if (path.StartsWith("parameters/transfers")) return "transfers.parameters";
        if (path.StartsWith("parameters/sales-orders")) return "sales.parameters";
        if (path.StartsWith("parameters/pricing")) return "pricing.parameters";
        if (path.StartsWith("parameters/shipping")) return "shipping.parameters";
        if (path.StartsWith("parameters/inventory-counts")) return "counting.parameters";
        if (path.StartsWith("parameters/inventory-traceability")) return "inventory.tracking-parameters";
        if (path.StartsWith("parameters/e-documents")) return "e-documents.parameters";
        if (path.StartsWith("parameters/accounting")) return "accounting.parameters";
        if (path.StartsWith("general-settings")) return "settings.general-settings";
        return null;
    }
}

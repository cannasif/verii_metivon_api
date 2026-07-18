using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.AccessControl.Domain.Entities;

namespace verii_metivon_api.Modules.AccessControl.Infrastructure.Persistence;

/// <summary>
/// Installs immutable, least-privilege ERP job profiles. Built-in profiles are
/// reconciled after permission catalog sync; customers can create custom groups
/// without modifying the secure defaults.
/// </summary>
public static class DefaultPermissionGroupSeeder
{
    private sealed record GroupPreset(
        string Code,
        string Name,
        string Description,
        int Priority,
        bool IsSystemAdmin,
        Func<PermissionDefinition, bool> Includes);

    private static readonly GroupPreset[] Presets =
    [
        new("system-administrators", "System Administrators", "Unrestricted system access. Assign only to dedicated privileged accounts.", 10, true, _ => false),
        new("master-data-specialist", "Master Data Specialist", "Maintains business partners, products, warehouses and their definitions.", 30, false,
            p => HasModule(p, "accounts", "products", "warehouses") && p.Action is "view" or "create" or "update"),
        new("purchasing-specialist", "Purchasing Specialist", "Creates and follows purchase orders and views related master data.", 40, false,
            p => (HasModule(p, "procurement") && p.Action is "view" or "create" or "update") ||
                 (HasModule(p, "accounts", "products", "warehouses", "pricing", "receiving") && p.Action == "view")),
        new("goods-receipt-operator", "Goods Receipt Operator", "Performs domestic/import goods receipt and label preparation operations.", 50, false,
            p => (HasModule(p, "receiving") && p.Action is "view" or "create" or "update") ||
                 (HasModule(p, "accounts", "products", "warehouses", "inventory", "procurement", "landed-costs", "trade-operations") && p.Action == "view")),
        new("warehouse-operator", "Warehouse Operator", "Performs warehouse, location, stock movement, transfer and count operations.", 60, false,
            p => (HasModule(p, "warehouses", "inventory", "transfers", "counting") && p.Action is "view" or "create" or "update") ||
                 (HasModule(p, "products", "receiving", "shipping") && p.Action == "view")),
        new("shipping-operator", "Shipping Operator", "Prepares shipments, delivery notes and outbound warehouse operations.", 70, false,
            p => (HasModule(p, "shipping") && p.Action is "view" or "create" or "update") ||
                 (HasModule(p, "e-documents") && p.Action is "view" or "create" or "update") ||
                 (HasModule(p, "sales", "accounts", "products", "warehouses", "inventory") && p.Action == "view")),
        new("sales-specialist", "Sales Specialist", "Manages sales orders and uses prices, customers and stock availability.", 80, false,
            p => (HasModule(p, "sales") && p.Action is "view" or "create" or "update") ||
                 (HasModule(p, "pricing") && p.Action is "view" or "create" or "update") ||
                 (HasModule(p, "accounts", "products", "warehouses", "inventory", "shipping") && p.Action == "view")),
        new("import-cost-specialist", "Import and Landed Cost Specialist", "Manages import dossiers, customs costs and landed-cost allocation.", 90, false,
            p => (HasModule(p, "landed-costs", "trade-operations") && p.Action is "view" or "create" or "update") ||
                 (HasModule(p, "procurement", "receiving", "accounts", "products", "warehouses", "inventory", "accounting") && p.Action == "view")),
        new("accounting-specialist", "Accounting Specialist", "Manages journals, ledger accounts, fiscal periods and electronic documents.", 100, false,
            p => (HasModule(p, "accounting", "e-documents") && p.Action is "view" or "create" or "update") ||
                 (HasModule(p, "accounts", "procurement", "sales", "pricing", "landed-costs", "trade-operations") && p.Action == "view")),
        new("erp-auditor", "ERP Auditor (Read Only)", "Read-only access to ERP records and access-control audit trails.", 110, false,
            p => p.Action == "view" && (IsBusinessPermission(p) || p.Code == "access-control.audit-logs.view"))
    ];

    public static async Task ApplyAsync(MetivonDbContext db, string? bootstrapAdminEmail = null, CancellationToken ct = default)
    {
        var definitions = await db.Set<PermissionDefinition>().AsNoTracking().Where(x => x.IsActive).ToListAsync(ct);
        var presetCodes = Presets.Select(x => x.Code).ToArray();
        var groups = await db.Set<PermissionGroup>().IgnoreQueryFilters()
            .Where(x => presetCodes.Contains(x.Code)).ToDictionaryAsync(x => x.Code, StringComparer.OrdinalIgnoreCase, ct);

        foreach (var preset in Presets)
        {
            if (!groups.TryGetValue(preset.Code, out var group))
            {
                group = new PermissionGroup { Code = preset.Code };
                db.Add(group);
                groups[preset.Code] = group;
            }

            group.Name = preset.Name;
            group.Description = preset.Description;
            group.Priority = preset.Priority;
            group.IsSystemAdmin = preset.IsSystemAdmin;
            group.IsSystem = true;
            group.IsActive = true;
            group.IsDeleted = false;
            group.DeletedAt = null;
            group.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(ct);

        foreach (var preset in Presets.Where(x => !x.IsSystemAdmin))
        {
            var group = groups[preset.Code];
            var expectedIds = definitions.Where(preset.Includes).Select(x => x.Id).ToHashSet();
            var existing = await db.Set<PermissionGroupPermission>().IgnoreQueryFilters()
                .Where(x => x.PermissionGroupId == group.Id).ToListAsync(ct);
            db.RemoveRange(existing.Where(x => !expectedIds.Contains(x.PermissionDefinitionId) || x.IsDenied));
            foreach (var link in existing.Where(x => !x.IsDenied && expectedIds.Contains(x.PermissionDefinitionId) && x.IsDeleted))
            {
                link.IsDeleted = false;
                link.DeletedAt = null;
            }
            var existingAllowedIds = existing.Where(x => !x.IsDenied && !x.IsDeleted && expectedIds.Contains(x.PermissionDefinitionId))
                .Select(x => x.PermissionDefinitionId).ToHashSet();
            db.AddRange(expectedIds.Except(existingAllowedIds).Select(permissionId => new PermissionGroupPermission
            {
                PermissionGroupId = group.Id,
                PermissionDefinitionId = permissionId
            }));
        }

        var adminGroup = groups["system-administrators"];
        var adminLinks = await db.Set<PermissionGroupPermission>().IgnoreQueryFilters()
            .Where(x => x.PermissionGroupId == adminGroup.Id).ToListAsync(ct);
        db.RemoveRange(adminLinks);

        if (!string.IsNullOrWhiteSpace(bootstrapAdminEmail))
        {
            var adminUser = await db.Users.FirstOrDefaultAsync(x => x.Email == bootstrapAdminEmail, ct);
            if (adminUser is not null)
            {
                var assignment = await db.Set<UserPermissionGroup>().IgnoreQueryFilters()
                    .FirstOrDefaultAsync(x => x.UserId == adminUser.Id && x.PermissionGroupId == adminGroup.Id, ct);
                if (assignment is null)
                    db.Add(new UserPermissionGroup { UserId = adminUser.Id, PermissionGroupId = adminGroup.Id });
                else
                {
                    assignment.IsDeleted = false;
                    assignment.DeletedAt = null;
                    assignment.ValidFrom = null;
                    assignment.ValidUntil = null;
                }
            }
        }

        await db.SaveChangesAsync(ct);
    }

    private static bool HasModule(PermissionDefinition permission, params string[] modules) =>
        modules.Contains(permission.Module, StringComparer.OrdinalIgnoreCase);

    private static bool IsBusinessPermission(PermissionDefinition permission) =>
        HasModule(permission, "accounts", "products", "warehouses", "inventory", "procurement", "receiving",
            "transfers", "sales", "pricing", "shipping", "counting", "e-documents", "accounting",
            "landed-costs", "trade-operations");
}

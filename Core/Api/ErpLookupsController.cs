using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.Persistence;

namespace verii_metivon_api.Core.Api;

[ApiController, Authorize, Route("api/erp-lookups")]
public sealed class ErpLookupsController(MetivonDbContext db) : ControllerBase
{
    public sealed class LookupQuery : PagedQuery
    {
        public long? ParentId { get; init; }
    }

    public sealed class LookupRow
    {
        public long Id { get; init; }
        public string Code { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
    }

    [HttpGet("{lookupKey}")]
    public async Task<IActionResult> GetPaged(
        string lookupKey,
        [FromQuery] LookupQuery request,
        CancellationToken ct = default)
    {
        var pageNumber = request.NormalizedPageNumber;
        var pageSize = request.NormalizedPageSize;
        var parentId = request.ParentId;
        var term = request.Search?.Trim();

        IQueryable<LookupRow>? query = lookupKey.ToLowerInvariant() switch
        {
            "branches" => db.Branches.Where(x => x.IsActive)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "partners" => db.BusinessPartners.Where(x => x.IsActive)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "businesspartnertypes" => db.BusinessPartnerTypes.Where(x => x.IsActive)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "customergroups" => db.CustomerGroups.Where(x => x.IsActive)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "products" => db.Products.Where(x => x.IsActive)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "productcategories" => db.ProductCategories.Where(x => x.IsActive)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "productgroups" => db.ProductGroups.Where(x => x.IsActive)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "brands" => db.Brands.Where(x => x.IsActive)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "taxgroups" => db.TaxGroups.Where(x => x.IsActive)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "units" => db.Units.Where(x => x.IsActive)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "warehouses" => db.Warehouses.Where(x => x.IsActive && (!parentId.HasValue || x.BranchId == parentId))
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "locations" => db.StorageLocations.Where(x => x.IsActive && !x.IsBlocked && (!parentId.HasValue || x.WarehouseId == parentId))
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Code }),
            "warehousetypes" => db.WarehouseTypes.Where(x => x.IsActive)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "locationtypes" => db.LocationTypes.Where(x => x.IsActive)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "warehousezones" => db.WarehouseZones.Where(x => x.IsActive && (!parentId.HasValue || x.WarehouseId == parentId))
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "inventorystatuses" => db.InventoryStatuses.Where(x => x.IsActive)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "inventorylots" => db.InventoryLots
                .Where(x => x.IsActive && !x.IsBlocked && (!parentId.HasValue || x.ProductId == parentId))
                .Select(x => new LookupRow
                {
                    Id = x.Id,
                    Code = x.LotNumber,
                    Name = x.SupplierLotNumber == null ? x.LotNumber : x.LotNumber + " / " + x.SupplierLotNumber
                }),
            "inventoryserials" => db.InventorySerials.Where(x => x.IsActive && (!parentId.HasValue || x.ProductId == parentId))
                .Join(db.InventoryBalances.Where(x=>x.AvailableQuantity>0),s=>s.Id,b=>b.InventorySerialId,(s,b)=>new LookupRow{Id=s.Id,Code=s.SerialNumber,Name=s.SerialNumber+" · "+b.AvailableQuantity}),
            "currencies" => db.Currencies.Where(x => x.IsActive)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "inventoryreceipttransactions" => db.InventoryTransactions
                .Where(x => x.Direction == Modules.Inventory.Domain.Enums.InventoryMovementDirection.Receipt
                    && (!parentId.HasValue || x.ProductId == parentId))
                .Select(x => new LookupRow
                {
                    Id = x.Id,
                    Code = x.DocumentNumber,
                    Name = x.Product.Code + " / " + x.Warehouse.Code + " / " + x.BaseQuantity
                }),
            "paymentterms" => db.PaymentTerms.Where(x => x.IsActive)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "purchaseorders" => db.PurchaseOrders
                .Where(x => x.Status != Modules.Procurement.Domain.Enums.PurchaseOrderStatus.Cancelled)
                .Select(x => new LookupRow { Id = x.Id, Code = x.OrderNumber, Name = x.OrderNumber }),
            "purchaseorderlines" => db.PurchaseOrderLines
                .Where(x => !x.IsClosed && (!parentId.HasValue || x.PurchaseOrderId == parentId))
                .Select(x => new LookupRow { Id = x.Id, Code = x.LineNumber.ToString(), Name = x.Product.Code + " · " + x.Product.Name }),
            "salesorders" => db.SalesOrders
                .Where(x => x.Status != Modules.Sales.Domain.Entities.SalesOrderStatus.Cancelled)
                .Select(x => new LookupRow { Id = x.Id, Code = x.OrderNumber, Name = x.OrderNumber }),
            "salesorderlines" => db.SalesOrderLines
                .Where(x => x.Status != Modules.Sales.Domain.Entities.SalesLineStatus.Shipped && (!parentId.HasValue || x.SalesOrderId == parentId))
                .Select(x => new LookupRow { Id = x.Id, Code = x.LineNumber.ToString(), Name = x.Product.Code + " · " + x.Product.Name }),
            "fiscalperiods" => db.FiscalPeriods.Where(x => x.IsOpen)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "ledgeraccounts" => db.LedgerAccounts.Where(x => x.IsActive && x.AllowPosting)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "landedcosttypes" => db.LandedCostTypes.Where(x => x.IsActive)
                .Select(x => new LookupRow { Id = x.Id, Code = x.Code, Name = x.Name }),
            "tradedossiers" => db.TradeDossiers.Where(x => x.Status != Modules.TradeOperations.Domain.Entities.TradeDossierStatus.Closed && x.Status != Modules.TradeOperations.Domain.Entities.TradeDossierStatus.Cancelled)
                .Select(x => new LookupRow { Id = x.Id, Code = x.DossierNumber, Name = x.BusinessPartner.Name + " · " + x.Direction }),
            _ => null
        };

        if (query is null)
            return NotFound(ApiResponse<object>.Error("Unknown lookup.", 404));

        if (!string.IsNullOrWhiteSpace(term))
            query = query.Where(x => x.Code.Contains(term) || x.Name.Contains(term));

        query = query.ApplyPagedFilters(request);
        var total = await query.CountAsync(ct);
        var rows = await query.OrderBy(x => x.Code)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        var totalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize);
        return Ok(ApiResponse<object>.Ok(new
        {
            items = rows,
            pageNumber,
            pageSize,
            totalCount = total,
            totalPages,
            hasPreviousPage = pageNumber > 1,
            hasNextPage = pageNumber < totalPages
        }));
    }

    [HttpGet]
    public async Task<IActionResult>Get(CancellationToken ct)
    {
        var result = new
        {
            branches = await db.Branches.Where(x => x.IsActive).OrderBy(x => x.Code).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync(ct),
            partners = await db.BusinessPartners.Where(x => x.IsActive).OrderBy(x => x.Code).Take(300).Select(x => new { x.Id, x.Code, x.Name, x.BusinessPartnerTypeId, x.CustomerGroupId }).ToListAsync(ct),
            products = await db.Products.Where(x => x.IsActive).OrderBy(x => x.Code).Take(500).Select(x => new { x.Id, x.Code, x.Name, x.BaseUnitId, x.TrackingType }).ToListAsync(ct),
            units = await db.Units.Where(x => x.IsActive).OrderBy(x => x.Code).Select(x => new { x.Id, x.Code, x.Name, x.Symbol }).ToListAsync(ct),
            warehouses = await db.Warehouses.Where(x => x.IsActive).OrderBy(x => x.Code).Select(x => new { x.Id, x.Code, x.Name, x.BranchId }).ToListAsync(ct),
            locations = await db.StorageLocations.Where(x => x.IsActive && !x.IsBlocked).OrderBy(x => x.Code).Select(x => new { x.Id, x.Code, Name = x.Code, x.WarehouseId, x.LocationTypeId }).ToListAsync(ct),
            warehouseTypes = await db.WarehouseTypes.Where(x => x.IsActive).OrderBy(x => x.DisplayOrder).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync(ct),
            locationTypes = await db.LocationTypes.Where(x => x.IsActive).OrderBy(x => x.DisplayOrder).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync(ct),
            warehouseZones = await db.WarehouseZones.Where(x => x.IsActive).OrderBy(x => x.Code).Select(x => new { x.Id, x.Code, x.Name, x.WarehouseId }).ToListAsync(ct),
            inventoryStatuses = await db.InventoryStatuses.Where(x => x.IsActive).OrderBy(x => x.DisplayOrder).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync(ct),
            inventorySerials = await db.InventorySerials.Where(x => x.IsActive).Join(db.InventoryBalances.Where(x=>x.AvailableQuantity>0),s=>s.Id,b=>b.InventorySerialId,(s,b)=>new {s.Id,Code=s.SerialNumber,Name=s.SerialNumber,s.ProductId,b.WarehouseId,b.StorageLocationId}).OrderBy(x=>x.Code).Take(2000).ToListAsync(ct),
            currencies = await db.Currencies.Where(x => x.IsActive).OrderBy(x => x.Code).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync(ct),
            paymentTerms = await db.PaymentTerms.Where(x => x.IsActive).OrderBy(x => x.DisplayOrder).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync(ct),
            purchaseOrders = await db.PurchaseOrders.Where(x => x.Status != Modules.Procurement.Domain.Enums.PurchaseOrderStatus.Cancelled).OrderByDescending(x => x.OrderDate).Take(200).Select(x => new { x.Id, Code = x.OrderNumber, Name = x.OrderNumber, x.SupplierId, x.WarehouseId }).ToListAsync(ct),
            purchaseOrderLines = await db.PurchaseOrderLines.Where(x => !x.IsClosed).OrderBy(x => x.PurchaseOrderId).ThenBy(x => x.LineNumber).Take(1000).Select(x => new { x.Id, Code = x.LineNumber.ToString(), Name = x.Product.Code + " · " + x.Product.Name, x.PurchaseOrderId, x.ProductId, x.UnitId }).ToListAsync(ct),
            salesOrders = await db.SalesOrders.Where(x => x.Status != Modules.Sales.Domain.Entities.SalesOrderStatus.Cancelled).OrderByDescending(x => x.OrderDate).Take(200).Select(x => new { x.Id, Code = x.OrderNumber, Name = x.OrderNumber, x.CustomerId, x.WarehouseId }).ToListAsync(ct),
            salesOrderLines = await db.SalesOrderLines.Where(x => x.Status != Modules.Sales.Domain.Entities.SalesLineStatus.Shipped).OrderBy(x => x.SalesOrderId).ThenBy(x => x.LineNumber).Take(1000).Select(x => new { x.Id, Code = x.LineNumber.ToString(), Name = x.Product.Code + " · " + x.Product.Name, x.SalesOrderId, x.ProductId, x.UnitId }).ToListAsync(ct),
            fiscalPeriods = await db.FiscalPeriods.Where(x => x.IsOpen).OrderBy(x => x.StartDate).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync(ct),
            ledgerAccounts = await db.LedgerAccounts.Where(x => x.IsActive && x.AllowPosting).OrderBy(x => x.Code).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync(ct),
            landedCostTypes = await db.LandedCostTypes.Where(x => x.IsActive).OrderBy(x => x.DisplayOrder).Select(x => new { x.Id, x.Code, x.Name, AllocationMethod = x.DefaultAllocationMethod.ToString() }).ToListAsync(ct),
            tradeDossiers = await db.TradeDossiers.Where(x => x.Status != Modules.TradeOperations.Domain.Entities.TradeDossierStatus.Closed && x.Status != Modules.TradeOperations.Domain.Entities.TradeDossierStatus.Cancelled).OrderByDescending(x => x.OpenDate).Take(300).Select(x => new { x.Id, Code=x.DossierNumber, Name=x.BusinessPartner.Name+" · "+x.Direction, x.Direction, x.BusinessPartnerId }).ToListAsync(ct)
        };
        return Ok(ApiResponse<object>.Ok(result));
    }
}

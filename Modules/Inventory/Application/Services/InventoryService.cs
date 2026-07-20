using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.UnitOfWork;
using verii_metivon_api.Modules.Accounting.Application.Parameters;
using verii_metivon_api.Modules.Accounting.Domain.Entities;
using verii_metivon_api.Modules.Inventory.Domain.Entities;
using verii_metivon_api.Modules.Inventory.Domain.Enums;
using verii_metivon_api.Modules.Products.Domain.Enums;
namespace verii_metivon_api.Modules.Inventory.Application.Services;
public sealed class InventoryService(IUnitOfWork u, IAccountingParameterService accountingParameters, verii_metivon_api.Core.Persistence.MetivonDbContext db) : IInventoryService
{
    public async Task<ApiResponse<PagedResult<InventoryTransactionRow>>> GetTransactionsAsync(InventoryTransactionQuery q, CancellationToken ct) { var x = u.Repository<InventoryTransaction>().Query().Include(v => v.Product).Include(v => v.Warehouse).Include(v => v.StorageLocation).Include(v => v.InventoryLot).Include(v => v.InventorySerial).AsQueryable(); if (q.ProductId.HasValue) x = x.Where(v => v.ProductId == q.ProductId); if (q.WarehouseId.HasValue) x = x.Where(v => v.WarehouseId == q.WarehouseId); if (q.LocationId.HasValue) x = x.Where(v => v.StorageLocationId == q.LocationId); if (q.MovementType.HasValue) x = x.Where(v => v.MovementType == q.MovementType); if (q.FromDate.HasValue) x = x.Where(v => v.PostingDate >= q.FromDate); if (q.ToDate.HasValue) x = x.Where(v => v.PostingDate <= q.ToDate); if (!string.IsNullOrWhiteSpace(q.Search)) { var s = q.Search.Trim(); x = x.Where(v => v.DocumentNumber.Contains(s) || v.Product.Code.Contains(s) || v.Product.Name.Contains(s)); } x = x.ApplyPagedFilters(q); var total = await x.CountAsync(ct); var items = await x.OrderByDescending(v => v.PostingDate).ThenByDescending(v => v.Id).Skip((q.NormalizedPageNumber - 1) * q.NormalizedPageSize).Take(q.NormalizedPageSize).Select(v => new InventoryTransactionRow(v.Id, v.PostingId, v.DocumentType, v.DocumentNumber, v.PostingDate, v.MovementType.ToString(), v.Product.Code, v.Product.Name, v.Warehouse.Code, v.StorageLocation.Code, v.InventoryLot != null ? v.InventoryLot.LotNumber : null, v.InventorySerial != null ? v.InventorySerial.SerialNumber : null, v.BaseQuantity * (int)v.Direction, v.CurrencyCode, v.UnitCost, v.TotalCost)).ToListAsync(ct); return ApiResponse<PagedResult<InventoryTransactionRow>>.Ok(new(items, q.NormalizedPageNumber, q.NormalizedPageSize, total)); }
    public async Task<ApiResponse<PagedResult<InventoryBalanceRow>>> GetBalancesAsync(InventoryBalanceQuery q, CancellationToken ct) { var x = u.Repository<InventoryBalance>().Query().Join(u.Repository<verii_metivon_api.Modules.Products.Domain.Entities.Product>().Query(), b => b.ProductId, p => p.Id, (b, p) => new { b, p }).Join(u.Repository<verii_metivon_api.Modules.Warehouses.Domain.Entities.Warehouse>().Query(), a => a.b.WarehouseId, w => w.Id, (a, w) => new { a.b, a.p, w }).Join(u.Repository<verii_metivon_api.Modules.Warehouses.Domain.Entities.StorageLocation>().Query(), a => a.b.StorageLocationId, l => l.Id, (a, l) => new { a.b, a.p, a.w, l }).Join(u.Repository<InventoryStatus>().Query(), a => a.b.InventoryStatusId, s => s.Id, (a, s) => new { a.b, a.p, a.w, a.l, s }); if (q.ProductId.HasValue) x = x.Where(v => v.b.ProductId == q.ProductId); if (q.WarehouseId.HasValue) x = x.Where(v => v.b.WarehouseId == q.WarehouseId); if (q.LocationId.HasValue) x = x.Where(v => v.b.StorageLocationId == q.LocationId); if (q.AvailableOnly) x = x.Where(v => v.s.IsAvailable && v.b.AvailableQuantity > 0); if (!string.IsNullOrWhiteSpace(q.Search)) { var z = q.Search.Trim(); x = x.Where(v => v.p.Code.Contains(z) || v.p.Name.Contains(z) || v.l.Code.Contains(z)); } x = x.ApplyPagedFilters(q); var total = await x.CountAsync(ct); var baseRows = await x.OrderBy(v => v.p.Code).ThenBy(v => v.w.Code).ThenBy(v => v.l.Code).Skip((q.NormalizedPageNumber - 1) * q.NormalizedPageSize).Take(q.NormalizedPageSize).Select(v => new { v.b, v.p, v.w, v.l, v.s }).ToListAsync(ct); var lotIds = baseRows.Where(v => v.b.InventoryLotId.HasValue).Select(v => v.b.InventoryLotId!.Value).Distinct().ToArray(); var serialIds = baseRows.Where(v => v.b.InventorySerialId.HasValue).Select(v => v.b.InventorySerialId!.Value).Distinct().ToArray(); var lots = await u.Repository<InventoryLot>().Query().Where(v => lotIds.Contains(v.Id)).ToDictionaryAsync(v => v.Id, ct); var serials = await u.Repository<InventorySerial>().Query().Where(v => serialIds.Contains(v.Id)).ToDictionaryAsync(v => v.Id, ct); var items = baseRows.Select(v => new InventoryBalanceRow(v.b.Id, v.p.Id, v.p.Code, v.p.Name, v.w.Id, v.w.Code, v.l.Id, v.l.Code, v.s.Name, v.b.InventoryLotId.HasValue && lots.TryGetValue(v.b.InventoryLotId.Value, out var lot) ? lot.LotNumber : null, v.b.InventorySerialId.HasValue && serials.TryGetValue(v.b.InventorySerialId.Value, out var serial) ? serial.SerialNumber : null, v.b.InventoryLotId.HasValue && lots.TryGetValue(v.b.InventoryLotId.Value, out var exp) ? exp.ExpiryDate : null, v.b.PhysicalQuantity, v.b.ReservedQuantity, v.s.IsAvailable ? v.b.AvailableQuantity : 0, v.b.InventoryValue)).ToList(); return ApiResponse<PagedResult<InventoryBalanceRow>>.Ok(new(items, q.NormalizedPageNumber, q.NormalizedPageSize, total)); }
    public async Task<ApiResponse<InventoryDashboardResult>> GetDashboardAsync(InventoryDashboardQuery q, CancellationToken ct)
    {
        var balances = from balance in u.Repository<InventoryBalance>().Query()
                       join warehouse in u.Repository<verii_metivon_api.Modules.Warehouses.Domain.Entities.Warehouse>().Query() on balance.WarehouseId equals warehouse.Id
                       join status in u.Repository<InventoryStatus>().Query() on balance.InventoryStatusId equals status.Id
                       select new { balance, warehouse, status };
        if (q.BranchId.HasValue) balances = balances.Where(x => x.warehouse.BranchId == q.BranchId.Value);
        if (q.WarehouseId.HasValue) balances = balances.Where(x => x.balance.WarehouseId == q.WarehouseId.Value);

        var aggregate = await balances.GroupBy(_ => 1).Select(group => new
        {
            Physical = group.Sum(x => x.balance.PhysicalQuantity),
            Reserved = group.Sum(x => x.balance.ReservedQuantity),
            Available = group.Sum(x => x.status.IsAvailable ? x.balance.AvailableQuantity : 0),
            Value = group.Sum(x => x.balance.InventoryValue),
            Products = group.Select(x => x.balance.ProductId).Distinct().Count(),
            Warehouses = group.Select(x => x.balance.WarehouseId).Distinct().Count(),
            Dimensions = group.Count(),
            Negative = group.Count(x => x.balance.PhysicalQuantity < 0)
        }).FirstOrDefaultAsync(ct);

        var warningDays = Math.Clamp(q.ExpiryWarningDays, 1, 3650);
        var warningDate = DateOnly.FromDateTime(DateTime.UtcNow.Date).AddDays(warningDays);
        var expiringLots = await (from row in balances
                                  join lot in u.Repository<InventoryLot>().Query() on row.balance.InventoryLotId equals lot.Id
                                  where row.balance.PhysicalQuantity > 0 && lot.ExpiryDate.HasValue && lot.ExpiryDate.Value <= warningDate
                                  select lot.Id).Distinct().CountAsync(ct);

        var lowStockProducts = 0;
        if (q.BranchId.HasValue)
        {
            var productAvailability = balances.GroupBy(x => x.balance.ProductId).Select(group => new
            {
                ProductId = group.Key,
                Available = group.Sum(x => x.status.IsAvailable ? x.balance.AvailableQuantity : 0)
            });
            lowStockProducts = await (from stock in productAvailability
                                      join setting in u.Repository<verii_metivon_api.Modules.Products.Domain.Entities.ProductBranchSetting>().Query()
                                          on stock.ProductId equals setting.ProductId
                                      where setting.BranchId == q.BranchId.Value && setting.ReorderPoint > 0 && stock.Available <= setting.ReorderPoint
                                      select stock.ProductId).Distinct().CountAsync(ct);
        }

        var warehouses = await balances.GroupBy(x => new { x.warehouse.Id, x.warehouse.Code, x.warehouse.Name })
            .Select(group => new InventoryDashboardWarehouse(group.Key.Id, group.Key.Code, group.Key.Name,
                group.Sum(x => x.balance.PhysicalQuantity),
                group.Sum(x => x.status.IsAvailable ? x.balance.AvailableQuantity : 0),
                group.Sum(x => x.balance.InventoryValue)))
            .OrderByDescending(x => x.InventoryValue).ThenBy(x => x.WarehouseCode).Take(8).ToListAsync(ct);

        var statuses = await balances.GroupBy(x => new { x.status.Code, x.status.Name })
            .Select(group => new InventoryDashboardStatus(group.Key.Code, group.Key.Name,
                group.Sum(x => x.balance.PhysicalQuantity),
                group.Sum(x => x.status.IsAvailable ? x.balance.AvailableQuantity : 0),
                group.Sum(x => x.balance.InventoryValue)))
            .OrderByDescending(x => x.PhysicalQuantity).Take(8).ToListAsync(ct);

        var movements = from movement in u.Repository<InventoryTransaction>().Query()
                        join warehouse in u.Repository<verii_metivon_api.Modules.Warehouses.Domain.Entities.Warehouse>().Query() on movement.WarehouseId equals warehouse.Id
                        join product in u.Repository<verii_metivon_api.Modules.Products.Domain.Entities.Product>().Query() on movement.ProductId equals product.Id
                        select new { movement, warehouse, product };
        if (q.BranchId.HasValue) movements = movements.Where(x => x.warehouse.BranchId == q.BranchId.Value);
        if (q.WarehouseId.HasValue) movements = movements.Where(x => x.movement.WarehouseId == q.WarehouseId.Value);
        var recentMovements = await movements.OrderByDescending(x => x.movement.PostingDate).ThenByDescending(x => x.movement.Id).Take(8)
            .Select(x => new InventoryDashboardMovement(x.movement.Id, x.movement.DocumentType, x.movement.DocumentNumber,
                x.movement.PostingDate, x.movement.MovementType.ToString(), x.product.Code, x.product.Name,
                x.warehouse.Code, x.movement.BaseQuantity * (int)x.movement.Direction, x.movement.TotalCost))
            .ToListAsync(ct);

        var summary = new InventoryDashboardSummary(aggregate?.Physical ?? 0, aggregate?.Reserved ?? 0,
            aggregate?.Available ?? 0, aggregate?.Value ?? 0, aggregate?.Products ?? 0, aggregate?.Warehouses ?? 0,
            aggregate?.Dimensions ?? 0, aggregate?.Negative ?? 0, lowStockProducts, expiringLots);
        return ApiResponse<InventoryDashboardResult>.Ok(new(summary, warehouses, statuses, recentMovements, DateTime.UtcNow));
    }
    public async Task<ApiResponse<PostInventoryResult>> PostAsync(PostInventoryRequest r, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(r.IdempotencyKey) || r.Lines.Count == 0
            || r.DocumentId <= 0 || string.IsNullOrWhiteSpace(r.DocumentType) || string.IsNullOrWhiteSpace(r.DocumentNumber)
            || !Enum.IsDefined(r.MovementType) || !Enum.IsDefined(r.Direction) || string.IsNullOrWhiteSpace(r.CurrencyCode))
            return ApiResponse<PostInventoryResult>.Error("Posting key and lines are required.", 400);

        return await u.ExecuteInTransactionAsync(async token =>
        {
            await AcquirePostingLockAsync($"POST:{r.IdempotencyKey}", token);
            var existing = await u.Repository<InventoryTransaction>().Query()
                .Where(x => x.IdempotencyKey.StartsWith(r.IdempotencyKey + ":"))
                .OrderBy(x => x.Id).ToListAsync(token);
            if (existing.Count > 0)
                return ApiResponse<PostInventoryResult>.Ok(
                    new(existing[0].PostingId, existing.Select(x => x.Id).ToList(), true), "Already posted.");

            var currencyCode = r.CurrencyCode.Trim().ToUpperInvariant();
            if (!await db.Currencies.AnyAsync(x => x.IsActive && (x.Code == currencyCode || x.IsoCode == currencyCode), token))
                throw new InvalidOperationException("Inventory posting currency not found or inactive.");
            var postingId = Guid.NewGuid();
            var transactionIds = new List<long>();
            for (var index = 0; index < r.Lines.Count; index++)
            {
                var line = r.Lines[index];
                if (line.Quantity <= 0) throw new InvalidOperationException("Quantity must be positive.");

                var product = await db.Products.FirstOrDefaultAsync(x => x.Id == line.ProductId && x.IsActive, token)
                    ?? throw new InvalidOperationException("Product not found or inactive.");
                if (!await db.Units.AnyAsync(x => x.Id == line.UnitId && x.IsActive, token))
                    throw new InvalidOperationException("Inventory unit not found or inactive.");
                var warehouse = await db.Warehouses.FirstOrDefaultAsync(x => x.Id == line.WarehouseId && x.IsActive, token)
                    ?? throw new InvalidOperationException("Warehouse not found or inactive.");
                var locationIsValid = await db.StorageLocations.AnyAsync(x =>
                    x.Id == line.StorageLocationId && x.WarehouseId == line.WarehouseId && x.IsActive && !x.IsBlocked, token);
                if (!locationIsValid) throw new InvalidOperationException("Storage location must be active, unblocked and belong to the posting warehouse.");
                var inventoryStatus = await db.InventoryStatuses.FirstOrDefaultAsync(x => x.Id == line.InventoryStatusId && x.IsActive, token)
                    ?? throw new InvalidOperationException("Inventory status not found or inactive.");
                if (line.InventoryLotId.HasValue && !await db.InventoryLots.AnyAsync(x =>
                        x.Id == line.InventoryLotId.Value && x.ProductId == line.ProductId && x.IsActive && !x.IsBlocked, token))
                    throw new InvalidOperationException("Inventory lot must be active, unblocked and belong to the posting product.");
                if (line.InventorySerialId.HasValue && !await db.InventorySerials.AnyAsync(x =>
                        x.Id == line.InventorySerialId.Value && x.ProductId == line.ProductId && x.IsActive, token))
                    throw new InvalidOperationException("Inventory serial must be active and belong to the posting product.");
                if (product.TrackingType == InventoryTrackingType.Lot && !line.InventoryLotId.HasValue)
                    throw new InvalidOperationException("Lot is required.");
                if (product.TrackingType == InventoryTrackingType.Serial && (!line.InventorySerialId.HasValue || line.Quantity != 1))
                    throw new InvalidOperationException("Serial tracked quantity must be one.");

                var dimensionKey = $"BAL:{line.ProductId}:{line.WarehouseId}:{line.StorageLocationId}:{line.InventoryStatusId}:{line.InventoryLotId?.ToString() ?? "-"}:{line.InventorySerialId?.ToString() ?? "-"}";
                await AcquirePostingLockAsync(dimensionKey, token);
                var balance = await u.Repository<InventoryBalance>().FirstOrDefaultAsync(x =>
                    x.ProductId == line.ProductId && x.WarehouseId == line.WarehouseId
                    && x.StorageLocationId == line.StorageLocationId && x.InventoryStatusId == line.InventoryStatusId
                    && x.InventoryLotId == line.InventoryLotId && x.InventorySerialId == line.InventorySerialId, true, token);
                if (balance is null)
                {
                    balance = new InventoryBalance
                    {
                        ProductId = line.ProductId, WarehouseId = line.WarehouseId,
                        StorageLocationId = line.StorageLocationId, InventoryStatusId = line.InventoryStatusId,
                        InventoryLotId = line.InventoryLotId, InventorySerialId = line.InventorySerialId
                    };
                    await u.Repository<InventoryBalance>().AddAsync(balance, token);
                }

                var signedQuantity = line.Quantity * (int)r.Direction;
                var nextPhysicalQuantity = balance.PhysicalQuantity + signedQuantity;
                if (nextPhysicalQuantity < 0 && !warehouse.AllowNegativeStock)
                    throw new InvalidOperationException("Insufficient inventory.");
                var settings = await accountingParameters.ResolveSettingsAsync(warehouse.BranchId, line.WarehouseId, token);
                if (settings.RequireInventoryPostingProfile && !await u.Repository<InventoryPostingProfile>().ExistsAsync(x =>
                        x.IsActive && x.MovementType == r.MovementType
                        && (x.WarehouseId == null || x.WarehouseId == line.WarehouseId)
                        && (x.ProductGroupId == null || x.ProductGroupId == product.ProductGroupId), token))
                    throw new InvalidOperationException("An active inventory posting profile is required by accounting parameters.");

                var unitCost = Math.Round(line.UnitCost ??
                    (balance.PhysicalQuantity > 0 ? balance.InventoryValue / balance.PhysicalQuantity : 0),
                    settings.CostDecimalPlaces, MidpointRounding.AwayFromZero);
                var nextInventoryValue = balance.InventoryValue + signedQuantity * unitCost;
                if (nextInventoryValue < 0 && !settings.AllowNegativeInventoryValue)
                    throw new InvalidOperationException("Negative inventory value is disabled by accounting parameters.");

                balance.PhysicalQuantity = nextPhysicalQuantity;
                balance.AvailableQuantity = inventoryStatus.IsAvailable
                    ? nextPhysicalQuantity - balance.ReservedQuantity
                    : 0;
                balance.InventoryValue = nextInventoryValue;

                var transaction = new InventoryTransaction
                {
                    PostingId = postingId, IdempotencyKey = $"{r.IdempotencyKey}:{index}",
                    DocumentType = r.DocumentType, DocumentId = r.DocumentId, DocumentLineId = line.DocumentLineId,
                    DocumentNumber = r.DocumentNumber, PostingDate = r.PostingDate, MovementType = r.MovementType,
                    Direction = r.Direction, ProductId = line.ProductId, UnitId = line.UnitId,
                    WarehouseId = line.WarehouseId, StorageLocationId = line.StorageLocationId,
                    InventoryStatusId = line.InventoryStatusId, InventoryLotId = line.InventoryLotId,
                    InventorySerialId = line.InventorySerialId, Quantity = line.Quantity, BaseQuantity = line.Quantity,
                    UnitCost = unitCost, TotalCost = Math.Round(line.Quantity * unitCost, settings.CostDecimalPlaces, MidpointRounding.AwayFromZero),
                    CurrencyCode = currencyCode, Explanation = line.Explanation
                };
                await u.Repository<InventoryTransaction>().AddAsync(transaction, token);
                await u.SaveChangesAsync(token);
                transactionIds.Add(transaction.Id);

                if (r.Direction == InventoryMovementDirection.Receipt && settings.CreateCostLayersOnReceipt)
                {
                    await u.Repository<InventoryCostLayer>().AddAsync(new InventoryCostLayer
                    {
                        ProductId = line.ProductId, WarehouseId = line.WarehouseId,
                        ReceiptTransactionId = transaction.Id, ReceiptDate = r.PostingDate,
                        OriginalQuantity = line.Quantity, RemainingQuantity = line.Quantity,
                        UnitCost = unitCost, CurrencyCode = currencyCode
                    }, token);
                    await u.SaveChangesAsync(token);
                }
            }

            return ApiResponse<PostInventoryResult>.Ok(new(postingId, transactionIds), "Inventory posted.");
        }, ct);
    }

    private async Task AcquirePostingLockAsync(string resource, CancellationToken ct)
    {
        var connection = db.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open) await connection.OpenAsync(ct);
        await using var command = connection.CreateCommand();
        command.Transaction = db.Database.CurrentTransaction?.GetDbTransaction();
        command.CommandText = "DECLARE @result int; EXEC @result = sys.sp_getapplock @Resource = @resource, @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = 15000; SELECT @result;";
        var parameter = command.CreateParameter();
        parameter.ParameterName = "@resource";
        parameter.Value = "METIVON:INVENTORY:" + resource;
        command.Parameters.Add(parameter);
        var result = Convert.ToInt32(await command.ExecuteScalarAsync(ct));
        if (result < 0) throw new TimeoutException("Inventory posting lock could not be acquired.");
    }
}



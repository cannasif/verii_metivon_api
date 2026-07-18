using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Domain;
using verii_metivon_api.Core.Persistence;

namespace verii_metivon_api.Core.Repositories;

public sealed class GenericRepository<T>(MetivonDbContext context, IHttpContextAccessor httpContextAccessor)
    : IGenericRepository<T> where T : Entity
{
    private readonly DbSet<T> _dbSet = context.Set<T>();

    private long? CurrentUserId => long.TryParse(
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    public IQueryable<T> Query(bool tracking = false, bool ignoreQueryFilters = false)
    {
        IQueryable<T> query = _dbSet;
        if (ignoreQueryFilters) query = query.IgnoreQueryFilters();
        return tracking ? query : query.AsNoTracking();
    }

    public Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        Query().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<T?> GetByIdForUpdateAsync(long id, CancellationToken cancellationToken = default) =>
        _dbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await Query().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        await Query().Where(predicate).ToListAsync(cancellationToken);

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool tracking = false, CancellationToken cancellationToken = default) =>
        Query(tracking).FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.CreatedBy = CurrentUserId;
        entity.IsDeleted = false;
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities) { entity.CreatedAt = DateTime.UtcNow; entity.CreatedBy = CurrentUserId; entity.IsDeleted = false; }
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public T Update(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = CurrentUserId;
        _dbSet.Update(entity);
        return entity;
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        foreach (var entity in entities) { entity.UpdatedAt = DateTime.UtcNow; entity.UpdatedBy = CurrentUserId; }
        _dbSet.UpdateRange(entities);
    }

    public async Task<bool> SoftDeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdForUpdateAsync(id, cancellationToken);
        if (entity is null) return false;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = CurrentUserId;
        return true;
    }

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        _dbSet.AnyAsync(predicate, cancellationToken);

    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default) =>
        predicate is null ? _dbSet.CountAsync(cancellationToken) : _dbSet.CountAsync(predicate, cancellationToken);

    public async Task<IReadOnlyList<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = Query();
        if (predicate is not null) query = query.Where(predicate);
        return await query.Skip((Math.Max(1, pageNumber) - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
    }
}

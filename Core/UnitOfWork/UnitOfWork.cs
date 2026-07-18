using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.Storage;
using verii_metivon_api.Core.Domain;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Core.Repositories;

namespace verii_metivon_api.Core.UnitOfWork;

public sealed class UnitOfWork(MetivonDbContext context, IHttpContextAccessor httpContextAccessor) : IUnitOfWork
{
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;

    public IGenericRepository<User> Users => Repository<User>();
    public IGenericRepository<UserDetail> UserDetails => Repository<UserDetail>();
    public IGenericRepository<Branch> Branches => Repository<Branch>();
    public IGenericRepository<BusinessPartner> BusinessPartners => Repository<BusinessPartner>();
    public IGenericRepository<BusinessPartnerType> BusinessPartnerTypes => Repository<BusinessPartnerType>();
    public IGenericRepository<CustomerGroup> CustomerGroups => Repository<CustomerGroup>();
    public IGenericRepository<PaymentTerm> PaymentTerms => Repository<PaymentTerm>();
    public IGenericRepository<Currency> Currencies => Repository<Currency>();
    public IGenericRepository<TaxGroup> TaxGroups => Repository<TaxGroup>();

    public IGenericRepository<T> Repository<T>() where T : Entity =>
        (IGenericRepository<T>)_repositories.GetOrAdd(typeof(T), _ => new GenericRepository<T>(context, httpContextAccessor));

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try { return await context.SaveChangesAsync(cancellationToken); }
        catch { await RollbackTransactionAsync(cancellationToken); throw; }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null) throw new InvalidOperationException("A transaction is already in progress.");
        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null) throw new InvalidOperationException("There is no active transaction.");
        try { await _transaction.CommitAsync(cancellationToken); }
        catch { await RollbackTransactionAsync(cancellationToken); throw; }
        finally { if (_transaction is not null) { await _transaction.DisposeAsync(); _transaction = null; } }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null) return;
        try { await _transaction.RollbackAsync(cancellationToken); }
        finally { await _transaction.DisposeAsync(); _transaction = null; }
    }

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)
    {
        if (_transaction is not null) { await operation(cancellationToken); return; }
        await BeginTransactionAsync(cancellationToken);
        try { await operation(cancellationToken); await CommitTransactionAsync(cancellationToken); }
        catch { await RollbackTransactionAsync(cancellationToken); throw; }
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
    {
        if (_transaction is not null) return await operation(cancellationToken);
        await BeginTransactionAsync(cancellationToken);
        try { var result = await operation(cancellationToken); await CommitTransactionAsync(cancellationToken); return result; }
        catch { await RollbackTransactionAsync(cancellationToken); throw; }
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null) { await _transaction.DisposeAsync(); _transaction = null; }
    }
}

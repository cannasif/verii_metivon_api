using verii_metivon_api.Core.Domain;
using verii_metivon_api.Core.Repositories;

namespace verii_metivon_api.Core.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IGenericRepository<User> Users { get; }
    IGenericRepository<UserDetail> UserDetails { get; }
    IGenericRepository<Branch> Branches { get; }
    IGenericRepository<BusinessPartner> BusinessPartners { get; }
    IGenericRepository<BusinessPartnerType> BusinessPartnerTypes { get; }
    IGenericRepository<CustomerGroup> CustomerGroups { get; }
    IGenericRepository<PaymentTerm> PaymentTerms { get; }
    IGenericRepository<Currency> Currencies { get; }
    IGenericRepository<TaxGroup> TaxGroups { get; }
    IGenericRepository<T> Repository<T>() where T : Entity;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default);
    Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default);
}

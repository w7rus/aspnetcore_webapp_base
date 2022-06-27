using System.Threading;
using System.Threading.Tasks;

namespace DAL.Data;

public interface IAppDbContextAction
{
    void Commit();
    Task CommitAsync();
    Task CommitAsync(CancellationToken cancellationToken);
    void BeginTransaction();
    Task BeginTransactionAsync();
    void CommitTransaction();
    Task CommitTransactionAsync();
    void RollbackTransaction();
    Task RollbackTransactionAsync();
}

public class AppDbContextAction : IAppDbContextAction
{
    private readonly AppDbContext _appDbContext;

    public AppDbContextAction(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public void Commit()
    {
        _appDbContext.SaveChanges();
    }

    public async Task CommitAsync()
    {
        await _appDbContext.SaveChangesAsync();
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await _appDbContext.SaveChangesAsync(cancellationToken);
    }

    public void BeginTransaction()
    {
        _appDbContext.Database.BeginTransaction();
    }

    public async Task BeginTransactionAsync()
    {
        await _appDbContext.Database.BeginTransactionAsync();
    }

    public void CommitTransaction()
    {
        _appDbContext.Database.CommitTransaction();
    }

    public async Task CommitTransactionAsync()
    {
        await _appDbContext.Database.CommitTransactionAsync();
    }

    public void RollbackTransaction()
    {
        _appDbContext.Database.RollbackTransaction();
    }

    public async Task RollbackTransactionAsync()
    {
        await _appDbContext.Database.RollbackTransactionAsync();
    }
}
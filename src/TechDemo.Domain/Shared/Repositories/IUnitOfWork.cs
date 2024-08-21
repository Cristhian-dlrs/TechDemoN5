using TechDemo.Domain.Permissions.Models;

namespace TechDemo.Domain.Shared.Repositories;

public interface IUnitOfWork : IDisposable
{
    public IPermissionsRepository PermissionsRepository { get; }
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}
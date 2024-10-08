using Newtonsoft.Json;
using TechDemo.Domain.Permissions.Models;
using TechDemo.Domain.Shared.Models;
using TechDemo.Domain.Shared.Repositories;
using TechDemo.Domain.Shared.Results;

namespace TechDemo.Infrastructure.EntityFramework.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext
            ?? throw new ArgumentNullException(nameof(dbContext));

        PermissionsRepository = new PermissionsRepository(dbContext);
    }

    public IPermissionsRepository PermissionsRepository { get; private set; }

    public async Task<Result<None>> SaveChangesAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var deferredEvents = MapDomainEventsToDeferredEvents();
        _dbContext.DeferredEvents.AddRange(deferredEvents);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private List<DeferredEvent> MapDomainEventsToDeferredEvents()
    => _dbContext.ChangeTracker
        .Entries<AggregateRoot>()
        .Select(entry => entry.Entity)
        .Where(entity => entity.DomainEvents.Any())
        .SelectMany(aggregateRoot =>
        {
            var domainEvents = aggregateRoot.DomainEvents;
            aggregateRoot.FlushDomainEvents();
            return domainEvents;
        })
        .Select(domainEvent => new DeferredEvent
        {
            Id = Guid.NewGuid(),
            Payload = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    }),
            RegisteredOn = DateTime.UtcNow,
            ProcessedOn = null
        }).ToList();

    public void Dispose() => _dbContext.Dispose();
}

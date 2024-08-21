namespace TechDemo.Domain.Shared.Models;

public interface IDomainEvent
{
    public string Operation { get; }
    public int EntityId { get; set; }
}

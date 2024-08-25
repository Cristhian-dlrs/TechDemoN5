using Microsoft.Extensions.DependencyInjection;
using TechDemo.Infrastructure.ElasticSearch;
using TechDemo.Infrastructure.Kafka;

namespace TechDemo.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddSingleton<TaskCompletionSource<bool>>();
        services.AddHostedService<InitialConfigurationService>();
        services.AddKafkaWorkers();
        services.AddElasticSearch();
        services.AddEntityFramework();
        return services;
    }
}
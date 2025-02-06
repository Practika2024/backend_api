using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureInfrastructure
{
    public static void AddInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddPersistence(builder);
    }
}
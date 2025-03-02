using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using DataAccessLayer.Data;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace DataAccessLayer;

public static class ConfigurePersistence
{
    public static void AddPersistence(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var dataSourceBuild = new NpgsqlDataSourceBuilder(
            // builder.Configuration.GetConnectionString("Default")
            builder.Configuration.GetConnectionString("PostgresDocker")
            );
        dataSourceBuild.EnableDynamicJson();
        var dataSource = dataSourceBuild.Build();

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(
                    dataSource,
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .UseSnakeCaseNamingConvention()
                .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning)));

        services.AddScoped<ApplicationDbContextInitializer>();
        services.AddRepositories();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<UserRepository>();
        services.AddScoped<IUserRepository>(provider => provider.GetRequiredService<UserRepository>());
        services.AddScoped<IUserQueries>(provider => provider.GetRequiredService<UserRepository>());
        
        services.AddScoped<ContainerHistoryRepository>();
        services.AddScoped<IContainerHistoryRepository>(provider => provider.GetRequiredService<ContainerHistoryRepository>());
        services.AddScoped<IContainerHistoryQueries>(provider => provider.GetRequiredService<ContainerHistoryRepository>());
        
        services.AddScoped<ContainerRepository>();
        services.AddScoped<IContainerRepository>(provider => provider.GetRequiredService<ContainerRepository>());
        services.AddScoped<IContainerQueries>(provider => provider.GetRequiredService<ContainerRepository>());
        
        services.AddScoped<ContainerTypeRepository>();
        services.AddScoped<IContainerTypeRepository>(provider => provider.GetRequiredService<ContainerTypeRepository>());
        services.AddScoped<IContainerTypeQueries>(provider => provider.GetRequiredService<ContainerTypeRepository>());
        
        services.AddScoped<ProductTypeRepository>();
        services.AddScoped<IProductTypeRepository>(provider => provider.GetRequiredService<ProductTypeRepository>());
        services.AddScoped<IProductTypeQueries>(provider => provider.GetRequiredService<ProductTypeRepository>());
        
        services.AddScoped<ProductRepository>();
        services.AddScoped<IProductRepository>(provider => provider.GetRequiredService<ProductRepository>());
        services.AddScoped<IProductQueries>(provider => provider.GetRequiredService<ProductRepository>());
        
        services.AddScoped<ReminderRepository>();
        services.AddScoped<IReminderRepository>(provider => provider.GetRequiredService<ReminderRepository>());
        services.AddScoped<IReminderQueries>(provider => provider.GetRequiredService<ReminderRepository>());
        
        services.AddScoped<RoleRepository>();
        services.AddScoped<IRoleQueries>(provider => provider.GetRequiredService<RoleRepository>());

        services.AddScoped<RefreshTokenRepository>();
        services.AddScoped<IRefreshTokenRepository>(provider => provider.GetRequiredService<RefreshTokenRepository>());
    }
}
using System.Text;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Services.AuthenticationServices.RefreshTokenService;
using Application.Services.AuthenticationServices.SignInService;
using Application.Services.AuthenticationServices.SignUpService;
using Application.Services.ContainerServices.AddContainerService;
using Application.Services.ContainerServices.AddReminderToContainerService;
using Application.Services.ContainerServices.ClearProductService;
using Application.Services.ContainerServices.DeleteContainerService;
using Application.Services.ContainerServices.SetCurrentProductService;
using Application.Services.ContainerServices.UpdateContainerService;
using Application.Services.HashPasswordService;
using Application.Services.ImageService;
using Application.Services.TokenService;
using Application.Services.UserServices.ChangeRolesService;
using Application.Services.UserServices.DeleteUserService;
using Application.Services.UserServices.UpdateUserService;
using Application.Services.UserServices.UploadUserImageService;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;

namespace Infrastructure.Persistence;

public static class ConfigurePersistence
{
    public static void AddPersistence(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var dataSourceBuild = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("Default"));
        dataSourceBuild.EnableDynamicJson();
        var dataSource = dataSourceBuild.Build();

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(
                    dataSource,
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .UseSnakeCaseNamingConvention()
                .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning)));

        services.AddScoped<ApplicationDbContextInitialiser>();
        services.AddRepositories();
        services.AddJwtTokenAuth(builder);
        services.AddSwaggerAuth();
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
        
        services.AddScoped<ProductRepository>();
        services.AddScoped<IProductRepository>(provider => provider.GetRequiredService<ProductRepository>());
        services.AddScoped<IProductQueries>(provider => provider.GetRequiredService<ProductRepository>());
        
        services.AddScoped<ReminderRepository>();
        services.AddScoped<IReminderRepository>(provider => provider.GetRequiredService<ReminderRepository>());
        services.AddScoped<IReminderQueries>(provider => provider.GetRequiredService<ReminderRepository>());

     

        services.AddScoped<RoleRepository>();
        services.AddScoped<IRoleQueries>(provider => provider.GetRequiredService<RoleRepository>());

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IHashPasswordService, HashPasswordService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IChangeRolesService, ChangeRolesService>();
        services.AddScoped<IDeleteUserService, DeleteUserService>();
        services.AddScoped<IUpdateUserService, UpdateUserService>();
        services.AddScoped<IUploadUserImageService, UploadUserImageService>();
        
        
        services.AddScoped<ISignUpService, SignUpService>();
        services.AddScoped<ISignInService, SignInService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IAddContainerService, AddContainerService>();
        services.AddScoped<IClearProductService, ClearProductService>();
        services.AddScoped<IDeleteContainerService, DeleteContainerService>();
        services.AddScoped<IAddReminderToContainerService, AddReminderToContainerService>();
        services.AddScoped<ISetCurrentProductService, SetCurrentProductService>();
        services.AddScoped<IUpdateContainerService, UpdateContainerService>();
        services.AddScoped<IUpdateContainerService, UpdateContainerService>();

        services.AddScoped<RefreshTokenRepository>();
        services.AddScoped<IRefreshTokenRepository>(provider => provider.GetRequiredService<RefreshTokenRepository>());
    }

    private static void AddJwtTokenAuth(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder!.Configuration["AuthSettings:key"]!)),
                    ValidIssuer = builder.Configuration["AuthSettings:issuer"],
                    ValidAudience = builder.Configuration["AuthSettings:audience"]
                };
            });
    }

    private static void AddSwaggerAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "NPR321", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Введіть JWT токен"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}
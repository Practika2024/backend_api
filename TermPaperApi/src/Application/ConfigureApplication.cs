using System.Reflection;
using System.Text;
using Application.Common.Behaviours;
using Application.Services.HashPasswordService;
using Application.Services.ImageService;
using Application.Services.TokenService;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Application;

public static class ConfigureApplication
{
    public static void AddApplication(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        services.AddServices();
        services.AddJwtTokenAuth(builder);
        services.AddSwaggerAuth();
        services.AddFluentEmailConfirmation(builder);
    }

    private static void AddFluentEmailConfirmation(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var smtpServer = builder.Configuration["MailSettings:SMTP"];
        var smtpPort = int.Parse(builder.Configuration["MailSettings:Port"]);
        var email = builder.Configuration["MailSettings:Email"];
        var password = builder.Configuration["MailSettings:Password"];

        services.AddFluentEmail(email)
            .AddSmtpSender(new System.Net.Mail.SmtpClient
            {
                Host = smtpServer,
                Port = smtpPort,
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(email, password),
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network
            });
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IHashPasswordService, HashPasswordService>();
        services.AddScoped<IImageService, ImageService>();
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
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "softstream", Version = "v1" });

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
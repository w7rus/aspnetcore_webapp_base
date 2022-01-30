using System;
using System.IO;
using API.AuthHandlers;
using API.Configuration;
using BLL.BackgroundServices;
using BLL.Handlers;
using BLL.Services;
using BLL.Services.Advanced;
using Common.Options;
using DAL.Data;
using DAL.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IJsonWebTokenRepository, JsonWebTokenRepository>();
        serviceCollection.AddScoped<IPermissionRepository, PermissionRepository>();
        serviceCollection.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        serviceCollection.AddScoped<IUserGroupPermissionValueRepository, UserGroupPermissionValueRepository>();
        serviceCollection.AddScoped<IUserGroupRepository, UserGroupRepository>();
        serviceCollection.AddScoped<IUserProfileRepository, UserProfileRepository>();
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IUserToGroupMappingRepository, UserToGroupMappingRepository>();

        return serviceCollection;
    }

    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUserToGroupService, UserToGroupService>();
        serviceCollection.AddScoped<IJsonWebTokenService, JsonWebTokenService>();
        serviceCollection.AddScoped<IPermissionService, PermissionService>();
        serviceCollection.AddScoped<IPermissionToPermissionValueService, PermissionToPermissionValueService>();
        serviceCollection.AddScoped<IRefreshTokenService, RefreshTokenService>();
        serviceCollection.AddScoped<IUserGroupPermissionValueService, UserGroupPermissionValueService>();
        serviceCollection.AddScoped<IUserGroupService, UserGroupService>();
        serviceCollection.AddScoped<IUserProfileService, UserProfileService>();
        serviceCollection.AddScoped<IUserService, UserService>();

        return serviceCollection;
    }

    public static IServiceCollection AddHandlers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAuthHandler, AuthHandler>();

        return serviceCollection;
    }

    public static IServiceCollection AddBackgroundServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<ConsumeScopedServiceHostedService>();
        serviceCollection.AddScoped<IScopedProcessingService, JsonWebTokenBackgroundService>();

        return serviceCollection;
    }

    public static IServiceCollection AddCustomDbContext(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        serviceCollection.AddScoped<AppDbContext>();
        serviceCollection.AddScoped<IAppDbContextAction, AppDbContextAction>();

        serviceCollection.AddDbContext<AppDbContext>(options =>
        {
            options
                .UseNpgsql(configuration.GetConnectionString("Default"),
                    _ => _.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery))
                .UseLazyLoadingProxies();
        });

        return serviceCollection;
    }

    public static IServiceCollection AddDbContextTest(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        serviceCollection.AddScoped<AppDbContext>();
        serviceCollection.AddScoped<IAppDbContextAction, AppDbContextAction>();

        serviceCollection.AddDbContext<AppDbContext>(options =>
        {
            options
                .UseNpgsql(configuration.GetConnectionString("Test"),
                    _ => _.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery))
                .UseLazyLoadingProxies();
        });

        return serviceCollection;
    }

    public static IServiceCollection AddCustomConfigureOptions(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IConfigureOptions<AuthenticationOptions>, ConfigureAuthenticationOptions>();
        serviceCollection.AddSingleton<IConfigureOptions<JsonWebTokenAuthenticationSchemeOptions>, ConfigureJwtBearerOptions>();

        return serviceCollection;
    }

    public static IServiceCollection AddCustomOptions(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        serviceCollection.AddOptions();
        serviceCollection.Configure<GoogleReCaptchaV2Options>(
            configuration.GetSection(nameof(GoogleReCaptchaV2Options)));
        serviceCollection.Configure<JsonWebTokenOptions>(configuration.GetSection(nameof(JsonWebTokenOptions)));
        serviceCollection.Configure<RefreshTokenOptions>(configuration.GetSection(nameof(RefreshTokenOptions)));
        serviceCollection.Configure<BackgroundServicesOptions>(
            configuration.GetSection(nameof(BackgroundServicesOptions)));

        return serviceCollection;
    }

    public static IServiceCollection AddCustomLogging(
        this IServiceCollection serviceCollection,
        Serilog.ILogger logger,
        IWebHostEnvironment env
    )
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Logger(_ =>
            {
                _.MinimumLevel.Error()
                    .WriteTo.File(
                        Path.Combine(Directory.GetCurrentDirectory(), "Logs",
                            $"log_error_{DateTime.UtcNow:yyyy_mm_dd}.log"),
                        LogEventLevel.Error, rollingInterval: RollingInterval.Day);
            });


        if (env.IsDevelopment())
        {
            loggerConfiguration
                .WriteTo.Logger(_ =>
                {
                    _.MinimumLevel.Information()
                        .WriteTo.Console()
                        .WriteTo.File(
                            Path.Combine(Directory.GetCurrentDirectory(), "Logs",
                                $"log_debug_{DateTime.UtcNow:yyyy_mm_dd}.log"),
                            rollingInterval: RollingInterval.Day);
                });
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        serviceCollection.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

        serviceCollection.AddSingleton(Log.Logger);

        return serviceCollection;
    }
}
using System;
using API.AuthHandlers;
using API.Configuration;
using BLL.BackgroundServices;
using BLL.Handlers;
using BLL.Jobs;
using BLL.Services.Advanced;
using BLL.Services.Entity;
using Common.Options;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IFileRepository<File>, FileRepository<File>>();
        serviceCollection.AddScoped<IJsonWebTokenRepository, JsonWebTokenRepository>();
        serviceCollection.AddScoped<IPermissionRepository, PermissionRepository>();
        serviceCollection.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        serviceCollection.AddScoped<IPermissionValueRepository, PermissionValueRepository>();
        serviceCollection.AddScoped<IUserGroupRepository, UserGroupRepository>();
        serviceCollection.AddScoped<IUserProfileRepository, UserProfileRepository>();
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IUserToUserGroupMappingRepository, UserToUserGroupMappingRepository>();
        serviceCollection.AddScoped<IAuthorizeRepository, AuthorizeRepository>();
        serviceCollection.AddScoped<IUserGroupInviteRequestRepository, UserGroupInviteRequestRepository>();
        serviceCollection.AddScoped<IUserGroupTransferRequestRepository, UserGroupTransferRequestRepository>();

        return serviceCollection;
    }

    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IFileEntityService, FileEntityService>();
        serviceCollection.AddScoped<IJsonWebTokenEntityService, JsonWebTokenEntityService>();
        serviceCollection.AddScoped<IPermissionEntityService, PermissionEntityService>();
        serviceCollection.AddScoped<IRefreshTokenEntityService, RefreshTokenEntityService>();
        serviceCollection.AddScoped<IPermissionValueEntityCollectionService, PermissionValueEntityCollectionService>();
        serviceCollection.AddScoped<IUserGroupEntityService, UserGroupEntityService>();
        serviceCollection.AddScoped<IUserProfileEntityService, UserProfileEntityService>();
        serviceCollection.AddScoped<IUserEntityService, UserEntityService>();
        serviceCollection.AddScoped<IUserToUserGroupMappingEntityService, UserToUserGroupMappingEntityService>();
        serviceCollection.AddScoped<IAuthorizeEntityService, AuthorizeEntityService>();
        serviceCollection.AddScoped<IUserGroupTransferRequestEntityService, UserGroupTransferRequestEntityService>();
        serviceCollection.AddScoped<IUserGroupInviteRequestEntityService, UserGroupInviteRequestEntityService>();


        return serviceCollection;
    }

    public static IServiceCollection AddAdvancedServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IApplicationAdvancedService, ApplicationAdvancedService>();
        serviceCollection.AddScoped<IJsonWebTokenAdvancedService, JsonWebTokenAdvancedService>();
        serviceCollection.AddScoped<IUserAdvancedService, UserAdvancedService>();
        serviceCollection.AddTransient<IWarningAdvancedService, WarningAdvancedService>();
        serviceCollection.AddScoped<IAuthorizeAdvancedService, AuthorizeAdvancedService>();
        serviceCollection.AddSingleton<IRedisService, RedisService>();

        return serviceCollection;
    }

    public static IServiceCollection AddHandlers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAuthHandler, AuthHandler>();
        serviceCollection.AddScoped<IFileHandler, FileHandler>();
        serviceCollection.AddScoped<IUserGroupPermissionValueHandler, UserGroupPermissionValueHandler>();
        serviceCollection.AddScoped<IPermissionHandler, PermissionHandler>();
        serviceCollection.AddScoped<IDomainInfoHandler, DomainInfoHandler>();
        serviceCollection.AddScoped<IApplicationHandler, ApplicationHandler>();
        serviceCollection.AddScoped<IUserGroupHandler, UserGroupHandler>();
        serviceCollection.AddScoped<IUserPermissionValueHandler, UserPermissionValueHandler>();
        serviceCollection.AddScoped<IUserGroupActionsHandler, UserGroupActionsHandler>();

        return serviceCollection;
    }

    public static IServiceCollection AddBackgroundServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHostedService<ConsumeScopedServiceHostedService>();
        // serviceCollection.AddScoped<IScopedProcessingService, JsonWebTokenBackgroundService>();

        return serviceCollection;
    }

    public static IServiceCollection AddJobs(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IJsonWebTokenJobs, JsonWebTokenJobs>();
        serviceCollection.AddScoped<IUserJobs, UserJobs>();
        serviceCollection.AddScoped<IRefreshTokenJobs, RefreshTokenJobs>();
        serviceCollection.AddScoped<IAuthorizeJobs, AuthorizeJobs>();

        return serviceCollection;
    }

    public static IServiceCollection AddCustomDbContext(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        IHostEnvironment env
    )
    {
        serviceCollection.AddScoped<AppDbContext>();
        serviceCollection.AddScoped<IAppDbContextAction, AppDbContextAction>();

        serviceCollection.AddDbContext<AppDbContext>(options =>
        {
            options
                .UseNpgsql(configuration.GetConnectionString("Default") + (env.IsDevelopment() ? ";Include Error Detail=true" : ""),
                    _ => _.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery))
                .UseLazyLoadingProxies();

            if (env.IsDevelopment())
            {
                options.EnableSensitiveDataLogging().EnableDetailedErrors();
            }
        });

        return serviceCollection;
    }

    public static IServiceCollection AddCustomDbContextTest(
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

    public static IServiceCollection AddCustomHangfire(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        serviceCollection.AddHangfire(_ =>
            _.SetDataCompatibilityLevel(CompatibilityLevel.Version_170).UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(configuration.GetConnectionString("Hangfire"), new PostgreSqlStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromMinutes(5)
                }));

        return serviceCollection;
    }

    public static IServiceCollection AddCustomHangfireTest(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        serviceCollection.AddHangfire(_ =>
            _.SetDataCompatibilityLevel(CompatibilityLevel.Version_170).UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(configuration.GetConnectionString("HangfireTest"), new PostgreSqlStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromMinutes(5)
                }));

        return serviceCollection;
    }

    public static IServiceCollection AddCustomConfigureOptions(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IConfigureOptions<AuthenticationOptions>, ConfigureAuthenticationOptions>();
        serviceCollection
            .AddSingleton<IConfigureOptions<JsonWebTokenAuthenticationSchemeOptions>, ConfigureJwtBearerOptions>();

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
        serviceCollection.Configure<MiscOptions>(configuration.GetSection(nameof(MiscOptions)));
        serviceCollection.Configure<RedisOptions>(configuration.GetSection(nameof(RedisOptions)));
        serviceCollection.Configure<SeqOptions>(configuration.GetSection(nameof(SeqOptions)));

        return serviceCollection;
    }

    public static IServiceCollection AddCustomLogging(
        this IServiceCollection serviceCollection,
        IHostEnvironment env,
        IConfiguration configuration,
        IServiceProvider serviceProvider
    )
    {
        // var logger = new LoggerConfiguration()
        //     .ReadFrom.Configuration(configuration)
        //     .Enrich.FromLogContext()
        //     .WriteTo.Console()
        //     .WriteTo.Seq("http://localhost:5341")
        //     .CreateLogger();
        //
        // Log.Logger = logger;
        //
        // serviceCollection.AddLogging(_ => _.ClearProviders().AddSerilog(Log.Logger, true));

        return serviceCollection;
    }
}
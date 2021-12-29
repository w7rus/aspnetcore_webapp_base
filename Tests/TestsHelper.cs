using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.Handlers;
using BLL.Services;
using DAL.Data;
using DAL.Repository;
using DAL.Repository.Base;
using Domain.Entities;
using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Tests;

public static class TestsHelper
{
    public static IServiceProvider GetIServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddLogging();

        //DbContext
        {
            services.AddScoped<AppDbContext>();
            services.AddScoped<IAppDbContextAction, AppDbContextAction>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });
        }

        //DI
        {
            //Repositories
            services.AddScoped<IJsonWebTokenRepository, JsonWebTokenRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUserGroupPermissionValueRepository, UserGroupPermissionValueRepository>();
            services.AddScoped<IUserGroupRepository, UserGroupRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserToGroupMappingRepository, UserToGroupMappingRepository>();

            //Services
            services.AddScoped<IJsonWebTokenService, JsonWebTokenService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IUserGroupPermissionValueService, UserGroupPermissionValueService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IUserService, UserService>();

            //Handlers
            services.AddScoped<IAuthHandler, AuthHandler>();
        }

        return services.BuildServiceProvider();
    }

    public static async Task SeedAppDbContext(
        IAppDbContextAction appDbContextAction,
        IPermissionRepository permissionRepository,
        IUserRepository userRepository,
        IUserGroupRepository userGroupRepository,
        IUserGroupPermissionValueRepository userGroupPermissionValueRepository,
        IUserProfileRepository userProfileRepository,
        IUserToGroupMappingRepository userToGroupMappingRepository,
        IJsonWebTokenRepository jsonWebTokenRepository,
        IRefreshTokenRepository refreshTokenRepository
    )
    {
        var appDbContextSeedLists = AppDbContext.Seed();

        await permissionRepository.AddAsync(appDbContextSeedLists.Permissions);
        await userRepository.AddAsync(appDbContextSeedLists.Users);
        await userGroupRepository.AddAsync(appDbContextSeedLists.UserGroups);
        await userGroupPermissionValueRepository.AddAsync(appDbContextSeedLists.UserGroupPermissionValues);
        await userProfileRepository.AddAsync(appDbContextSeedLists.UserProfiles);
        await userToGroupMappingRepository.AddAsync(appDbContextSeedLists.UserToGroupMappings);
        await jsonWebTokenRepository.AddAsync(appDbContextSeedLists.JsonWebTokens);
        await refreshTokenRepository.AddAsync(appDbContextSeedLists.RefreshTokens);

        await appDbContextAction.CommitAsync();
    }
}
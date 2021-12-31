using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
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
    // public static IServiceProvider GetIServiceProvider()
    // {
    //     var services = new ServiceCollection();
    //
    //     services.AddLogging();
    //     services.AddHttpContextAccessor();
    //     services.AddCustomDbContextInMemory();
    //     services.AddRepositories();
    //     services.AddServices();
    //     services.AddHandlers();
    //
    //     return services.BuildServiceProvider();
    // }

    public static async Task SeedAppDbContext(
        IServiceProvider serviceProvider
    )
    {
        var appDbContext = serviceProvider.GetRequiredService<AppDbContext>();
        var appDbContextAction = serviceProvider.GetRequiredService<IAppDbContextAction>();

        var permissionRepository = serviceProvider.GetRequiredService<IPermissionRepository>();
        var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
        var userGroupRepository = serviceProvider.GetRequiredService<IUserGroupRepository>();
        var userGroupPermissionValueRepository =
            serviceProvider.GetRequiredService<IUserGroupPermissionValueRepository>();
        var userProfileRepository = serviceProvider.GetRequiredService<IUserProfileRepository>();
        var userToGroupMappingRepository = serviceProvider.GetRequiredService<IUserToGroupMappingRepository>();
        var jsonWebTokenRepository = serviceProvider.GetRequiredService<IJsonWebTokenRepository>();
        var refreshTokenRepository = serviceProvider.GetRequiredService<IRefreshTokenRepository>();

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

    public static async Task PurgeAppDbContext(
        IServiceProvider serviceProvider
    )
    {
        var appDbContextAction = serviceProvider.GetRequiredService<IAppDbContextAction>();

        var permissionRepository = serviceProvider.GetRequiredService<IPermissionRepository>();
        var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
        var userGroupRepository = serviceProvider.GetRequiredService<IUserGroupRepository>();
        var userGroupPermissionValueRepository =
            serviceProvider.GetRequiredService<IUserGroupPermissionValueRepository>();
        var userProfileRepository = serviceProvider.GetRequiredService<IUserProfileRepository>();
        var userToGroupMappingRepository = serviceProvider.GetRequiredService<IUserToGroupMappingRepository>();
        var jsonWebTokenRepository = serviceProvider.GetRequiredService<IJsonWebTokenRepository>();
        var refreshTokenRepository = serviceProvider.GetRequiredService<IRefreshTokenRepository>();

        permissionRepository.Delete(await permissionRepository.QueryAll().ToArrayAsync());
        userRepository.Delete(await userRepository.QueryAll().ToArrayAsync());
        userGroupRepository.Delete(await userGroupRepository.QueryAll().ToArrayAsync());
        userGroupPermissionValueRepository.Delete(await userGroupPermissionValueRepository.QueryAll().ToArrayAsync());
        userProfileRepository.Delete(await userProfileRepository.QueryAll().ToArrayAsync());
        userToGroupMappingRepository.Delete(await userToGroupMappingRepository.QueryAll().ToArrayAsync());
        jsonWebTokenRepository.Delete(await jsonWebTokenRepository.QueryAll().ToArrayAsync());
        refreshTokenRepository.Delete(await refreshTokenRepository.QueryAll().ToArrayAsync());

        await appDbContextAction.CommitAsync();
    }
}
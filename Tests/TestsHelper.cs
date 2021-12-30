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
    public static IServiceProvider GetIServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddCustomDbContextInMemory();
        services.AddRepositories();
        services.AddServices();
        services.AddHandlers();

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
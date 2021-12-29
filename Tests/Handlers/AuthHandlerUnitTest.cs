using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BLL.Handlers;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using DTO.Models.Auth;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tests.Handlers;

public class AuthHandlerTests
{
    private readonly IServiceProvider _serviceProvider;

    public AuthHandlerTests()
    {
        _serviceProvider = TestsHelper.GetIServiceProvider();
    }

    [Fact]
    public async Task SignUp()
    {
        var appDbContext = _serviceProvider.GetRequiredService<AppDbContext>();
        var appDbContextAction = _serviceProvider.GetRequiredService<IAppDbContextAction>();

        var permissionRepository = _serviceProvider.GetRequiredService<IPermissionRepository>();
        var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
        var userGroupRepository = _serviceProvider.GetRequiredService<IUserGroupRepository>();
        var userGroupPermissionValueRepository =
            _serviceProvider.GetRequiredService<IUserGroupPermissionValueRepository>();
        var userProfileRepository = _serviceProvider.GetRequiredService<IUserProfileRepository>();
        var userToGroupMappingRepository = _serviceProvider.GetRequiredService<IUserToGroupMappingRepository>();
        var jsonWebTokenRepository = _serviceProvider.GetRequiredService<IJsonWebTokenRepository>();
        var refreshTokenRepository = _serviceProvider.GetRequiredService<IRefreshTokenRepository>();

        await TestsHelper.SeedAppDbContext(appDbContextAction, permissionRepository, userRepository,
            userGroupRepository, userGroupPermissionValueRepository, userProfileRepository,
            userToGroupMappingRepository, jsonWebTokenRepository, refreshTokenRepository);
        await appDbContextAction.CommitAsync();

        var authHandler = _serviceProvider.GetRequiredService<IAuthHandler>();

        // var userGuest = new User
        // {
        //     Id = new Guid(),
        // };
        // appDbContext.Users.Add(userGuest);
        // await appDbContextAction.CommitAsync();

        var test = await authHandler.SignUp(new AuthSignUp()
        {
        }, new CancellationToken() { });

        var users = userRepository.QueryAll().ToList();
        var userGroups = userGroupRepository.QueryAll().ToList();

        // Assert.NotNull(appDbContext.Users.FirstOrDefault(_ => _.Id == userGuest.Id));
        Assert.NotNull(
            appDbContext.Users.FirstOrDefault(_ => _.Id == Guid.Parse("00000047-0000-0000-0000-000000000000")));

        Assert.NotNull(test);

        // Assert.NotNull(_appDbContextFactory);
        // Assert.NotNull(appDbContext);

        // try
        // {
        //     await _appDbContextAction.BeginTransactionAsync();
        //
        //     var test = await _jsonWebTokenService.Add("test123", DateTimeOffset.UtcNow.AddDays(7),
        //         Guid.Parse("00000047-0000-0000-0000-000000000000"));
        //
        //     // throw new Exception("TEST");
        //
        //     await _appDbContextAction.CommitTransactionAsync();
        // }
        // catch (Exception e)
        // {
        //     _logger.Log(LogLevel.Error, Localize.Log.HandlerEnd(GetType().FullName), e.Message);
        //     await _appDbContextAction.RollbackTransactionAsync();
        // }
    }
}
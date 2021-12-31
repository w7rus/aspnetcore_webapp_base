using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Repository;
using DAL.Repository.Base;
using Domain.Entities;
using Moq;
using Xunit;

namespace Tests.Handlers;

public class AuthHandlerTests
{
    // private readonly IServiceProvider _serviceProvider;

    public AuthHandlerTests()
    {
        // _serviceProvider = TestsHelper.GetIServiceProvider();
    }


    [Fact]
    public async Task ExampleMoq()
    {
        var initialEntities = new List<User>
        {
            new()
            {
                Email = "test123@email.com",
                IsEmailValidated = false,
                PhoneNumber = null,
                IsPhoneNumberVerified = false,
                Password = "test123",
                FailedSignInAttempts = 0,
                DisableSignInUntil = null,
                LastSignIn = default,
                LastActivity = default,
                LastIpAddress = null,
                UserToGroupMappings = null,
                UserProfile = null
            }
        };

        var userDbSet = new List<User>();

        var userRepositoryMock = new Mock<IRepositoryBase<User, Guid>>();
        userRepositoryMock.Setup(_ => _.Add(initialEntities))
            .Callback<IList<User>>(_ => userDbSet.AddRange(_));
        userRepositoryMock.Setup(_ => _.QueryAll()).Returns(userDbSet.AsQueryable);

        var userRepository = userRepositoryMock.Object;
        
        userRepository.Add(initialEntities);
        var res = userRepository.QueryAll();
    }

    // [Fact]
    // public async Task SignUp()
    // {
    //     await TestsHelper.SeedAppDbContext(_serviceProvider);
    //     
    //     var authHandler = _serviceProvider.GetRequiredService<IAuthHandler>();
    //
    //     var result = await authHandler.SignUp(new AuthSignUp
    //     {
    //         Email = "test123@email.com",
    //         Password = "12345678",
    //         Username = "test123"
    //     }, default);
    //
    //     Assert.IsType<AuthSignUpResult>(result);
    //
    //     await TestsHelper.PurgeAppDbContext(_serviceProvider);
    //
    //     // var userGuest = new User
    //     // {
    //     //     Id = new Guid(),
    //     // };
    //     // appDbContext.Users.Add(userGuest);
    //     // await appDbContextAction.CommitAsync();
    //
    //     // var test = await authHandler.SignUp(new AuthSignUp()
    //     // {
    //     // }, new CancellationToken() { });
    //     //
    //     // var users = userRepository.QueryAll().ToList();
    //     // var userGroups = userGroupRepository.QueryAll().ToList();
    //     //
    //     // // Assert.NotNull(appDbContext.Users.FirstOrDefault(_ => _.Id == userGuest.Id));
    //     // Assert.NotNull(
    //     //     appDbContext.Users.FirstOrDefault(_ => _.Id == Guid.Parse("00000047-0000-0000-0000-000000000000")));
    //
    //     // Assert.NotNull(test);
    // }
    //
    // [Fact]
    // public async Task SignUpBadEmail()
    // {
    //     await TestsHelper.SeedAppDbContext(_serviceProvider);
    //     
    //     var authHandler = _serviceProvider.GetRequiredService<IAuthHandler>();
    //
    //     var result = await authHandler.SignUp(new AuthSignUp
    //     {
    //         Email = "test123",
    //         Password = "12345678",
    //         Username = "test123"
    //     }, default);
    //
    //     Assert.IsType<ErrorModelResult>(result);
    //     
    //     await TestsHelper.PurgeAppDbContext(_serviceProvider);
    // }
    //
    // [Fact]
    // public async Task SignUpBadPassword()
    // {
    //     await TestsHelper.SeedAppDbContext(_serviceProvider);
    //     
    //     var authHandler = _serviceProvider.GetRequiredService<IAuthHandler>();
    //
    //     var result = await authHandler.SignUp(new AuthSignUp
    //     {
    //         Email = "test123@email.com",
    //         Password = "12",
    //         Username = "test123"
    //     }, default);
    //
    //     Assert.IsType<ErrorModelResult>(result);
    //     
    //     await TestsHelper.PurgeAppDbContext(_serviceProvider);
    // }
    //
    // [Fact]
    // public async Task SignUpBadUsername()
    // {
    //     await TestsHelper.SeedAppDbContext(_serviceProvider);
    //     
    //     var authHandler = _serviceProvider.GetRequiredService<IAuthHandler>();
    //
    //     var result = await authHandler.SignUp(new AuthSignUp
    //     {
    //         Email = "test123@email.com",
    //         Password = "12345678",
    //         Username = ""
    //     }, default);
    //
    //     Assert.IsType<ErrorModelResult>(result);
    //     
    //     await TestsHelper.PurgeAppDbContext(_serviceProvider);
    // }
    //
    // [Fact]
    // public async Task SignUpBadAlreadyExists()
    // {
    //     await TestsHelper.SeedAppDbContext(_serviceProvider);
    //     
    //     var authHandler = _serviceProvider.GetRequiredService<IAuthHandler>();
    //     var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
    //
    //     var result = await authHandler.SignUp(new AuthSignUp
    //     {
    //         Email = "test123@email.com",
    //         Password = "12345678",
    //         Username = "test123"
    //     }, default);
    //
    //     var result2 = await authHandler.SignUp(new AuthSignUp
    //     {
    //         Email = "test123@email.com",
    //         Password = "12345678",
    //         Username = "test123"
    //     }, default);
    //     
    //     var users = userRepository.QueryAll().ToList();
    //
    //     Assert.IsType<ErrorModelResult>(result2);
    //     
    //     await TestsHelper.PurgeAppDbContext(_serviceProvider);
    // }
}
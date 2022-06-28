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
    [Fact]
    public async Task ExampleMoq()
    {
        var initialEntities = new List<User>
        {
            new()
            {
                Email = "test123@email.com",
                IsEmailVerified = false,
                Password = "test123",
                FailedSignInAttempts = 0,
                DisableSignInUntil = null,
                LastSignIn = default,
                LastActivity = default,
                LastIpAddress = null,
            }
        };

        var userDbSet = new List<User>();

        var userRepositoryMock = new Mock<IRepositoryBase<User, Guid>>();
        userRepositoryMock.Setup(_ => _.Add(initialEntities))
            .Callback<IList<User>>(_ => userDbSet.AddRange(_));
        userRepositoryMock.Setup(_ => _.QueryAll()).Returns(userDbSet.AsQueryable);

        var userRepository = userRepositoryMock.Object;

        await userRepository.AddAsync(initialEntities);
        var res = userRepository.QueryAll();
    }
}
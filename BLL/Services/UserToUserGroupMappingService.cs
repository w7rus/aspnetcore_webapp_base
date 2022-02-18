using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Base;
using DAL.Data;
using DAL.Repository;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BLL.Services;

/// <summary>
/// Service to work with UserToUserGroupMapping entity
/// </summary>
public interface IUserToUserGroupMappingService : IEntityToGroupMappingServiceBase<UserToUserGroupMapping>
{
}

public class UserToUserGroupMappingService : IUserToUserGroupMappingService
{
    #region Fields

    private readonly ILogger<UserToUserGroupMappingService> _logger;
    private readonly IUserToUserGroupMappingRepository _userToUserGroupMappingRepository;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Ctor

    public UserToUserGroupMappingService(
        ILogger<UserToUserGroupMappingService> logger,
        IUserToUserGroupMappingRepository userToUserGroupMappingRepository,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _userToUserGroupMappingRepository = userToUserGroupMappingRepository;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Methods

    public async Task Save(UserToUserGroupMapping entity, CancellationToken cancellationToken = default)
    {
        _userToUserGroupMappingRepository.Save(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task Delete(UserToUserGroupMapping entity, CancellationToken cancellationToken = default)
    {
        _userToUserGroupMappingRepository.Delete(entity);
        await _appDbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<UserToUserGroupMapping> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _userToUserGroupMappingRepository.SingleOrDefaultAsync(_ => _.Id == id);
    }

    public async Task<UserToUserGroupMapping> Create(
        UserToUserGroupMapping entity,
        CancellationToken cancellationToken = default
    )
    {
        await Save(entity, cancellationToken);
        return entity;
    }

    #endregion
}
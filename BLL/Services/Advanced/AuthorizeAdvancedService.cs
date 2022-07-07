using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Entity;
using Common.Exceptions;
using Common.Models;
using DAL.Data;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services.Advanced;

public interface IAuthorizeAdvancedService
{
    bool Authorize(AuthorizeModel authorizeModel);

    Task<bool> IsPermissionValueSet(
        string permissionAlias,
        Guid entityId,
        PermissionType permissionType,
        CancellationToken cancellationToken
    );
}

public class AuthorizeAdvancedService : IAuthorizeAdvancedService
{
    #region Ctor

    public AuthorizeAdvancedService(
        AppDbContext appDbContext,
        IAuthorizeEntityService authorizeEntityService,
        IPermissionValueEntityService permissionValueEntityService,
        IPermissionEntityService permissionEntityService
    )
    {
        _appDbContext = appDbContext;
        _authorizeEntityService = authorizeEntityService;
        _permissionValueEntityService = permissionValueEntityService;
        _permissionEntityService = permissionEntityService;
    }

    #endregion

    #region Methods

    public bool Authorize(AuthorizeModel authorizeModel)
    {
        var sql = authorizeModel.GetRawSqlAuthorizeResult();

        var authorizeModelResult = _appDbContext.Set<AuthorizeResult>()
            .FromSqlRaw(sql).ToList().SingleOrDefault();

        return authorizeModelResult?.Result != null && authorizeModelResult.Result;
    }

    public async Task<bool> IsPermissionValueSet(
        string permissionAlias,
        Guid entityId,
        PermissionType permissionType,
        CancellationToken cancellationToken
    )
    {
        var permission = await _permissionEntityService.GetByAliasTypeAsync(permissionAlias, permissionType);
        if (permission == null)
            throw new CustomException(Localize.Error.PermissionNotFound);

        var permissionValue =
            await _permissionValueEntityService.GetByPermissionIdEntityIdAsync(permission.Id, entityId,
                cancellationToken);

        return permissionValue != null;
    }

    #endregion

    #region Fields

    private readonly AppDbContext _appDbContext;
    private readonly IAuthorizeEntityService _authorizeEntityService;
    private readonly IPermissionValueEntityService _permissionValueEntityService;
    private readonly IPermissionEntityService _permissionEntityService;

    #endregion
}
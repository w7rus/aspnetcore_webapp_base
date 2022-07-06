using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Handlers.Base;
using BLL.Services.Entity;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Common.Models.Base;
using DAL.Data;
using DTO.Models.Permission;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BLL.Handlers;

public interface IPermissionHandler
{
    Task<IDtoResultBase> Read(PermissionReadDto data, CancellationToken cancellationToken = default);

    Task<IDtoResultBase> ReadCollection(
        PermissionReadCollectionDto data,
        CancellationToken cancellationToken = default
    );
}

public class PermissionHandler : HandlerBase, IPermissionHandler
{
    #region Ctor

    public PermissionHandler(
        ILogger<HandlerBase> logger,
        IAppDbContextAction appDbContextAction,
        IMapper mapper,
        IPermissionEntityService permissionEntityService
    )
    {
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _mapper = mapper;
        _permissionEntityService = permissionEntityService;
    }

    #endregion

    #region Fields

    private readonly ILogger<HandlerBase> _logger;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly IMapper _mapper;
    private readonly IPermissionEntityService _permissionEntityService;

    #endregion

    #region Methods

    public async Task<IDtoResultBase> Read(PermissionReadDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Read)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var permission = await _permissionEntityService.GetByIdAsync(data.Id, cancellationToken);
            if (permission == null)
                throw new HttpResponseException(StatusCodes.Status404NotFound, ErrorType.Permission,
                    Localize.Error.PermissionNotFound);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Read)));

            return _mapper.Map<PermissionReadResultDto>(permission);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<IDtoResultBase> ReadCollection(
        PermissionReadCollectionDto data,
        CancellationToken cancellationToken = default
    )
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(ReadCollection)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var permissions = await _permissionEntityService.GetFiltered(data.FilterExpressionModel,
                data.FilterSortModel, data.PageModel, null, cancellationToken: cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();

            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Read)));

            return new PermissionReadFSPCollectionResultDto
            {
                Total = permissions.total,
                Items = permissions.entities.Select(_ =>
                    _mapper.ProjectTo<PermissionReadFSPCollectionItemResultDto>(new[] {_}.AsQueryable()).Single())
            };
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    #endregion
}
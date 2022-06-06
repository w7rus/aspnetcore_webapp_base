using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Handlers.Base;
using BLL.Services;
using Common.Enums;
using Common.Exceptions;
using Common.Models;
using Common.Models.Base;
using DAL.Data;
using Domain.Entities;
using DTO.Models.Permission;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ValueType = Common.Enums.ValueType;

namespace BLL.Handlers;

public interface IPermissionHandler
{
    Task<DTOResultBase> Read(PermissionRead data, CancellationToken cancellationToken = default);
    Task<DTOResultBase> ReadFSPCollection(PermissionReadFSPCollection data, CancellationToken cancellationToken = default);
}

public class PermissionHandler : HandlerBase, IPermissionHandler
{
    #region Fields

    private readonly ILogger<HandlerBase> _logger;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly IMapper _mapper;
    private readonly IPermissionService _permissionService;

    #endregion

    #region Ctor

    public PermissionHandler(
        ILogger<HandlerBase> logger,
        IAppDbContextAction appDbContextAction,
        IMapper mapper,
        IPermissionService permissionService
    )
    {
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _mapper = mapper;
        _permissionService = permissionService;
    }

    #endregion

    #region Methods

    public async Task<DTOResultBase> Read(PermissionRead data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Read)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;
        
        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var permission = await _permissionService.GetByIdAsync(data.Id, cancellationToken);
            if (permission == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Permission,
                    Localize.Error.PermissionDoesNotExist);
            
            await _appDbContextAction.CommitTransactionAsync();
            
            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Read)));
            
            return _mapper.Map<PermissionReadResult>(permission);
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }
    
    public async Task<DTOResultBase> ReadFSPCollection(PermissionReadFSPCollection data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(ReadFSPCollection)));

        if (ValidateModel(data) is { } validationResult)
            return validationResult;
        
        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var permissions = await _permissionService.GetFilteredSortedPaged(data.FilterExpressionModel,
                data.FilterSortModel, data.PageModel, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();
            
            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Read)));

            return new PermissionReadFSPCollectionResult()
            {
                Total = permissions.total,
                Items = permissions.entities.Select(_ =>
                    _mapper.ProjectTo<PermissionReadFSPCollectionItemResult>(new[] {_}.AsQueryable()).Single())
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
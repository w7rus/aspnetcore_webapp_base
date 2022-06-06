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
    Task<DTOResultBase> ReadFilteredSortedPaged(PermissionReadCollection data, CancellationToken cancellationToken = default);
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

    //TODO: Rename DTO (Filtered) + inherit PageModel + Add Sorting Model
    public async Task<DTOResultBase> ReadFilteredSortedPaged(PermissionReadCollection data, CancellationToken cancellationToken = default)
    {
        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            data.FilterMatchModel = new FilterMatchModel
            {
                ExpressionLogicalOperation = ExpressionLogicalOperation.Not,
                Items = new List<FilterMatchModelItem>()
                {
                    new FilterMatchModelItemScope
                    {
                        ExpressionLogicalOperation = ExpressionLogicalOperation.None,
                        Items = new List<FilterMatchModelItem>()
                        {
                            new FilterMatchModelItemExpression
                            {
                                ExpressionLogicalOperation = ExpressionLogicalOperation.Not,
                                Key = "ValueType",
                                Value = BitConverter.GetBytes(8),
                                FilterMatchOperation = FilterMatchOperation.Equal
                            },
                            new FilterMatchModelItemExpression
                            {
                                ExpressionLogicalOperation = ExpressionLogicalOperation.And,
                                Key = "ValueType",
                                Value = BitConverter.GetBytes(9),
                                FilterMatchOperation = FilterMatchOperation.Equal
                            }
                        }
                    },
                    new FilterMatchModelItemExpression
                    {
                        ExpressionLogicalOperation = ExpressionLogicalOperation.And,
                        Key = "Alias",
                        Value = Encoding.UTF8.GetBytes("file"),
                        FilterMatchOperation = FilterMatchOperation.Contains
                    },
                    new FilterMatchModelItemScope
                    {
                        ExpressionLogicalOperation = ExpressionLogicalOperation.Or,
                        Items = new List<FilterMatchModelItem>()
                        {
                            new FilterMatchModelItemExpression
                            {
                                ExpressionLogicalOperation = ExpressionLogicalOperation.Not,
                                Key = "ValueType",
                                Value = BitConverter.GetBytes(8),
                                FilterMatchOperation = FilterMatchOperation.Equal
                            },
                        }
                    },
                }
            };

            var permissions = await _permissionService.GetFilteredSortedPaged(data.FilterMatchModel,
                data.FilterSortModel, data.PageModel, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();
            
            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Read)));

            return new PermissionReadCollectionResult()
            {
                Total = permissions.total,
                Items = permissions.entities.Select(_ =>
                    _mapper.ProjectTo<PermissionReadCollectionItemResult>(new[] {_}.AsQueryable()).Single())
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
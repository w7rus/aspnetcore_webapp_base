using System;
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
            
            return _mapper.Map<PermissionReadCollectionResult>(permission);
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
        var vl = Convert.ToBase64String(BitConverter.GetBytes(9));
        vl = Convert.ToBase64String(BitConverter.GetBytes(8));
        
        data.FilterMatchModel = new FilterMatchModel
        {
            MatchRules = new[]
            {
                new FilterMatchModelItem
                {
                    Key = "ValueType",
                    Value = Convert.FromBase64String("CAAAAA=="), //CQAAAA== 9 //CAAAAA== 8
                    ValueType = ValueType.Int32,
                    FilterMatchMode = FilterMatchMode.Equal
                }
            }
        };

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            // var permissions = await _permissionService.GetBySubAliasAndTypeAsync(data.SubAlias, data.PermissionType,
            //     data.Page, data.PageSize, cancellationToken);

            var permissions = await _permissionService.GetFilteredSortedPaged(data.FilterMatchModel,
                data.FilterSortModel, data.PageModel, cancellationToken);

            await _appDbContextAction.CommitTransactionAsync();
            
            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Read)));

            return new PermissionReadFilteredResult()
            {
                Total = permissions.Total,
                Items = permissions.Items.Select(_ =>
                    _mapper.ProjectTo<PermissionReadCollectionResult>(new[] {_}.AsQueryable()).Single())
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
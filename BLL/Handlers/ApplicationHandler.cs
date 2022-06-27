using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Handlers.Base;
using BLL.Services;
using BLL.Services.Advanced;
using BLL.Services.Entity;
using Common.Enums;
using Common.Exceptions;
using Common.Helpers;
using Common.Models;
using Common.Models.Base;
using DAL.Data;
using DTO.Models.Application;
using DTO.Models.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BLL.Handlers;

public interface IApplicationHandler
{
    Task<DTOResultBase> Setup(ApplicationSetupDto data, CancellationToken cancellationToken = default);
}

public class ApplicationHandler : HandlerBase, IApplicationHandler
{
    #region Fields

    private readonly ILogger<ApplicationHandler> _logger;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly IUserEntityService _userEntityService;
    private readonly IWarningAdvancedService _warningAdvancedService;

    #endregion

    #region Ctor

    public ApplicationHandler(
        ILogger<ApplicationHandler> logger,
        IAppDbContextAction appDbContextAction,
        IUserEntityService userEntityService,
        IWarningAdvancedService warningAdvancedService
    )
    {
        _logger = logger;
        _appDbContextAction = appDbContextAction;
        _userEntityService = userEntityService;
        _warningAdvancedService = warningAdvancedService;
    }

    #endregion

    #region Methods

    public async Task<DTOResultBase> Setup(ApplicationSetupDto data, CancellationToken cancellationToken = default)
    {
        _logger.Log(LogLevel.Information, Localize.Log.MethodStart(GetType(), nameof(Setup)));
        
        if (ValidateModel(data) is { } validationResult)
            return validationResult;

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            var user = await _userEntityService.GetByIdAsync(Consts.RootUserId, cancellationToken);
            if (user == null)
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserNotFound);
            
            if (!string.IsNullOrEmpty(user.Password))
                throw new HttpResponseException(StatusCodes.Status500InternalServerError, ErrorType.Generic,
                    Localize.Error.UserAlreadyClaimed);
            
            var customPasswordHasher = new CustomPasswordHasher();

            var passwordHashed = customPasswordHasher.HashPassword(data.Password);
            
            user.Email = data.Email;
            user.Password = passwordHashed;
            
            await _userEntityService.Save(user, cancellationToken);
            
            _logger.Log(LogLevel.Information, Localize.Log.MethodEnd(GetType(), nameof(Setup)));
            
            await _appDbContextAction.CommitTransactionAsync();

            return new OkResultDto();
        }
        catch (Exception)
        {
            await _appDbContextAction.RollbackTransactionAsync();

            throw;
        }
    }

    #endregion
}
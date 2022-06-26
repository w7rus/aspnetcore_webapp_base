using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services;
using BLL.Services.Entity;
using Common.Models;
using Common.Options;
using DAL.Data;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BLL.BackgroundServices;

public class JsonWebTokenBackgroundService : IScopedProcessingService
{
    #region Fields

    private readonly string _fullName;
    private readonly ILogger<JsonWebTokenBackgroundService> _logger;
    private readonly IJsonWebTokenEntityService _jsonWebTokenEntityService;
    private readonly IAppDbContextAction _appDbContextAction;
    private readonly BackgroundServicesOptions _backgroundServicesOptions;

    #endregion

    #region Ctor

    public JsonWebTokenBackgroundService(
        ILogger<JsonWebTokenBackgroundService> logger,
        IJsonWebTokenEntityService jsonWebTokenEntityService,
        IAppDbContextAction appDbContextAction,
        IOptions<BackgroundServicesOptions> backgroundServicesOptions
    )
    {
        _fullName = GetType().FullName;
        _logger = logger;
        _jsonWebTokenEntityService = jsonWebTokenEntityService;
        _appDbContextAction = appDbContextAction;
        _backgroundServicesOptions = backgroundServicesOptions.Value;
    }

    #endregion

    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Log(LogLevel.Information, Localize.Log.BackgroundServiceStarting(_fullName));

        stoppingToken.Register(() =>
            _logger.Log(LogLevel.Information, Localize.Log.BackgroundServiceStopping(_fullName)));

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.Log(LogLevel.Information, Localize.Log.BackgroundServiceWorking(_fullName));

            try
            {
                await _appDbContextAction.BeginTransactionAsync();

                await _jsonWebTokenEntityService.PurgeAsync(stoppingToken);

                await _appDbContextAction.CommitTransactionAsync();
            }
            catch (Exception e)
            {
                await _appDbContextAction.RollbackTransactionAsync();
                _logger.Log(LogLevel.Error, Localize.Log.BackgroundServiceError(_fullName, e.Message));
            }

            await Task.Delay(
                _backgroundServicesOptions.JsonWebTokenBackgroundServiceOptions.CheckUpdateTimeSeconds * 1000,
                stoppingToken);
        }

        _logger.Log(LogLevel.Information, Localize.Log.BackgroundServiceStopping(_fullName));
    }
}
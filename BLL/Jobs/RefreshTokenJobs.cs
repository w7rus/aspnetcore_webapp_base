using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Entity;
using Common.Models;
using DAL.Data;
using Microsoft.Extensions.Logging;

namespace BLL.Jobs;

public interface IRefreshTokenJobs
{
    public Task PurgeAsync(CancellationToken stoppingToken = default);
}

public class RefreshTokenJobs : IRefreshTokenJobs
{
    #region Fields

    private readonly ILogger<RefreshTokenJobs> _logger;
    private readonly IRefreshTokenEntityService _refreshTokenEntityService;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Ctor

    public RefreshTokenJobs(
        ILogger<RefreshTokenJobs> logger,
        IRefreshTokenEntityService refreshTokenEntityService,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _refreshTokenEntityService = refreshTokenEntityService;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Methods

    public async Task PurgeAsync(CancellationToken stoppingToken = default)
    {
        var jobName = $"{GetType().FullName}.PurgeAsync";

        _logger.Log(LogLevel.Information, Localize.Log.JobExecuted(jobName));

        stoppingToken.Register(() =>
            _logger.Log(LogLevel.Information, Localize.Log.JobAborted(jobName)));

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            await _refreshTokenEntityService.PurgeAsync(stoppingToken);

            await _appDbContextAction.CommitTransactionAsync();
        }
        catch (Exception e)
        {
            await _appDbContextAction.RollbackTransactionAsync();
            _logger.Log(LogLevel.Error, Localize.Log.JobError(jobName, e.Message));
        }

        _logger.Log(LogLevel.Information, Localize.Log.JobCompleted(jobName));
    }

    #endregion
}
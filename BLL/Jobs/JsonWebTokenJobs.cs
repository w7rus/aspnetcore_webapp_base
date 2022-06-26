using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services;
using BLL.Services.Entity;
using Common.Models;
using DAL.Data;
using Microsoft.Extensions.Logging;

namespace BLL.Jobs;

public interface IJsonWebTokenJobs
{
    public Task PurgeAsync(CancellationToken stoppingToken = default);
}

public class JsonWebTokenJobs : IJsonWebTokenJobs
{
    #region Fields

    private readonly string _fullName;
    private readonly ILogger<JsonWebTokenJobs> _logger;
    private readonly IJsonWebTokenEntityService _jsonWebTokenEntityService;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Ctor

    public JsonWebTokenJobs(
        ILogger<JsonWebTokenJobs> logger,
        IJsonWebTokenEntityService jsonWebTokenEntityService,
        IAppDbContextAction appDbContextAction
    )
    {
        _fullName = GetType().FullName;
        _logger = logger;
        _jsonWebTokenEntityService = jsonWebTokenEntityService;
        _appDbContextAction = appDbContextAction;
    }

    #endregion

    #region Methods

    public async Task PurgeAsync(CancellationToken stoppingToken = default)
    {
        var jobName = $"{_fullName}.PurgeAsync";

        _logger.Log(LogLevel.Information, Localize.Log.JobExecuted(jobName));

        stoppingToken.Register(() =>
            _logger.Log(LogLevel.Information, Localize.Log.JobAborted(jobName)));

        try
        {
            await _appDbContextAction.BeginTransactionAsync();

            await _jsonWebTokenEntityService.PurgeAsync(stoppingToken);

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
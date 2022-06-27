using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.Entity;
using Common.Models;
using DAL.Data;
using Microsoft.Extensions.Logging;

namespace BLL.Jobs;

public interface IAuthorizeJobs
{
    public Task PurgeAsync(CancellationToken stoppingToken = default);
}

public class AuthorizeJobs : IAuthorizeJobs
{
    #region Fields

    private readonly ILogger<AuthorizeJobs> _logger;
    private readonly IAuthorizeEntityService _authorizeEntityService;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Ctor

    public AuthorizeJobs(
        ILogger<AuthorizeJobs> logger,
        IAuthorizeEntityService authorizeEntityService,
        IAppDbContextAction appDbContextAction
    )
    {
        _logger = logger;
        _authorizeEntityService = authorizeEntityService;
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

            await _authorizeEntityService.PurgeAsync(stoppingToken);

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
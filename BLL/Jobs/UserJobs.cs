using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services;
using Common.Models;
using DAL.Data;
using Microsoft.Extensions.Logging;

namespace BLL.Jobs;

public interface IUserJobs
{
    public Task PurgeAsync(CancellationToken stoppingToken = default);
}

public class UserJobs : IUserJobs
{
    #region Fields

    private readonly string _fullName;
    private readonly ILogger<UserJobs> _logger;
    private readonly IUserService _userService;
    private readonly IAppDbContextAction _appDbContextAction;

    #endregion

    #region Ctor

    public UserJobs(ILogger<UserJobs> logger, IUserService userService, IAppDbContextAction appDbContextAction)
    {
        _fullName = GetType().FullName;
        _logger = logger;
        _userService = userService;
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

            await _userService.PurgeAsync(stoppingToken);

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
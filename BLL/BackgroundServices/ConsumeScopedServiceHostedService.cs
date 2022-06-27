using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BLL.BackgroundServices;

public class ConsumeScopedServiceHostedService : BackgroundService
{
    #region Ctor

    public ConsumeScopedServiceHostedService(
        IServiceProvider serviceProvider,
        ILogger<ConsumeScopedServiceHostedService> logger
    )
    {
        _fullName = GetType().FullName;
        ServiceProvider = serviceProvider;
        _logger = logger;
    }

    #endregion

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Log(LogLevel.Information, Localize.Log.BackgroundServiceStarting(_fullName));

        stoppingToken.Register(() =>
            _logger.Log(LogLevel.Information, Localize.Log.BackgroundServiceStopping(_fullName)));

        using var scope = ServiceProvider.CreateScope();

        var scopedProcessingService =
            scope.ServiceProvider
                .GetRequiredService<IScopedProcessingService>();

        _logger.Log(LogLevel.Information, Localize.Log.BackgroundServiceWorking(_fullName));

        await scopedProcessingService.ExecuteAsync(stoppingToken);

        _logger.Log(LogLevel.Information, Localize.Log.BackgroundServiceStopping(_fullName));
    }

    #region Fields

    private readonly string _fullName;
    private readonly ILogger<ConsumeScopedServiceHostedService> _logger;
    private IServiceProvider ServiceProvider { get; }

    #endregion
}
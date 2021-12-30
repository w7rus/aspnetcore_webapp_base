using System.Threading;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.Extensions.Logging;

namespace BLL.BackgroundServices;

public interface IScopedProcessingService
{
    Task ExecuteAsync(CancellationToken stoppingToken);
}
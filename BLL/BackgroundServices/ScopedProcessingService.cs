using System.Threading;
using System.Threading.Tasks;

namespace BLL.BackgroundServices;

public interface IScopedProcessingService
{
    Task ExecuteAsync(CancellationToken stoppingToken);
}
using Microsoft.Extensions.Hosting;

namespace ContosoDashboard.Services;

public class DocumentScanWorker : BackgroundService
{
    private readonly ILogger<DocumentScanWorker> _logger;

    public DocumentScanWorker(ILogger<DocumentScanWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DocumentScanWorker starting up.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Skeleton: Poll for messages or use a queue abstraction
                _logger.LogDebug("DocumentScanWorker heartbeat - no-op (skeleton)");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // graceful shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DocumentScanWorker loop");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("DocumentScanWorker stopping.");
    }
}

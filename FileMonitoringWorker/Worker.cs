
using FileMonitoringWorker.Configuration;
using FileMonitoringWorker.Helpers;
using Microsoft.Extensions.Options;

namespace FileMonitoringWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly FileMonitorSettings _settings;

    public Worker(ILogger<Worker> logger,IOptions<FileMonitorSettings> options)
    {
        _logger = logger;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        try
        {
            // Initialize required directories
            DirectoryManager.InitializeDirectories(_settings.SourceFolder,_settings.DestinationFolder,_settings.LogFolder);

            _logger.LogInformation("All required directories initialized successfully.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}",DateTimeOffset.Now);

                await Task.Delay(5000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            // Normal application shutdown
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Worker encountered an error.");
            throw;
        }
    }
}

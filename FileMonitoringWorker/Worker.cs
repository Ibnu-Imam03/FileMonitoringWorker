
using FileMonitoringWorker.Configuration;
using FileMonitoringWorker.Helpers;
using FileMonitoringWorker.Services;
using Microsoft.Extensions.Options;

namespace FileMonitoringWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly FileMonitorSettings _settings;
    private readonly FileMonitorService _fileMonitor;

    public Worker(
        ILogger<Worker> logger,
        IOptions<FileMonitorSettings> options,
        FileMonitorService fileMonitor)
    {
        _logger = logger;
        _settings = options.Value;
        _fileMonitor = fileMonitor;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        try
        {
            // Initialize required directories
            DirectoryManager.InitializeDirectories(
                _settings.SourceFolder,
                _settings.DestinationFolder,
                _settings.LogFolder);

            _logger.LogInformation(
                "Directories initialized successfully.");

            // Start monitoring
            _fileMonitor.StartWatching(
                _settings.SourceFolder,
                _settings.DestinationFolder);

            _logger.LogInformation(
                "File Monitoring Worker started.");

            // Keep the background service alive
            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "File Monitoring Worker stopped.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Worker encountered an error.");

            throw;
        }
    }
}

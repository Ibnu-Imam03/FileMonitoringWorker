
namespace FileMonitoringWorker.Services;

public class FileMonitorService : IDisposable
{
    private readonly ILogger<FileMonitorService> _logger;
    private FileSystemWatcher? _watcher;

    public FileMonitorService(ILogger<FileMonitorService> logger)
    {
        _logger = logger;
    }

    public void StartWatching(string sourceFolder)
    {
        _watcher = new FileSystemWatcher(sourceFolder);

        // Configure which changes to monitor
        _watcher.Filter = "*.*";
        _watcher.NotifyFilter = NotifyFilters.FileName |NotifyFilters.CreationTime;

        // Subscribe to the Created event
        _watcher.Created += OnFileCreated;

        // Start monitoring
        _watcher.EnableRaisingEvents = true;

        _logger.LogInformation("Started monitoring: {Folder}", sourceFolder);
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        _logger.LogInformation("New file detected: {FileName}", e.Name);
    }

    public void Dispose()
    {
        _watcher?.Dispose();
    }
}

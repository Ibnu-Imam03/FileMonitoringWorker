
using System.Collections.Concurrent;
using FileMonitoringWorker.Helpers;
namespace FileMonitoringWorker.Services;

public class FileMonitorService : IDisposable
{
    private readonly DailyFileLogger _fileLogger;
    private readonly ILogger<FileMonitorService> _logger;
    private readonly ConcurrentDictionary<string, byte> _processingFiles =
    new(StringComparer.OrdinalIgnoreCase);
    private FileSystemWatcher? _watcher;

    private string _destinationFolder = string.Empty;

    public FileMonitorService(
        ILogger<FileMonitorService> logger,
        DailyFileLogger fileLogger)
    {
        _logger = logger;
        _fileLogger = fileLogger;
    }


    private async Task ProcessFileAsync(string filePath)
    {
        const int maxRetries = 10;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                if (!File.Exists(filePath))
                    return;

                // Check whether the file is ready.
                using (FileStream stream = new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.ReadWrite,
                    FileShare.None))
                {
                }

                // Generate a unique filename.
                string extension = Path.GetExtension(filePath);
                string newFileName = Guid.NewGuid().ToString("N") + extension;

                string destinationPath = Path.Combine(
                    _destinationFolder,
                    newFileName);

                // Move and rename the file.
                File.Move(filePath, destinationPath);

                _logger.LogInformation(
                    "File processed successfully: {FileName}", newFileName);

                await _fileLogger.LogInformationAsync(
                    $"File processed successfully: {destinationPath}");

                return;
            }
            catch (IOException ex)
            {
                _logger.LogWarning(
                    "File is not ready. Attempt {Attempt}: {Message}",
                    attempt, ex.Message);

                await _fileLogger.LogWarningAsync(
                    $"File is not ready. Attempt {attempt}: {ex.Message}");
            }

            await Task.Delay(1000);
        }

        _logger.LogError("File processing failed after {Retries} attempts: {File}", maxRetries, filePath);

        await _fileLogger.LogErrorAsync(
            $"File processing failed after {maxRetries} attempts: {filePath}");
    }
    public void StartWatching(
        string sourceFolder,
        string destinationFolder)
    {
        _destinationFolder = destinationFolder;

        _watcher = new FileSystemWatcher(sourceFolder)
        {
            Filter = "*.*",
            NotifyFilter =
                NotifyFilters.FileName |
                NotifyFilters.CreationTime,
            EnableRaisingEvents = false
        };

        _watcher.Created += OnFileCreated;
        _watcher.EnableRaisingEvents = true;

        _logger.LogInformation(
            "Started monitoring: {Folder}", sourceFolder);

        _ = _fileLogger.LogInformationAsync(
            $"Monitoring started. Source: {sourceFolder}");
    }

    private void OnFileCreated(
        object sender,
        FileSystemEventArgs e)
    {
        if (!_processingFiles.TryAdd(e.FullPath, 0))
            return;

        _logger.LogInformation(
            "New file detected: {FileName}", e.Name);

        _ = ProcessFileAsync(e.FullPath);
    }

    public void Dispose()
    {
        _watcher?.Dispose();
    }
}

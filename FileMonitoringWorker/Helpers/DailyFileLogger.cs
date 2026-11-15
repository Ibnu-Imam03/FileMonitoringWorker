
using FileMonitoringWorker.Configuration;
using Microsoft.Extensions.Options;

namespace FileMonitoringWorker.Helpers;

public class DailyFileLogger
{
    private readonly string _logFolder;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public DailyFileLogger(IOptions<FileMonitorSettings> options)
    {
        _logFolder = options.Value.LogFolder;
    }

    public async Task LogInformationAsync(string message)
    {
        await WriteLogAsync("INFO", message);
    }

    public async Task LogWarningAsync(string message)
    {
        await WriteLogAsync("WARNING", message);
    }

    public async Task LogErrorAsync(string message, Exception? exception = null)
    {
        string details = exception == null ? message : $"{message} | Exception: {exception}";

        await WriteLogAsync("ERROR", details);
    }

    private async Task WriteLogAsync(string level, string message)
    {
        await _semaphore.WaitAsync();

        try
        {
            Directory.CreateDirectory(_logFolder);

            string fileName = $"{DateTime.Now:yyyy-MM-dd}.log";

            string filePath = Path.Combine(_logFolder, fileName);

            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";

            await File.AppendAllTextAsync(filePath, logEntry);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}

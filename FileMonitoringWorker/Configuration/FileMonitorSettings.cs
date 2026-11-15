
namespace FileMonitoringWorker.Configuration;

public class FileMonitorSettings
{
    public string SourceFolder { get; set; } = string.Empty;

    public string DestinationFolder { get; set; } = string.Empty;

    public string LogFolder { get; set; } = string.Empty;
}

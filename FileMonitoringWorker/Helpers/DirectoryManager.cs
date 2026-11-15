
namespace FileMonitoringWorker.Helpers;

public class DirectoryManager
{
    public static void InitializeDirectories(string sourceFolder,string destinationFolder,string logFolder)
    {
        Directory.CreateDirectory(sourceFolder);
        Directory.CreateDirectory(destinationFolder);
        Directory.CreateDirectory(logFolder);
    }


}

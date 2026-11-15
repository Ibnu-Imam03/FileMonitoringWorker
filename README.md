# File Monitoring Worker Service

A background file monitoring application built with C# and .NET 10 Worker Service. It monitors a configured source directory, detects newly created files, renames them using unique GUIDs, and moves them to a destination directory.

## Features

- Background execution using .NET Worker Service.
- Automatic directory initialization.
- Real-time file monitoring using `FileSystemWatcher`.
- Unique file naming using GUIDs.
- Automatic file movement to a destination directory.
- Retry mechanism for files that are temporarily unavailable.
- Daily log files with information, warning, and error messages.
- Windows Service integration.

## Technologies

- C#
- .NET 10
- .NET Worker Service
- Windows Services
- FileSystemWatcher
- Microsoft.Extensions.Hosting
- Dependency Injection
- JSON Configuration

## Project Structure

```text
FileMonitoringWorker/
│
├── Configuration/
│   └── FileMonitorSettings.cs
│
├── Helpers/
│   ├── DirectoryManager.cs
│   └── DailyFileLogger.cs
│
├── Services/
│   └── FileMonitorService.cs
│
├── Worker.cs
├── Program.cs
└── appsettings.json
```

## Configuration

Configure the monitoring directories in `appsettings.json`:

```json
{
  "FileMonitor": {
    "SourceFolder": "C:\\FileMonitor\\Incoming",
    "DestinationFolder": "C:\\FileMonitor\\Processed",
    "LogFolder": "C:\\FileMonitor\\Logs"
  }
}
```

The application automatically creates these directories if they do not exist.

## How It Works

1. The Worker Service starts in the background.
2. Required directories are initialized.
3. `FileSystemWatcher` monitors the source directory.
4. When a new file is detected, the service attempts to process it.
5. A GUID is generated while preserving the original extension.
6. The file is moved to the destination directory.
7. Processing activities are recorded in daily log files.

## Windows Service Deployment

Build the project in Release configuration:

```powershell
dotnet build -c Release
```

Run Command Prompt as Administrator.

### Install the service

Replace the executable path with your actual project path:

```cmd
sc.exe create FileMonitoringWorker binPath= "\"C:\Path\To\FileMonitoringWorker.exe\"" start= auto
```

### Start the service

```cmd
sc.exe start FileMonitoringWorker
```

### Stop the service

```cmd
sc.exe stop FileMonitoringWorker
```

### Check service status

```cmd
sc.exe query FileMonitoringWorker
```

### Uninstall the service

Stop it first, then execute:

```cmd
sc.exe delete FileMonitoringWorker
```

## Logging

Daily log files are stored in the configured log directory.

Example:

```text
C:\FileMonitor\Logs\
    2026-11-15.log
    2026-11-16.log
```

## Requirements

- Windows 10 or later.
- .NET 10 Runtime for framework-dependent deployment.
- Administrator privileges for Windows Service installation.
- Appropriate folder permissions for the service account.

## Future Improvements

- More robust file-copy completion detection.
- Recovery of files after application restarts.
- Improved duplicate-event handling.
- Monitoring and recovery after FileSystemWatcher errors.

## Author

**Abubakar Yimam**

GitHub: [Ibnu-Imam03](https://github.com/Ibnu-Imam03)

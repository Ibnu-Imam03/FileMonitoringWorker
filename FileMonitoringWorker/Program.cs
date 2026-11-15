
using FileMonitoringWorker;
using FileMonitoringWorker.Configuration;
using FileMonitoringWorker.Helpers;
using FileMonitoringWorker.Services;

var builder = Host.CreateApplicationBuilder(args);

// Configure Windows Service
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "FileMonitoringWorker";
});

// Configure application settings
builder.Services.Configure<FileMonitorSettings>(
    builder.Configuration.GetSection("FileMonitor"));

// Register dependencies
builder.Services.AddSingleton<DailyFileLogger>();
builder.Services.AddSingleton<FileMonitorService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();


using FileMonitoringWorker;
using FileMonitoringWorker.Configuration;

var builder = Host.CreateApplicationBuilder(args);

// Bind appsettings.json to FileMonitorSettings
builder.Services.Configure<FileMonitorSettings>(builder.Configuration.GetSection("FileMonitor"));

// Register Worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();

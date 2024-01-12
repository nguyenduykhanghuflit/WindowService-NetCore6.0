using WorkService.NetCore6._0.Workers;

using Serilog;
using WorkService.NetCore6._0.Helpers;

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .UseWindowsService(o =>
    {
        o.ServiceName = "My service";
    })
    .ConfigureServices((hostContext, services) =>
    {
        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(hostContext.Configuration).CreateLogger();
        services.AddHostedService<Worker>();
        services.AddSingleton(hostContext.Configuration);
        services.AddTransient<SqlHelper>();
    })
    .Build();

await host.RunAsync();

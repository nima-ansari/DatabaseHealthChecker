using DatabaseHealthChecker.Interfaces;
using DatabaseHealthChecker.Models;
using DatabaseHealthChecker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Application started...");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<AppSettings>(context.Configuration.GetSection("AppSettings"));
        services.AddSingleton<ILoggerService, FileLoggerService>();
        services.AddHostedService<DatabaseHealthCheckerService>();
    })
    .Build();

await host.RunAsync();
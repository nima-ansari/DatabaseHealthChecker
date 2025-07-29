using DatabaseHealthChecker.Common.Models;
using DatabaseHealthChecker.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Application started...");

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<AppSettings>(context.Configuration.GetSection("AppSettings"));
        services.AddSingleton<FileLoggerService>();
        services.AddHostedService<DatabaseHealthCheckerService>();
    })
    .Build();

await host.RunAsync();

Console.ReadLine();
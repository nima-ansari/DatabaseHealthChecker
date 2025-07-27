using DatabaseHealthChecker.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;

namespace DatabaseHealthChecker.Services;

public class DatabaseHealthCheckerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AppSettings _settings;

    public DatabaseHealthCheckerService(IServiceProvider serviceProvider, IOptions<AppSettings> options)
    {
        _serviceProvider = serviceProvider;
        _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scopeDebug = _serviceProvider.CreateScope();
        var _fileLoggerService = scopeDebug.ServiceProvider.GetRequiredService<FileLoggerService>();

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await GetConnectionStatusesMessageAsync(cancellationToken);
                await _fileLoggerService.LogAsync(result, "Ok");

                await Task.Delay(TimeSpan.FromMinutes(_settings.HealthCheckIntervalMinutes), cancellationToken);
            }

            await _fileLoggerService.LogAsync("cancellation token requested", "Cancelation");
        }
        catch (Exception ex)
        {
            await _fileLoggerService.LogAsync(GetExceptionDetails(ex), "Error");
        }

    }

    private async Task<string> GetConnectionStatusesMessageAsync(CancellationToken cancellationToken)
    {
        var results = new List<string>();
        foreach (var (name, connStr) in _settings.ConnectionStrings)
        {
            string status = await CheckConnectionAsync(connStr) ? "Success" : "Unsuccess";
            results.Add($"Connection \"{name}\": {status}");
        }

        return string.Join(Environment.NewLine, results);
    }

    private async Task<bool> CheckConnectionAsync(string connectionString)
    {
        try
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string GetExceptionDetails(Exception ex)
    {
        var lines = new List<string>
        {
            $"Exception Type: {ex.GetType().FullName}",
            $"Message: {ex.Message}",
            $"StackTrace: {ex.StackTrace}"
        };

        if (ex.InnerException is not null)
        {
            lines.Add("");
            lines.Add("Inner Exception:");
            lines.Add(GetExceptionDetails(ex.InnerException));
        }

        return string.Join(Environment.NewLine, lines);
    }
}
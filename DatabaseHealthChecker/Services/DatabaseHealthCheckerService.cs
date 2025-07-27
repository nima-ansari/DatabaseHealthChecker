using DatabaseHealthChecker.Interfaces;
using DatabaseHealthChecker.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DatabaseHealthChecker.Services;

public class DatabaseHealthCheckerService : BackgroundService
{
    private readonly AppSettings _settings;
    private readonly ILoggerService _loggerService;

    public DatabaseHealthCheckerService(IOptions<AppSettings> options, ILoggerService loggerService)
    {
        _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _loggerService = loggerService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await LogConnectionStatusesAsync(cancellationToken);
            await Task.Delay(TimeSpan.FromMinutes(_settings.HealthCheckIntervalMinutes), cancellationToken);
        }
    }

    private async Task LogConnectionStatusesAsync(CancellationToken cancellationToken)
    {
        var results = new List<string>();
        foreach (var (name, connStr) in _settings.ConnectionStrings)
        {
            string status = await CheckConnectionAsync(connStr) ? "Success" : "Unsuccess";
            results.Add($"Connection \"{name}\": {status}");
        }
        await _loggerService.LogAsync(results, cancellationToken);
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
}
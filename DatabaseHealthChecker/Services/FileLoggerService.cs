using DatabaseHealthChecker.Interfaces;
using DatabaseHealthChecker.Models;
using Microsoft.Extensions.Options;

namespace DatabaseHealthChecker.Services;

public class FileLoggerService : ILoggerService
{
    private readonly AppSettings _settings;

    public FileLoggerService(IOptions<AppSettings> options)
    {
        _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));

        if (!Directory.Exists(_settings.LogDirectoryPath))
            Directory.CreateDirectory(_settings.LogDirectoryPath);
    }

    public async Task LogAsync(string messages, string? suffixFileName = default, CancellationToken cancellationToken = default)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
        var fileName = $"{timestamp} {suffixFileName}.txt";
        var filePath = Path.Combine(_settings.LogDirectoryPath, fileName);

        await File.WriteAllTextAsync(filePath, messages, cancellationToken);
    }
}
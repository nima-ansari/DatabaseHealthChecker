namespace DatabaseHealthChecker.Common.Interfaces;

public interface ILoggerService
{
    Task LogAsync(string message, string? suffixFileName = default, CancellationToken cancellationToken = default);
}

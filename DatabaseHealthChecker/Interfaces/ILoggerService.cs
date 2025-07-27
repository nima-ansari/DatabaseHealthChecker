namespace DatabaseHealthChecker.Interfaces;

public interface ILoggerService
{
    Task LogAsync(IEnumerable<string> messages, CancellationToken cancellationToken = default);
}

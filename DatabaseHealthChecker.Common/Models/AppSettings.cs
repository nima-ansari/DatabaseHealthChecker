namespace DatabaseHealthChecker.Common.Models;

public class AppSettings
{
    public Dictionary<string, string> ConnectionStrings { get; set; } = new();
    public int HealthCheckIntervalMinutes { get; set; } 
    public required string LogDirectoryPath { get; set; }
}

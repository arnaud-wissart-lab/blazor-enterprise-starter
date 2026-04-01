namespace BlazorEnterpriseStarter.Shared.Contracts;

public sealed record ApplicationStatusDto(
    string ApplicationName,
    string EnvironmentName,
    bool IsHealthy,
    string Message,
    DateTimeOffset CheckedAtUtc)
{
    public static ApplicationStatusDto CreateHealthy(string applicationName, string environmentName, string message) =>
        new(applicationName, environmentName, true, message, DateTimeOffset.UtcNow);

    public static ApplicationStatusDto CreateUnavailable(string applicationName, string environmentName, string message) =>
        new(applicationName, environmentName, false, message, DateTimeOffset.UtcNow);
}

using System.Net;
using System.Net.Sockets;

namespace BlazorEnterpriseStarter.E2ETests.Infrastructure;

/// <summary>
/// Orchestre le backend et l’application Blazor pour les scénarios E2E.
/// </summary>
public sealed class E2ETestHostFixture : IAsyncLifetime
{
    private string? _databasePath;
    private ManagedDotNetApp? _server;
    private ManagedDotNetApp? _app;

    public Uri AppBaseAddress => _app?.BaseAddress ?? throw new InvalidOperationException("L’application n’est pas démarrée.");

    public Task InitializeAsync() => StartAsync(CancellationToken.None);

    public async Task DisposeAsync()
    {
        if (_app is not null)
        {
            await _app.DisposeAsync();
        }

        if (_server is not null)
        {
            await _server.DisposeAsync();
        }

        if (!string.IsNullOrWhiteSpace(_databasePath))
        {
            DeleteTempDirectory(Path.GetDirectoryName(_databasePath));
        }
    }
    private async Task StartAsync(CancellationToken cancellationToken)
    {
        var repositoryRoot = RepositoryPaths.ResolveRepositoryRoot();
        var tempDirectory = Path.Combine(Path.GetTempPath(), "bes-e2e", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDirectory);
        _databasePath = Path.Combine(tempDirectory, "backlog-e2e.db");

        var serverPort = GetFreeTcpPort();
        var appPort = GetFreeTcpPort();

        _server = new ManagedDotNetApp(
            name: "server",
            projectPath: Path.Combine(repositoryRoot, "src", "BlazorEnterpriseStarter.Server", "BlazorEnterpriseStarter.Server.csproj"),
            workingDirectory: repositoryRoot,
            readinessPath: "/health",
            environmentVariables: new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = "Development",
                ["DOTNET_RUNNING_IN_CONTAINER"] = "true",
                ["ConnectionStrings__BacklogDatabase"] = $"Data Source={_databasePath}"
            });

        await _server.StartAsync(serverPort, cancellationToken);

        _app = new ManagedDotNetApp(
            name: "app",
            projectPath: Path.Combine(repositoryRoot, "src", "BlazorEnterpriseStarter.App", "BlazorEnterpriseStarter.App.csproj"),
            workingDirectory: repositoryRoot,
            readinessPath: "/backlog",
            environmentVariables: new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = "Development",
                ["DOTNET_RUNNING_IN_CONTAINER"] = "true",
                ["PlatformApi__BaseUrl"] = _server.BaseAddress.ToString().TrimEnd('/')
            });

        await _app.StartAsync(appPort, cancellationToken);
    }

    private static int GetFreeTcpPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();

        try
        {
            return ((IPEndPoint)listener.LocalEndpoint).Port;
        }
        finally
        {
            listener.Stop();
        }
    }

    private static void DeleteTempDirectory(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
        {
            return;
        }

        try
        {
            Directory.Delete(path, recursive: true);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}

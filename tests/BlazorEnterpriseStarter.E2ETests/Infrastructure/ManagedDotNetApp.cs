using System.Diagnostics;
using System.Net.Http;
using System.Text;

namespace BlazorEnterpriseStarter.E2ETests.Infrastructure;

/// <summary>
/// Démarre et arrête un projet ASP.NET Core pour un scénario E2E.
/// </summary>
internal sealed class ManagedDotNetApp : IAsyncDisposable
{
    private readonly string _name;
    private readonly string _projectPath;
    private readonly string _workingDirectory;
    private readonly string _readinessPath;
    private readonly IDictionary<string, string?> _environmentVariables;
    private readonly List<string> _logs = [];
    private Process? _process;

    public ManagedDotNetApp(
        string name,
        string projectPath,
        string workingDirectory,
        string readinessPath,
        IDictionary<string, string?> environmentVariables)
    {
        _name = name;
        _projectPath = projectPath;
        _workingDirectory = workingDirectory;
        _readinessPath = readinessPath;
        _environmentVariables = environmentVariables;
    }

    public Uri BaseAddress { get; private set; } = default!;

    public async Task StartAsync(int port, CancellationToken cancellationToken)
    {
        BaseAddress = new Uri($"http://127.0.0.1:{port}");

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{_projectPath}\" --no-launch-profile --urls {BaseAddress}",
            WorkingDirectory = _workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var entry in _environmentVariables)
        {
            startInfo.Environment[entry.Key] = entry.Value;
        }

        _process = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };

        _process.OutputDataReceived += (_, args) =>
        {
            if (!string.IsNullOrWhiteSpace(args.Data))
            {
                lock (_logs)
                {
                    _logs.Add($"[{_name}:out] {args.Data}");
                }
            }
        };

        _process.ErrorDataReceived += (_, args) =>
        {
            if (!string.IsNullOrWhiteSpace(args.Data))
            {
                lock (_logs)
                {
                    _logs.Add($"[{_name}:err] {args.Data}");
                }
            }
        };

        if (!_process.Start())
        {
            throw new InvalidOperationException($"Impossible de démarrer le processus {_name}.");
        }

        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        await WaitUntilReadyAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_process is null)
        {
            return;
        }

        try
        {
            if (!_process.HasExited)
            {
                _process.Kill(entireProcessTree: true);
                await _process.WaitForExitAsync();
            }
        }
        catch (InvalidOperationException)
        {
        }
        finally
        {
            _process.Dispose();
        }
    }

    private async Task WaitUntilReadyAsync(CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(2)
        };

        var readinessUri = new Uri(BaseAddress, _readinessPath);
        var timeoutAt = DateTimeOffset.UtcNow.AddSeconds(45);

        while (DateTimeOffset.UtcNow < timeoutAt)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_process?.HasExited == true)
            {
                throw new InvalidOperationException(
                    $"Le processus {_name} s’est arrêté avant d’être prêt.{Environment.NewLine}{DumpLogs()}");
            }

            try
            {
                using var response = await httpClient.GetAsync(readinessUri, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            await Task.Delay(500, cancellationToken);
        }

        throw new TimeoutException($"Le processus {_name} n’a pas répondu sur {readinessUri}.{Environment.NewLine}{DumpLogs()}");
    }

    private string DumpLogs()
    {
        lock (_logs)
        {
            return string.Join(Environment.NewLine, _logs.TakeLast(30));
        }
    }
}

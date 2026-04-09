namespace BlazorEnterpriseStarter.E2ETests.Infrastructure;

/// <summary>
/// Centralise la résolution des chemins du dépôt utilisés par les scénarios E2E.
/// </summary>
internal static class RepositoryPaths
{
    public static string ResolveRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "BlazorEnterpriseStarter.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Impossible de retrouver la racine du dépôt pour les tests E2E.");
    }

    public static string GetScreenshotsDirectory() =>
        Path.Combine(ResolveRepositoryRoot(), "docs", "screenshots");
}

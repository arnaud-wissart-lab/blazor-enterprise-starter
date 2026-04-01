namespace BlazorEnterpriseStarter.App.Services;

/// <summary>
/// Représente une erreur métier ou HTTP remontée par l’API backlog.
/// </summary>
public sealed class BacklogApiException(string message, IReadOnlyDictionary<string, string[]>? erreurs = null) : Exception(message)
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } =
        erreurs ?? new Dictionary<string, string[]>();
}

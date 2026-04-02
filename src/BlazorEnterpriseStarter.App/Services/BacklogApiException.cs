namespace BlazorEnterpriseStarter.App.Services;

/// <summary>
/// Représente une erreur métier ou HTTP remontée par l’API backlog.
/// </summary>
public sealed class BacklogApiException(string message, IReadOnlyDictionary<string, string[]>? erreurs = null) : Exception(message)
{
    private static readonly IReadOnlyDictionary<string, string[]> ErreursVides =
        new Dictionary<string, string[]>();

    public IReadOnlyDictionary<string, string[]> Errors { get; } =
        erreurs ?? ErreursVides;
}

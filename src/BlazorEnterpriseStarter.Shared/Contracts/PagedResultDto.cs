namespace BlazorEnterpriseStarter.Shared.Contracts;

/// <summary>
/// Représente une réponse paginée générique.
/// </summary>
/// <typeparam name="T">Type des éléments transportés.</typeparam>
public sealed record PagedResultDto<T>(
    IReadOnlyList<T> Elements,
    int NombreTotal,
    int NumeroPage,
    int TaillePage)
{
    public int NombrePages =>
        TaillePage <= 0
            ? 0
            : (int)Math.Ceiling((double)NombreTotal / TaillePage);
}

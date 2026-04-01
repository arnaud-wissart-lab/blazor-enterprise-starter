using System.Net.Http.Json;
using BlazorEnterpriseStarter.Shared.Contracts;

namespace BlazorEnterpriseStarter.App.Services;

public sealed class PlatformApiClient(HttpClient httpClient, ILogger<PlatformApiClient> logger)
{
    public async Task<ApplicationStatusDto> GetApplicationStatusAsync(CancellationToken cancellationToken)
    {
        try
        {
            var status = await httpClient.GetFromJsonAsync<ApplicationStatusDto>(ApiRoutes.System.Status, cancellationToken);

            return status ?? ApplicationStatusDto.CreateUnavailable(
                "BlazorEnterpriseStarter.Server",
                "Indéterminé",
                "L’API a répondu sans fournir de contenu exploitable.");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return ApplicationStatusDto.CreateUnavailable(
                "BlazorEnterpriseStarter.Server",
                "Interrompu",
                "La vérification de l’état a été interrompue avant la fin de l’appel.");
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Impossible de récupérer l’état de la plateforme.");

            return ApplicationStatusDto.CreateUnavailable(
                "BlazorEnterpriseStarter.Server",
                "Indisponible",
                "Impossible de joindre l’API backend pour le moment.");
        }
    }
}

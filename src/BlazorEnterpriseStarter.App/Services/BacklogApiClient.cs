using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BlazorEnterpriseStarter.Shared.Contracts;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEnterpriseStarter.App.Services;

/// <summary>
/// Centralise l’accès HTTP au module backlog exposé par l’API.
/// </summary>
public sealed class BacklogApiClient(HttpClient httpClient)
{
    public async Task<PagedResultDto<BacklogItemDto>> ListerAsync(BacklogItemsQueryDto requete, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(ConstruireUriListe(requete), cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await CreerExceptionAsync(response, cancellationToken);
        }

        var resultat = await response.Content.ReadFromJsonAsync<PagedResultDto<BacklogItemDto>>(cancellationToken);

        return resultat ?? new PagedResultDto<BacklogItemDto>(
            [],
            0,
            requete.NumeroPage,
            requete.TaillePage);
    }

    public async Task<BacklogItemDto> CreerAsync(BacklogItemUpsertRequest commande, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(ApiRoutes.Backlog.Base, commande, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await CreerExceptionAsync(response, cancellationToken);
        }

        var item = await response.Content.ReadFromJsonAsync<BacklogItemDto>(cancellationToken);
        return item ?? throw new BacklogApiException("L’API n’a pas renvoyé l’élément créé.");
    }

    public async Task<BacklogItemDto> ModifierAsync(Guid id, BacklogItemUpsertRequest commande, CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"{ApiRoutes.Backlog.Base}/{id}", commande, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await CreerExceptionAsync(response, cancellationToken);
        }

        var item = await response.Content.ReadFromJsonAsync<BacklogItemDto>(cancellationToken);
        return item ?? throw new BacklogApiException("L’API n’a pas renvoyé l’élément modifié.");
    }

    public async Task SupprimerAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"{ApiRoutes.Backlog.Base}/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await CreerExceptionAsync(response, cancellationToken);
        }
    }

    private static string ConstruireUriListe(BacklogItemsQueryDto requete)
    {
        var query = new List<string>
        {
            $"{nameof(requete.NumeroPage)}={requete.NumeroPage}",
            $"{nameof(requete.TaillePage)}={requete.TaillePage}",
            $"{nameof(requete.Tri)}={Uri.EscapeDataString(requete.Tri.ToString())}",
            $"{nameof(requete.Direction)}={Uri.EscapeDataString(requete.Direction.ToString())}"
        };

        if (!string.IsNullOrWhiteSpace(requete.Recherche))
        {
            query.Add($"{nameof(requete.Recherche)}={Uri.EscapeDataString(requete.Recherche)}");
        }

        if (requete.Statut is not null)
        {
            query.Add($"{nameof(requete.Statut)}={Uri.EscapeDataString(requete.Statut.Value.ToString())}");
        }

        if (requete.Priorite is not null)
        {
            query.Add($"{nameof(requete.Priorite)}={Uri.EscapeDataString(requete.Priorite.Value.ToString())}");
        }

        return $"{ApiRoutes.Backlog.Base}?{string.Join("&", query)}";
    }

    private static async Task<BacklogApiException> CreerExceptionAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new BacklogApiException("La ressource backlog demandée est introuvable.");
        }

        if (response.Content.Headers.ContentLength == 0)
        {
            return new BacklogApiException("L’API a renvoyé une réponse vide inattendue.");
        }

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);

        try
        {
            var validation = JsonSerializer.Deserialize<HttpValidationProblemDetails>(payload);

            if (validation?.Errors is { Count: > 0 })
            {
                return new BacklogApiException(
                    validation.Title ?? "La validation du formulaire a échoué.",
                    validation.Errors.ToDictionary(entry => entry.Key, entry => entry.Value));
            }
        }
        catch (JsonException)
        {
            // Un autre format de problème sera tenté juste après.
        }

        try
        {
            var problem = JsonSerializer.Deserialize<ProblemDetails>(payload);

            if (!string.IsNullOrWhiteSpace(problem?.Detail) || !string.IsNullOrWhiteSpace(problem?.Title))
            {
                return new BacklogApiException(problem.Detail ?? problem.Title!);
            }
        }
        catch (JsonException)
        {
            // Rien à faire, un message de repli sera fourni.
        }

        return new BacklogApiException("Une erreur inattendue est survenue lors de l’appel au backend backlog.");
    }
}

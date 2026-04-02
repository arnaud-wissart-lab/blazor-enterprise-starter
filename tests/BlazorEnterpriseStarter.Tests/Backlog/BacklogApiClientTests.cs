using System.Net;
using System.Text;
using BlazorEnterpriseStarter.App.Services;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;

namespace BlazorEnterpriseStarter.Tests.Backlog;

public class BacklogApiClientTests
{
    [Fact]
    public async Task CreerAsync_devrait_exposer_les_erreurs_de_validation_issues_du_payload_problem_details()
    {
        var client = CreerClient(
            HttpStatusCode.BadRequest,
            """
            {
              "title": "La validation a échoué.",
              "errors": {
                "Titre": [
                  "Le titre est obligatoire."
                ]
              }
            }
            """);

        var exception = await Assert.ThrowsAsync<BacklogApiException>(() =>
            client.CreerAsync(
                new BacklogItemUpsertRequest
                {
                    Titre = string.Empty,
                    Description = "Description invalide."
                },
                CancellationToken.None));

        Assert.Equal("La validation a échoué.", exception.Message);
        Assert.True(exception.Errors.ContainsKey("Titre"));
    }

    [Fact]
    public async Task ModifierAsync_devrait_recuperer_le_detail_d_un_problem_details_camel_case()
    {
        var client = CreerClient(
            HttpStatusCode.Conflict,
            """
            {
              "title": "Conflit métier",
              "detail": "L’élément a été modifié par un autre traitement."
            }
            """);

        var exception = await Assert.ThrowsAsync<BacklogApiException>(() =>
            client.ModifierAsync(
                Guid.NewGuid(),
                new BacklogItemUpsertRequest
                {
                    Titre = "Mettre à jour la démonstration",
                    Description = "Description valide.",
                    Statut = BacklogItemStatus.EnCours,
                    Priorite = BacklogItemPriority.Haute
                },
                CancellationToken.None));

        Assert.Equal("L’élément a été modifié par un autre traitement.", exception.Message);
    }

    private static BacklogApiClient CreerClient(HttpStatusCode codeRetour, string contenu)
    {
        var httpClient = new HttpClient(new FakeHttpMessageHandler(codeRetour, contenu))
        {
            BaseAddress = new Uri("https://api.local")
        };

        return new BacklogApiClient(httpClient);
    }

    private sealed class FakeHttpMessageHandler(HttpStatusCode codeRetour, string contenu) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(codeRetour)
            {
                Content = new StringContent(contenu, Encoding.UTF8, "application/problem+json")
            });
    }
}

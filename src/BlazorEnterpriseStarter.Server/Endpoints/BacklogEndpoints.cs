using BlazorEnterpriseStarter.Server.Application.Backlog;
using BlazorEnterpriseStarter.Shared.Contracts;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEnterpriseStarter.Server.Endpoints;

/// <summary>
/// Expose les endpoints REST du module backlog produit.
/// </summary>
public static class BacklogEndpoints
{
    public static IEndpointRouteBuilder MapBacklogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(ApiRoutes.Backlog.Base)
            .WithTags("Backlog");

        group.MapGet(string.Empty, async (
            [AsParameters] BacklogItemsQueryDto requete,
            IBacklogService service,
            CancellationToken cancellationToken) =>
        {
            var erreurs = BacklogValidation.ValiderRequete(requete);

            if (erreurs is not null)
            {
                return Results.ValidationProblem(erreurs);
            }

            var resultat = await service.ListerAsync(requete, cancellationToken);
            return Results.Ok(resultat);
        })
        .WithName("ListerBacklogItems");

        group.MapGet("{id:guid}", async (
            Guid id,
            IBacklogService service,
            CancellationToken cancellationToken) =>
        {
            var item = await service.ObtenirParIdAsync(id, cancellationToken);
            return item is null ? Results.NotFound() : Results.Ok(item);
        })
        .WithName("ObtenirBacklogItem");

        group.MapPost(string.Empty, async (
            BacklogItemUpsertRequest commande,
            IBacklogService service,
            CancellationToken cancellationToken) =>
        {
            var erreurs = BacklogValidation.ValiderCommande(commande);

            if (erreurs is not null)
            {
                return Results.ValidationProblem(erreurs);
            }

            var item = await service.CreerAsync(commande, cancellationToken);
            return Results.Created($"{ApiRoutes.Backlog.Base}/{item.Id}", item);
        })
        .WithName("CreerBacklogItem");

        group.MapPut("{id:guid}", async (
            Guid id,
            BacklogItemUpsertRequest commande,
            IBacklogService service,
            CancellationToken cancellationToken) =>
        {
            var erreurs = BacklogValidation.ValiderCommande(commande);

            if (erreurs is not null)
            {
                return Results.ValidationProblem(erreurs);
            }

            var item = await service.ModifierAsync(id, commande, cancellationToken);
            return item is null ? Results.NotFound() : Results.Ok(item);
        })
        .WithName("ModifierBacklogItem");

        group.MapDelete("{id:guid}", async (
            Guid id,
            IBacklogService service,
            CancellationToken cancellationToken) =>
        {
            var supprime = await service.SupprimerAsync(id, cancellationToken);
            return supprime ? Results.NoContent() : Results.NotFound();
        })
        .WithName("SupprimerBacklogItem");

        return endpoints;
    }
}

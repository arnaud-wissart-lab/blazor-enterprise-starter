using BlazorEnterpriseStarter.Server.Application.Backlog;
using BlazorEnterpriseStarter.Server.Infrastructure.Backlog;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;

namespace BlazorEnterpriseStarter.Tests.Backlog;

public class BacklogServiceTests
{
    [Fact]
    public async Task ListerAsync_devrait_filtrer_et_paginer_les_elements()
    {
        var service = CreerService();

        var resultat = await service.ListerAsync(
            new BacklogItemsQueryDto
            {
                Recherche = "backlog",
                Statut = BacklogItemStatus.Nouveau,
                NumeroPage = 1,
                TaillePage = 5
            },
            CancellationToken.None);

        var item = Assert.Single(resultat.Elements);
        Assert.Equal(1, resultat.NombreTotal);
        Assert.Equal("Ajouter la vue backlog produit", item.Titre);
    }

    [Fact]
    public async Task CreerAsync_devrait_ajouter_un_nouvel_element()
    {
        var service = CreerService();

        var cree = await service.CreerAsync(
            new BacklogItemUpsertRequest
            {
                Titre = "Préparer la démonstration client",
                Description = "Construire un parcours simple pour montrer le backlog en réunion.",
                Statut = BacklogItemStatus.Pret,
                Priorite = BacklogItemPriority.Haute
            },
            CancellationToken.None);

        var resultat = await service.ListerAsync(
            new BacklogItemsQueryDto
            {
                Recherche = "démonstration client",
                NumeroPage = 1,
                TaillePage = 10
            },
            CancellationToken.None);

        Assert.Equal("Préparer la démonstration client", cree.Titre);
        Assert.Contains(resultat.Elements, item => item.Id == cree.Id);
    }

    [Fact]
    public async Task ModifierAsync_devrait_mettre_a_jour_un_element_existant()
    {
        var service = CreerService();
        var elementExistant = (await service.ListerAsync(new BacklogItemsQueryDto(), CancellationToken.None)).Elements.First();

        var modifie = await service.ModifierAsync(
            elementExistant.Id,
            new BacklogItemUpsertRequest
            {
                Titre = "Backlog mis à jour",
                Description = "Description actualisée pour le test.",
                Statut = BacklogItemStatus.EnCours,
                Priorite = BacklogItemPriority.Critique
            },
            CancellationToken.None);

        Assert.NotNull(modifie);
        Assert.Equal("Backlog mis à jour", modifie!.Titre);
        Assert.Equal(BacklogItemPriority.Critique, modifie.Priorite);
    }

    private static IBacklogService CreerService() =>
        new BacklogService(new InMemoryBacklogRepository());
}

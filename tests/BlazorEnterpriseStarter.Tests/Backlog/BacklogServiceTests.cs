using BlazorEnterpriseStarter.Server.Application.Backlog;
using BlazorEnterpriseStarter.Server.Domain.Backlog;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;
using BlazorEnterpriseStarter.Tests.Backlog.Fakes;

namespace BlazorEnterpriseStarter.Tests.Backlog;

public class BacklogServiceTests
{
    private static readonly DateTimeOffset BaseDate = new(2026, 03, 20, 09, 00, 00, TimeSpan.Zero);

    [Fact]
    public async Task ListerAsync_devrait_rechercher_dans_le_titre_et_la_description_sans_sensibilite_a_la_casse()
    {
        var service = CreerService(
            CreerItem("Concevoir le tableau de bord", "Vue de synthèse pour la direction produit.", BacklogItemStatus.EnCours, BacklogItemPriority.Haute, -1),
            CreerItem("Préparer la roadmap", "Un BACKLOG transverse est nécessaire pour la prochaine démonstration.", BacklogItemStatus.Pret, BacklogItemPriority.Moyenne, -2),
            CreerItem("Documenter les composants", "Clarifier l’usage du design system.", BacklogItemStatus.Termine, BacklogItemPriority.Basse, -3));

        var resultat = await service.ListerAsync(
            new BacklogItemsQueryDto
            {
                Recherche = "backlog",
                NumeroPage = 1,
                TaillePage = 10
            },
            CancellationToken.None);

        var element = Assert.Single(resultat.Elements);
        Assert.Equal("Préparer la roadmap", element.Titre);
        Assert.Equal(1, resultat.NombreTotal);
    }

    [Fact]
    public async Task ListerAsync_devrait_combiner_filtre_par_statut_priorite_et_pagination()
    {
        var service = CreerService(
            CreerItem("Préparer le sprint 1", "Premier lot.", BacklogItemStatus.Pret, BacklogItemPriority.Haute, -1),
            CreerItem("Préparer le sprint 2", "Deuxième lot.", BacklogItemStatus.Pret, BacklogItemPriority.Haute, -2),
            CreerItem("Préparer le sprint 3", "Troisième lot.", BacklogItemStatus.Pret, BacklogItemPriority.Haute, -3),
            CreerItem("Mettre à jour la navigation", "Hors filtre.", BacklogItemStatus.EnCours, BacklogItemPriority.Haute, -4));

        var resultat = await service.ListerAsync(
            new BacklogItemsQueryDto
            {
                Statut = BacklogItemStatus.Pret,
                Priorite = BacklogItemPriority.Haute,
                NumeroPage = 2,
                TaillePage = 1
            },
            CancellationToken.None);

        var element = Assert.Single(resultat.Elements);
        Assert.Equal(3, resultat.NombreTotal);
        Assert.Equal(2, resultat.NumeroPage);
        Assert.Equal("Préparer le sprint 2", element.Titre);
    }

    [Fact]
    public async Task ListerAsync_devrait_appliquer_le_tri_par_priorite_croissante()
    {
        var service = CreerService(
            CreerItem("Item critique", "Critique.", BacklogItemStatus.Nouveau, BacklogItemPriority.Critique, -1),
            CreerItem("Item basse", "Basse.", BacklogItemStatus.Nouveau, BacklogItemPriority.Basse, -2),
            CreerItem("Item haute", "Haute.", BacklogItemStatus.Nouveau, BacklogItemPriority.Haute, -3));

        var resultat = await service.ListerAsync(
            new BacklogItemsQueryDto
            {
                Tri = BacklogItemSortField.Priorite,
                Direction = DirectionTri.Croissante,
                NumeroPage = 1,
                TaillePage = 10
            },
            CancellationToken.None);

        Assert.Collection(
            resultat.Elements,
            item => Assert.Equal("Item basse", item.Titre),
            item => Assert.Equal("Item haute", item.Titre),
            item => Assert.Equal("Item critique", item.Titre));
    }

    [Fact]
    public async Task CreerAsync_devrait_supprimer_les_espaces_superflus_et_persister_l_element()
    {
        var service = CreerService();

        var cree = await service.CreerAsync(
            new BacklogItemUpsertRequest
            {
                Titre = "  Préparer la démonstration client  ",
                Description = "  Construire un parcours clair pour la revue produit.  ",
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

        var element = Assert.Single(resultat.Elements);
        Assert.Equal("Préparer la démonstration client", cree.Titre);
        Assert.Equal("Construire un parcours clair pour la revue produit.", cree.Description);
        Assert.Equal(cree.Id, element.Id);
    }

    [Fact]
    public async Task ModifierAsync_devrait_retourner_null_quand_l_element_est_introuvable()
    {
        var service = CreerService(CreerItem("Initial", "Description", BacklogItemStatus.Nouveau, BacklogItemPriority.Moyenne, -1));

        var resultat = await service.ModifierAsync(
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            new BacklogItemUpsertRequest
            {
                Titre = "Backlog mis à jour",
                Description = "Description actualisée pour le test.",
                Statut = BacklogItemStatus.EnCours,
                Priorite = BacklogItemPriority.Critique
            },
            CancellationToken.None);

        Assert.Null(resultat);
    }

    [Fact]
    public async Task SupprimerAsync_devrait_supprimer_l_element_existant()
    {
        var item = CreerItem("Élément à supprimer", "Suppression", BacklogItemStatus.Nouveau, BacklogItemPriority.Basse, -1);
        var service = CreerService(item);

        var supprime = await service.SupprimerAsync(item.Id, CancellationToken.None);
        var resultat = await service.ListerAsync(new BacklogItemsQueryDto { NumeroPage = 1, TaillePage = 10 }, CancellationToken.None);

        Assert.True(supprime);
        Assert.Empty(resultat.Elements);
    }

    private static IBacklogService CreerService(params BacklogItem[] items) =>
        new BacklogService(new FakeBacklogRepository(items));

    private static BacklogItem CreerItem(
        string titre,
        string description,
        BacklogItemStatus statut,
        BacklogItemPriority priorite,
        int decalageJours) =>
        new(
            Guid.NewGuid(),
            titre,
            description,
            statut,
            priorite,
            BaseDate.AddDays(decalageJours));
}

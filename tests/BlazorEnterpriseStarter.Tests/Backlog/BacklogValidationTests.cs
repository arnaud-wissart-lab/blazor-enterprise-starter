using BlazorEnterpriseStarter.Server.Application.Backlog;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;

namespace BlazorEnterpriseStarter.Tests.Backlog;

public class BacklogValidationTests
{
    [Fact]
    public void ValiderCommande_devrait_refuser_un_titre_avec_un_retour_ligne()
    {
        var commande = new BacklogItemUpsertRequest
        {
            Titre = "Titre\r\ninjecté",
            Description = "Description valide.",
            Statut = BacklogItemStatus.Nouveau,
            Priorite = BacklogItemPriority.Moyenne
        };

        var erreurs = BacklogValidation.ValiderCommande(commande);

        Assert.NotNull(erreurs);
        Assert.Contains(nameof(BacklogItemUpsertRequest.Titre), erreurs!.Keys);
    }

    [Fact]
    public void ValiderCommande_devrait_refuser_une_description_avec_un_caractere_de_controle()
    {
        var commande = new BacklogItemUpsertRequest
        {
            Titre = "Titre valide",
            Description = "Description\u0000invalide",
            Statut = BacklogItemStatus.Nouveau,
            Priorite = BacklogItemPriority.Moyenne
        };

        var erreurs = BacklogValidation.ValiderCommande(commande);

        Assert.NotNull(erreurs);
        Assert.Contains(nameof(BacklogItemUpsertRequest.Description), erreurs!.Keys);
    }

    [Fact]
    public void ValiderRequete_devrait_refuser_une_recherche_avec_un_caractere_de_controle()
    {
        var requete = new BacklogItemsQueryDto
        {
            Recherche = "pilotage\u0007",
            NumeroPage = 1,
            TaillePage = 6
        };

        var erreurs = BacklogValidation.ValiderRequete(requete);

        Assert.NotNull(erreurs);
        Assert.Contains(nameof(BacklogItemsQueryDto.Recherche), erreurs!.Keys);
    }
}

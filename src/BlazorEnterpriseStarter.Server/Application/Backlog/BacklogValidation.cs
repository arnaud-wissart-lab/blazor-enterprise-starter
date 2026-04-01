using BlazorEnterpriseStarter.Shared.Contracts.Backlog;

namespace BlazorEnterpriseStarter.Server.Application.Backlog;

/// <summary>
/// Centralise les règles de validation simples du module backlog.
/// </summary>
public static class BacklogValidation
{
    public static Dictionary<string, string[]>? ValiderRequete(BacklogItemsQueryDto requete)
    {
        var erreurs = new Dictionary<string, string[]>();
        var recherche = BacklogInputRules.NormaliserRecherche(requete.Recherche);

        if (requete.NumeroPage < 1)
        {
            erreurs[nameof(requete.NumeroPage)] = ["Le numéro de page doit être supérieur ou égal à 1."];
        }

        if (requete.TaillePage is < BacklogInputRules.TaillePageMinimale or > BacklogInputRules.TaillePageMaximale)
        {
            erreurs[nameof(requete.TaillePage)] =
                [$"La taille de page doit être comprise entre {BacklogInputRules.TaillePageMinimale} et {BacklogInputRules.TaillePageMaximale} éléments."];
        }

        if (BacklogInputRules.ContientDesCaracteresInterditsPourRecherche(requete.Recherche))
        {
            erreurs[nameof(requete.Recherche)] = ["La recherche contient des caractères non pris en charge."];
        }
        else if (recherche?.Length > BacklogInputRules.RechercheLongueurMaximale)
        {
            erreurs[nameof(requete.Recherche)] =
                [$"La recherche ne peut pas dépasser {BacklogInputRules.RechercheLongueurMaximale} caractères."];
        }

        return erreurs.Count == 0 ? null : erreurs;
    }

    public static Dictionary<string, string[]>? ValiderCommande(BacklogItemUpsertRequest commande)
    {
        var erreurs = new Dictionary<string, string[]>();
        var titre = BacklogInputRules.NormaliserTitre(commande.Titre);
        var description = BacklogInputRules.NormaliserDescription(commande.Description);

        if (string.IsNullOrWhiteSpace(titre))
        {
            erreurs[nameof(commande.Titre)] = ["Le titre est obligatoire."];
        }
        else if (BacklogInputRules.ContientDesCaracteresInterditsPourTitre(commande.Titre))
        {
            erreurs[nameof(commande.Titre)] = ["Le titre contient des caractères non pris en charge."];
        }
        else if (titre.Length > BacklogInputRules.TitreLongueurMaximale)
        {
            erreurs[nameof(commande.Titre)] =
                [$"Le titre ne peut pas dépasser {BacklogInputRules.TitreLongueurMaximale} caractères."];
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            erreurs[nameof(commande.Description)] = ["La description est obligatoire."];
        }
        else if (BacklogInputRules.ContientDesCaracteresInterditsPourDescription(commande.Description))
        {
            erreurs[nameof(commande.Description)] = ["La description contient des caractères non pris en charge."];
        }
        else if (description.Length > BacklogInputRules.DescriptionLongueurMaximale)
        {
            erreurs[nameof(commande.Description)] =
                [$"La description ne peut pas dépasser {BacklogInputRules.DescriptionLongueurMaximale:N0} caractères."];
        }

        return erreurs.Count == 0 ? null : erreurs;
    }
}

using BlazorEnterpriseStarter.App.Services;
using BlazorEnterpriseStarter.Components.Common;
using BlazorEnterpriseStarter.Shared.Contracts;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorEnterpriseStarter.App.Components.Pages;

/// <summary>
/// Porte le cas d’usage démonstratif de gestion de backlog produit.
/// </summary>
public partial class Backlog : ComponentBase
{
    private readonly BacklogItemsQueryDto _requete = new();
    private readonly BacklogItemFormModel _formulaire = new();

    private readonly IReadOnlyList<AppSelectOption> _statutOptions =
        CreerOptionsAvecEnum<BacklogItemStatus>(static statut => ObtenirLibelleStatut(statut));

    private readonly IReadOnlyList<AppSelectOption> _prioriteOptions =
        CreerOptionsAvecEnum<BacklogItemPriority>(static priorite => ObtenirLibellePriorite(priorite));

    private readonly IReadOnlyList<AppSelectOption> _statutFormulaireOptions =
        CreerOptionsAvecEnum<BacklogItemStatus>(static statut => ObtenirLibelleStatut(statut));

    private readonly IReadOnlyList<AppSelectOption> _prioriteFormulaireOptions =
        CreerOptionsAvecEnum<BacklogItemPriority>(static priorite => ObtenirLibellePriorite(priorite));

    private readonly IReadOnlyList<AppSelectOption> _triOptions =
    [
        new(nameof(BacklogItemSortField.DateCreation), "Date de création"),
        new(nameof(BacklogItemSortField.Priorite), "Priorité"),
        new(nameof(BacklogItemSortField.Statut), "Statut"),
        new(nameof(BacklogItemSortField.Titre), "Titre")
    ];

    private readonly IReadOnlyList<AppSelectOption> _directionOptions =
    [
        new(nameof(DirectionTri.Decroissante), "Décroissante"),
        new(nameof(DirectionTri.Croissante), "Croissante")
    ];

    private PagedResultDto<BacklogItemDto>? _resultat;
    private bool _chargementInitial = true;
    private bool _enregistrementEnCours;
    private bool _suppressionEnCours;
    private string? _erreurChargement;
    private string? _erreurFormulaireGlobale;
    private Dictionary<string, string[]> _erreursFormulaire = [];
    private string? _recherche;
    private string? _statutSelectionne;
    private string? _prioriteSelectionnee;
    private string _triSelectionne = nameof(BacklogItemSortField.DateCreation);
    private string _directionSelectionnee = nameof(DirectionTri.Decroissante);
    private string _statutFormulaireSelectionne = nameof(BacklogItemStatus.Nouveau);
    private string _prioriteFormulaireSelectionnee = nameof(BacklogItemPriority.Moyenne);
    private Guid? _idEdition;
    private bool _modaleEditionOuverte;
    private bool _modaleSuppressionOuverte;
    private BacklogItemDto? _itemEnSuppression;

    [Inject]
    public BacklogApiClient BacklogApiClient { get; set; } = default!;

    [Inject]
    public ILogger<Backlog> Logger { get; set; } = default!;

    private IReadOnlyList<BacklogItemDto> Elements => _resultat?.Elements ?? [];

    private string NombreTotalLibelle => $"{(_resultat?.NombreTotal ?? 0)} éléments";

    private BacklogItemStatus? FiltreStatutActif => ConvertirEnumNullable<BacklogItemStatus>(_statutSelectionne);

    private BacklogItemPriority? FiltrePrioriteActif => ConvertirEnumNullable<BacklogItemPriority>(_prioriteSelectionnee);

    private bool PeutAllerPagePrecedente => (_resultat?.NumeroPage ?? 1) > 1;

    private bool PeutAllerPageSuivante => _resultat is not null && _resultat.NumeroPage < _resultat.NombrePages;

    private string TitreModaleEdition => _idEdition is null ? "Créer un élément de backlog" : "Modifier l’élément de backlog";

    private string LibelleActionEdition => _idEdition is null ? "Créer l’élément" : "Enregistrer les modifications";

    protected override async Task OnInitializedAsync()
    {
        await ChargerAsync(CancellationToken.None);
    }

    private async Task ChargerAsync(CancellationToken cancellationToken)
    {
        _erreurChargement = null;

        try
        {
            _resultat = await BacklogApiClient.ListerAsync(CreerRequeteCourante(), cancellationToken);
        }
        catch (BacklogApiException exception)
        {
            _erreurChargement = exception.Message;
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Impossible de charger le backlog produit.");
            _erreurChargement = "Une erreur inattendue est survenue lors du chargement du backlog.";
        }
        finally
        {
            _chargementInitial = false;
        }
    }

    private Task ActualiserAsync(MouseEventArgs _)
    {
        _chargementInitial = true;
        return ChargerAsync(CancellationToken.None);
    }

    private Task AppliquerFiltresAsync(MouseEventArgs _)
    {
        _requete.NumeroPage = 1;
        return RechargerAvecFiltresAsync();
    }

    private Task ReinitialiserFiltresAsync(MouseEventArgs _)
    {
        _recherche = null;
        _statutSelectionne = null;
        _prioriteSelectionnee = null;
        _triSelectionne = nameof(BacklogItemSortField.DateCreation);
        _directionSelectionnee = nameof(DirectionTri.Decroissante);
        _requete.NumeroPage = 1;

        return RechargerAvecFiltresAsync();
    }

    private async Task AllerPagePrecedenteAsync(MouseEventArgs _)
    {
        if (!PeutAllerPagePrecedente)
        {
            return;
        }

        _requete.NumeroPage--;
        await RechargerAvecFiltresAsync();
    }

    private async Task AllerPageSuivanteAsync(MouseEventArgs _)
    {
        if (!PeutAllerPageSuivante)
        {
            return;
        }

        _requete.NumeroPage++;
        await RechargerAvecFiltresAsync();
    }

    private Task OuvrirCreationAsync(MouseEventArgs _)
    {
        _idEdition = null;
        _formulaire.Titre = string.Empty;
        _formulaire.Description = string.Empty;
        _statutFormulaireSelectionne = nameof(BacklogItemStatus.Nouveau);
        _prioriteFormulaireSelectionnee = nameof(BacklogItemPriority.Moyenne);
        _erreursFormulaire.Clear();
        _erreurFormulaireGlobale = null;
        _modaleEditionOuverte = true;

        return Task.CompletedTask;
    }

    private Task OuvrirEditionAsync(BacklogItemDto item, MouseEventArgs _)
    {
        _idEdition = item.Id;
        _formulaire.Titre = item.Titre;
        _formulaire.Description = item.Description;
        _statutFormulaireSelectionne = item.Statut.ToString();
        _prioriteFormulaireSelectionnee = item.Priorite.ToString();
        _erreursFormulaire.Clear();
        _erreurFormulaireGlobale = null;
        _modaleEditionOuverte = true;

        return Task.CompletedTask;
    }

    private Task FermerEditionAsync()
    {
        _modaleEditionOuverte = false;
        _enregistrementEnCours = false;
        _erreurFormulaireGlobale = null;
        _erreursFormulaire.Clear();

        return Task.CompletedTask;
    }

    private Task AnnulerEditionAsync(MouseEventArgs _)
    {
        return FermerEditionAsync();
    }

    private async Task EnregistrerAsync(MouseEventArgs _)
    {
        _erreursFormulaire.Clear();
        _erreurFormulaireGlobale = null;

        if (!ValiderFormulaire())
        {
            return;
        }

        _enregistrementEnCours = true;

        try
        {
            var commande = new BacklogItemUpsertRequest
            {
                Titre = _formulaire.Titre,
                Description = _formulaire.Description,
                Statut = ConvertirEnum<BacklogItemStatus>(_statutFormulaireSelectionne, BacklogItemStatus.Nouveau),
                Priorite = ConvertirEnum<BacklogItemPriority>(_prioriteFormulaireSelectionnee, BacklogItemPriority.Moyenne)
            };

            if (_idEdition is null)
            {
                await BacklogApiClient.CreerAsync(commande, CancellationToken.None);
            }
            else
            {
                await BacklogApiClient.ModifierAsync(_idEdition.Value, commande, CancellationToken.None);
            }

            await FermerEditionAsync();
            await ChargerAsync(CancellationToken.None);
        }
        catch (BacklogApiException exception)
        {
            _erreurFormulaireGlobale = exception.Message;
            _erreursFormulaire = exception.Errors.ToDictionary(entry => entry.Key, entry => entry.Value);
        }
        finally
        {
            _enregistrementEnCours = false;
        }
    }

    private Task OuvrirSuppressionAsync(BacklogItemDto item, MouseEventArgs _)
    {
        _itemEnSuppression = item;
        _modaleSuppressionOuverte = true;
        return Task.CompletedTask;
    }

    private Task FermerSuppressionAsync()
    {
        _itemEnSuppression = null;
        _modaleSuppressionOuverte = false;
        _suppressionEnCours = false;
        return Task.CompletedTask;
    }

    private Task AnnulerSuppressionAsync(MouseEventArgs _)
    {
        return FermerSuppressionAsync();
    }

    private async Task ConfirmerSuppressionAsync(MouseEventArgs _)
    {
        if (_itemEnSuppression is null)
        {
            return;
        }

        _suppressionEnCours = true;

        try
        {
            await BacklogApiClient.SupprimerAsync(_itemEnSuppression.Id, CancellationToken.None);

            if (_resultat is not null && _resultat.Elements.Count == 1 && _resultat.NumeroPage > 1)
            {
                _requete.NumeroPage--;
            }

            await FermerSuppressionAsync();
            await ChargerAsync(CancellationToken.None);
        }
        catch (BacklogApiException exception)
        {
            _erreurChargement = exception.Message;
            await FermerSuppressionAsync();
        }
        finally
        {
            _suppressionEnCours = false;
        }
    }

    private string? ObtenirErreurChamp(string nomChamp) =>
        _erreursFormulaire.TryGetValue(nomChamp, out var erreurs)
            ? string.Join(" ", erreurs)
            : null;

    private Task RechargerAvecFiltresAsync()
    {
        _requete.Recherche = string.IsNullOrWhiteSpace(_recherche) ? null : _recherche.Trim();
        _requete.Statut = ConvertirEnumNullable<BacklogItemStatus>(_statutSelectionne);
        _requete.Priorite = ConvertirEnumNullable<BacklogItemPriority>(_prioriteSelectionnee);
        _requete.Tri = ConvertirEnum(_triSelectionne, BacklogItemSortField.DateCreation);
        _requete.Direction = ConvertirEnum(_directionSelectionnee, DirectionTri.Decroissante);

        return ChargerAsync(CancellationToken.None);
    }

    private BacklogItemsQueryDto CreerRequeteCourante() =>
        new()
        {
            Recherche = _requete.Recherche,
            Statut = _requete.Statut,
            Priorite = _requete.Priorite,
            Tri = _requete.Tri,
            Direction = _requete.Direction,
            NumeroPage = _requete.NumeroPage,
            TaillePage = _requete.TaillePage
        };

    private bool ValiderFormulaire()
    {
        var erreurs = new Dictionary<string, string[]>();
        var titre = _formulaire.Titre.Trim();
        var description = _formulaire.Description.Trim();

        if (string.IsNullOrWhiteSpace(titre))
        {
            erreurs[nameof(BacklogItemFormModel.Titre)] = ["Le titre est obligatoire."];
        }
        else if (titre.Length > 120)
        {
            erreurs[nameof(BacklogItemFormModel.Titre)] = ["Le titre ne peut pas dépasser 120 caractères."];
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            erreurs[nameof(BacklogItemFormModel.Description)] = ["La description est obligatoire."];
        }
        else if (description.Length > 2000)
        {
            erreurs[nameof(BacklogItemFormModel.Description)] = ["La description ne peut pas dépasser 2 000 caractères."];
        }

        _erreursFormulaire = erreurs;
        return erreurs.Count == 0;
    }

    private static IReadOnlyList<AppSelectOption> CreerOptionsAvecEnum<TEnum>(Func<TEnum, string> formatter)
        where TEnum : struct, Enum =>
        Enum.GetValues<TEnum>()
            .Select(value => new AppSelectOption(value.ToString(), formatter(value)))
            .ToArray();

    private static TEnum ConvertirEnum<TEnum>(string? value, TEnum valeurParDefaut)
        where TEnum : struct, Enum =>
        Enum.TryParse<TEnum>(value, out var resultat)
            ? resultat
            : valeurParDefaut;

    private static TEnum? ConvertirEnumNullable<TEnum>(string? value)
        where TEnum : struct, Enum =>
        Enum.TryParse<TEnum>(value, out var resultat)
            ? resultat
            : null;

    private static string ObtenirLibelleStatut(BacklogItemStatus statut) => statut switch
    {
        BacklogItemStatus.Nouveau => "Nouveau",
        BacklogItemStatus.Pret => "Prêt",
        BacklogItemStatus.EnCours => "En cours",
        BacklogItemStatus.Termine => "Terminé",
        BacklogItemStatus.Archive => "Archivé",
        _ => statut.ToString()
    };

    private static string ObtenirLibellePriorite(BacklogItemPriority priorite) => priorite switch
    {
        BacklogItemPriority.Basse => "Basse",
        BacklogItemPriority.Moyenne => "Moyenne",
        BacklogItemPriority.Haute => "Haute",
        BacklogItemPriority.Critique => "Critique",
        _ => priorite.ToString()
    };

    private static AppTone ObtenirTonaliteStatut(BacklogItemStatus statut) => statut switch
    {
        BacklogItemStatus.Termine => AppTone.Success,
        BacklogItemStatus.EnCours => AppTone.Accent,
        BacklogItemStatus.Pret => AppTone.Info,
        BacklogItemStatus.Archive => AppTone.Neutral,
        _ => AppTone.Warning
    };

    private static AppTone ObtenirTonalitePriorite(BacklogItemPriority priorite) => priorite switch
    {
        BacklogItemPriority.Critique => AppTone.Danger,
        BacklogItemPriority.Haute => AppTone.Warning,
        BacklogItemPriority.Moyenne => AppTone.Accent,
        _ => AppTone.Neutral
    };

    private sealed class BacklogItemFormModel
    {
        public string Titre { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}

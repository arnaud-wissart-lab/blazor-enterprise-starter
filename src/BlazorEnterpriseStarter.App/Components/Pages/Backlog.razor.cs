using BlazorEnterpriseStarter.App.Services;
using BlazorEnterpriseStarter.App.State.Backlog;
using BlazorEnterpriseStarter.Components.Common;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorEnterpriseStarter.App.Components.Pages;

/// <summary>
/// Porte le cas d’usage démonstratif de gestion de backlog produit.
/// </summary>
public partial class Backlog : ComponentBase, IDisposable
{
    private readonly BacklogItemFormModel _formulaire = new();

    private readonly IReadOnlyList<AppSelectOption> _statutOptions =
        CreerOptionsAvecEnum<BacklogItemStatus>(static statut => ObtenirLibelleStatut(statut), inclureOptionVide: true, libelleOptionVide: "Tous les statuts");

    private readonly IReadOnlyList<AppSelectOption> _prioriteOptions =
        CreerOptionsAvecEnum<BacklogItemPriority>(static priorite => ObtenirLibellePriorite(priorite), inclureOptionVide: true, libelleOptionVide: "Toutes les priorités");

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

    private Dictionary<string, string[]> _erreursFormulaire = [];
    private string? _erreurFormulaireGlobale;
    private string? _messageSucces;
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
    public BacklogState State { get; set; } = default!;

    private IReadOnlyList<BacklogItemDto> Elements => State.Items;

    private string NombreTotalLibelle => $"{State.TotalCount} éléments";

    private BacklogItemStatus? FiltreStatutActif => ConvertirEnumNullable<BacklogItemStatus>(_statutSelectionne);

    private BacklogItemPriority? FiltrePrioriteActif => ConvertirEnumNullable<BacklogItemPriority>(_prioriteSelectionnee);

    private bool PeutAllerPagePrecedente => State.CanGoPrevious;

    private bool PeutAllerPageSuivante => State.CanGoNext;

    private string TitreModaleEdition => _idEdition is null ? "Créer un élément de backlog" : "Modifier l’élément de backlog";

    private string LibelleActionEdition => _idEdition is null ? "Créer l’élément" : "Enregistrer les modifications";

    protected override async Task OnInitializedAsync()
    {
        State.Changed += HandleStateChanged;
        SynchroniserFiltresDepuisEtat();
        await State.InitializeAsync(CancellationToken.None);
    }

    public void Dispose()
    {
        State.Changed -= HandleStateChanged;
    }

    private void HandleStateChanged()
    {
        _ = InvokeAsync(StateHasChanged);
    }

    private async Task ActualiserAsync(MouseEventArgs _)
    {
        _messageSucces = null;
        await State.RefreshAsync(CancellationToken.None);
    }

    private async Task AppliquerFiltresAsync(MouseEventArgs _)
    {
        _messageSucces = null;
        await State.ApplyFiltersAsync(CreerRequeteDepuisSaisie(resetPage: true), CancellationToken.None);
    }

    private async Task ReinitialiserFiltresAsync(MouseEventArgs _)
    {
        _messageSucces = null;
        _recherche = null;
        _statutSelectionne = null;
        _prioriteSelectionnee = null;
        _triSelectionne = nameof(BacklogItemSortField.DateCreation);
        _directionSelectionnee = nameof(DirectionTri.Decroissante);

        await State.ApplyFiltersAsync(CreerRequeteDepuisSaisie(resetPage: true), CancellationToken.None);
    }

    private async Task AllerPagePrecedenteAsync(MouseEventArgs _)
    {
        _messageSucces = null;
        await State.GoToPreviousPageAsync(CancellationToken.None);
    }

    private async Task AllerPageSuivanteAsync(MouseEventArgs _)
    {
        _messageSucces = null;
        await State.GoToNextPageAsync(CancellationToken.None);
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
        _messageSucces = null;
        State.ResetMutationFeedback();
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
        _messageSucces = null;
        State.ResetMutationFeedback();
        _modaleEditionOuverte = true;

        return Task.CompletedTask;
    }

    private Task FermerEditionAsync()
    {
        _modaleEditionOuverte = false;
        _erreurFormulaireGlobale = null;
        _erreursFormulaire.Clear();

        return Task.CompletedTask;
    }

    private Task AnnulerEditionAsync(MouseEventArgs _)
    {
        State.ResetMutationFeedback();
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

        try
        {
            var commande = new BacklogItemUpsertRequest
            {
                Titre = _formulaire.Titre,
                Description = _formulaire.Description,
                Statut = ConvertirEnum(_statutFormulaireSelectionne, BacklogItemStatus.Nouveau),
                Priorite = ConvertirEnum(_prioriteFormulaireSelectionnee, BacklogItemPriority.Moyenne)
            };

            if (_idEdition is null)
            {
                await State.CreateAsync(commande, CancellationToken.None);
                _messageSucces = "L’élément de backlog a été créé avec succès.";
            }
            else
            {
                await State.UpdateAsync(_idEdition.Value, commande, CancellationToken.None);
                _messageSucces = "L’élément de backlog a été mis à jour.";
            }

            await FermerEditionAsync();
        }
        catch (BacklogApiException)
        {
            _erreurFormulaireGlobale = State.MutationErrorMessage;
            _erreursFormulaire = State.MutationErrors.ToDictionary(entry => entry.Key, entry => entry.Value);
        }
    }

    private Task OuvrirSuppressionAsync(BacklogItemDto item, MouseEventArgs _)
    {
        _itemEnSuppression = item;
        _messageSucces = null;
        State.ResetMutationFeedback();
        _modaleSuppressionOuverte = true;
        return Task.CompletedTask;
    }

    private Task FermerSuppressionAsync()
    {
        _itemEnSuppression = null;
        _modaleSuppressionOuverte = false;
        return Task.CompletedTask;
    }

    private Task AnnulerSuppressionAsync(MouseEventArgs _)
    {
        State.ResetMutationFeedback();
        return FermerSuppressionAsync();
    }

    private async Task ConfirmerSuppressionAsync(MouseEventArgs _)
    {
        if (_itemEnSuppression is null)
        {
            return;
        }

        try
        {
            await State.DeleteAsync(_itemEnSuppression.Id, CancellationToken.None);
            _messageSucces = "L’élément de backlog a été supprimé.";
            await FermerSuppressionAsync();
        }
        catch (BacklogApiException)
        {
            await FermerSuppressionAsync();
        }
    }

    private string? ObtenirErreurChamp(string nomChamp) =>
        _erreursFormulaire.TryGetValue(nomChamp, out var erreurs)
            ? string.Join(" ", erreurs)
            : null;

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

    private BacklogItemsQueryDto CreerRequeteDepuisSaisie(bool resetPage) =>
        new()
        {
            Recherche = string.IsNullOrWhiteSpace(_recherche) ? null : _recherche.Trim(),
            Statut = ConvertirEnumNullable<BacklogItemStatus>(_statutSelectionne),
            Priorite = ConvertirEnumNullable<BacklogItemPriority>(_prioriteSelectionnee),
            Tri = ConvertirEnum(_triSelectionne, BacklogItemSortField.DateCreation),
            Direction = ConvertirEnum(_directionSelectionnee, DirectionTri.Decroissante),
            NumeroPage = resetPage ? 1 : State.Query.NumeroPage,
            TaillePage = State.Query.TaillePage
        };

    private void SynchroniserFiltresDepuisEtat()
    {
        _recherche = State.Query.Recherche;
        _statutSelectionne = State.Query.Statut?.ToString();
        _prioriteSelectionnee = State.Query.Priorite?.ToString();
        _triSelectionne = State.Query.Tri.ToString();
        _directionSelectionnee = State.Query.Direction.ToString();
    }

    private static IReadOnlyList<AppSelectOption> CreerOptionsAvecEnum<TEnum>(
        Func<TEnum, string> formatter,
        bool inclureOptionVide = false,
        string? libelleOptionVide = null)
        where TEnum : struct, Enum
    {
        var options = Enum.GetValues<TEnum>()
            .Select(value => new AppSelectOption(value.ToString(), formatter(value)))
            .ToList();

        if (inclureOptionVide)
        {
            options.Insert(0, new AppSelectOption(string.Empty, libelleOptionVide ?? "Tous"));
        }

        return options;
    }

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

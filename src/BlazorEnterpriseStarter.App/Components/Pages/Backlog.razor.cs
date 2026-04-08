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
    private const int DelaiRechercheDebounceEnMillisecondes = 350;

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
    private CancellationTokenSource? _rechercheDebounceCts;
    private Guid? _idEdition;
    private bool _modaleEditionOuverte;
    private bool _modaleSuppressionOuverte;
    private BacklogItemDto? _itemEnSuppression;
    private DirectionAnimationPagination _directionAnimationPagination = DirectionAnimationPagination.Aucune;

    [Inject]
    public BacklogState State { get; set; } = default!;

    private IReadOnlyList<BacklogItemDto> Elements => State.Items;

    private string? Recherche
    {
        get => _recherche;
        set
        {
            if (string.Equals(_recherche, value, StringComparison.Ordinal))
            {
                return;
            }

            _recherche = value;
            _messageSucces = null;
            _ = PlanifierRechercheDebounceeAsync();
        }
    }

    private string NombreTotalLibelle => $"{State.TotalCount} éléments";

    private BacklogItemStatus? FiltreStatutActif => ConvertirEnumNullable<BacklogItemStatus>(_statutSelectionne);

    private BacklogItemPriority? FiltrePrioriteActif => ConvertirEnumNullable<BacklogItemPriority>(_prioriteSelectionnee);

    private bool PeutAllerPagePrecedente => State.CanGoPrevious;

    private bool PeutAllerPageSuivante => State.CanGoNext;

    private bool AfficherEtatChargementInitial => !State.HasLoadedOnce;

    private bool AfficherIndicateurRafraichissement => State.IsRefreshing;

    private bool AfficherErreurPleinePage =>
        State.ListStatus == BacklogRequestStatus.Error
        && !State.HasResult
        && !string.IsNullOrWhiteSpace(State.ListErrorMessage);

    private bool AfficherErreurNonBloquante =>
        State.ListStatus == BacklogRequestStatus.Error
        && State.HasResult
        && !string.IsNullOrWhiteSpace(State.ListErrorMessage);

    private bool AUnFiltreActif =>
        !string.IsNullOrWhiteSpace(_recherche)
        || FiltreStatutActif is not null
        || FiltrePrioriteActif is not null;

    private string TitreModaleEdition => _idEdition is null ? "Créer un élément de backlog" : "Modifier l’élément de backlog";

    private string LibelleActionEdition => _idEdition is null ? "Créer l’élément" : "Enregistrer les modifications";

    private int NombreElementsEnCoursVisibles => Elements.Count(static item => item.Statut == BacklogItemStatus.EnCours);

    private int NombreElementsPretsVisibles => Elements.Count(static item => item.Statut == BacklogItemStatus.Pret);

    private int NombreElementsCritiquesVisibles => Elements.Count(static item => item.Priorite == BacklogItemPriority.Critique);

    private string DescriptionRechercheActive =>
        string.IsNullOrWhiteSpace(_recherche)
            ? "La recherche se lance automatiquement après une courte pause."
            : $"Recherche active : « {_recherche} »";

    private string LibelleTriCourant =>
        $"{ObtenirLibelleTri(ConvertirEnum(_triSelectionne, BacklogItemSortField.DateCreation))} · {ObtenirLibelleDirection(ConvertirEnum(_directionSelectionnee, DirectionTri.Decroissante))}";

    private string ResumeVueActive
    {
        get
        {
            var segments = new List<string>();

            if (string.IsNullOrWhiteSpace(_recherche)
                && FiltreStatutActif is null
                && FiltrePrioriteActif is null)
            {
                segments.Add("Vue complète");
            }

            if (!string.IsNullOrWhiteSpace(_recherche))
            {
                segments.Add($"Recherche « {_recherche} »");
            }

            if (FiltreStatutActif is not null)
            {
                segments.Add($"Statut {ObtenirLibelleStatut(FiltreStatutActif.Value)}");
            }

            if (FiltrePrioriteActif is not null)
            {
                segments.Add($"Priorité {ObtenirLibellePriorite(FiltrePrioriteActif.Value)}");
            }

            segments.Add(LibelleTriCourant);

            return string.Join(" · ", segments);
        }
    }

    private string? VueRapideActive => (FiltreStatutActif, FiltrePrioriteActif) switch
    {
        (null, null) when string.IsNullOrWhiteSpace(_recherche) => "tout",
        (BacklogItemStatus.Pret, null) => "pret",
        (BacklogItemStatus.EnCours, null) => "encours",
        (null, BacklogItemPriority.Critique) => "critique",
        _ => null
    };

    private BacklogItemStatus StatutFormulaireCourant => ConvertirEnum(_statutFormulaireSelectionne, BacklogItemStatus.Nouveau);

    private BacklogItemPriority PrioriteFormulaireCourante => ConvertirEnum(_prioriteFormulaireSelectionnee, BacklogItemPriority.Moyenne);

    private string LibelleStatutFormulaire => ObtenirLibelleStatut(StatutFormulaireCourant);

    private string LibellePrioriteFormulaire => ObtenirLibellePriorite(PrioriteFormulaireCourante);

    private AppTone StatutFormulaireTone => ObtenirTonaliteStatut(StatutFormulaireCourant);

    private AppTone PrioriteFormulaireTone => ObtenirTonalitePriorite(PrioriteFormulaireCourante);

    private string TitreSyntheseFormulaire =>
        _idEdition is null
            ? "Préparer un élément clair, actionnable et facile à prioriser."
            : "Mettre à jour l’élément sans casser la lisibilité du backlog.";

    private string DescriptionSyntheseFormulaire =>
        $"La fiche reste courte, avec le statut {LibelleStatutFormulaire.ToLowerInvariant()} et la priorité {LibellePrioriteFormulaire.ToLowerInvariant()}.";

    private string DescriptionTitreFormulaire =>
        $"{_formulaire.Titre.Length}/{BacklogInputRules.TitreLongueurMaximale} caractères · privilégiez un libellé orienté action.";

    private string DescriptionDescriptionFormulaire =>
        $"{_formulaire.Description.Length}/{BacklogInputRules.DescriptionLongueurMaximale:N0} caractères · décrivez le besoin, le contexte et l’attendu.";

    private string? ClasseAnimationPagination => _directionAnimationPagination switch
    {
        DirectionAnimationPagination.Suivante => "backlog-list--slide-next",
        DirectionAnimationPagination.Precedente => "backlog-list--slide-previous",
        _ => null
    };

    protected override async Task OnInitializedAsync()
    {
        State.Changed += HandleStateChanged;
        SynchroniserFiltresDepuisEtat();
        await State.InitializeAsync(CancellationToken.None);
    }

    public void Dispose()
    {
        AnnulerRechercheDebouncee();
        State.Changed -= HandleStateChanged;
    }

    private void HandleStateChanged()
    {
        _ = InvokeAsync(StateHasChanged);
    }

    private async Task ActualiserAsync(MouseEventArgs _)
    {
        AnnulerRechercheDebouncee();
        _messageSucces = null;
        _directionAnimationPagination = DirectionAnimationPagination.Aucune;
        await State.RefreshAsync(CancellationToken.None);
    }

    private async Task AppliquerFiltresAsync(MouseEventArgs _)
    {
        AnnulerRechercheDebouncee();
        _messageSucces = null;
        _directionAnimationPagination = DirectionAnimationPagination.Aucune;
        await State.ApplyFiltersAsync(CreerRequeteDepuisSaisie(resetPage: true), CancellationToken.None);
    }

    private async Task ReinitialiserFiltresAsync(MouseEventArgs _)
    {
        AnnulerRechercheDebouncee();
        _messageSucces = null;
        _directionAnimationPagination = DirectionAnimationPagination.Aucune;
        _recherche = null;
        _statutSelectionne = null;
        _prioriteSelectionnee = null;
        _triSelectionne = nameof(BacklogItemSortField.DateCreation);
        _directionSelectionnee = nameof(DirectionTri.Decroissante);

        await State.ApplyFiltersAsync(CreerRequeteDepuisSaisie(resetPage: true), CancellationToken.None);
    }

    private async Task AppliquerVueRapideAsync(string vue, MouseEventArgs _)
    {
        AnnulerRechercheDebouncee();
        _messageSucces = null;
        _directionAnimationPagination = DirectionAnimationPagination.Aucune;

        switch (vue)
        {
            case "pret":
                _statutSelectionne = nameof(BacklogItemStatus.Pret);
                _prioriteSelectionnee = null;
                break;
            case "encours":
                _statutSelectionne = nameof(BacklogItemStatus.EnCours);
                _prioriteSelectionnee = null;
                break;
            case "critique":
                _statutSelectionne = null;
                _prioriteSelectionnee = nameof(BacklogItemPriority.Critique);
                break;
            default:
                _statutSelectionne = null;
                _prioriteSelectionnee = null;
                break;
        }

        await State.ApplyFiltersAsync(CreerRequeteDepuisSaisie(resetPage: true), CancellationToken.None);
    }

    private async Task EffacerRechercheAsync(MouseEventArgs _)
    {
        if (string.IsNullOrWhiteSpace(_recherche))
        {
            return;
        }

        AnnulerRechercheDebouncee();
        _messageSucces = null;
        _directionAnimationPagination = DirectionAnimationPagination.Aucune;
        _recherche = null;
        await State.ApplyFiltersAsync(CreerRequeteDepuisSaisie(resetPage: true), CancellationToken.None);
    }

    private async Task EffacerFiltreStatutAsync(MouseEventArgs _)
    {
        if (FiltreStatutActif is null)
        {
            return;
        }

        AnnulerRechercheDebouncee();
        _messageSucces = null;
        _directionAnimationPagination = DirectionAnimationPagination.Aucune;
        _statutSelectionne = null;
        await State.ApplyFiltersAsync(CreerRequeteDepuisSaisie(resetPage: true), CancellationToken.None);
    }

    private async Task EffacerFiltrePrioriteAsync(MouseEventArgs _)
    {
        if (FiltrePrioriteActif is null)
        {
            return;
        }

        AnnulerRechercheDebouncee();
        _messageSucces = null;
        _directionAnimationPagination = DirectionAnimationPagination.Aucune;
        _prioriteSelectionnee = null;
        await State.ApplyFiltersAsync(CreerRequeteDepuisSaisie(resetPage: true), CancellationToken.None);
    }

    private async Task AllerPagePrecedenteAsync(MouseEventArgs _)
    {
        AnnulerRechercheDebouncee();
        _messageSucces = null;
        _directionAnimationPagination = DirectionAnimationPagination.Precedente;
        await State.GoToPreviousPageAsync(CancellationToken.None);
    }

    private async Task AllerPageSuivanteAsync(MouseEventArgs _)
    {
        AnnulerRechercheDebouncee();
        _messageSucces = null;
        _directionAnimationPagination = DirectionAnimationPagination.Suivante;
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
            var titre = BacklogInputRules.NormaliserTitre(_formulaire.Titre);
            var description = BacklogInputRules.NormaliserDescription(_formulaire.Description);

            _formulaire.Titre = titre;
            _formulaire.Description = description;

            var commande = new BacklogItemUpsertRequest
            {
                Titre = titre,
                Description = description,
                Statut = ConvertirEnum(_statutFormulaireSelectionne, BacklogItemStatus.Nouveau),
                Priorite = ConvertirEnum(_prioriteFormulaireSelectionnee, BacklogItemPriority.Moyenne)
            };

            if (_idEdition is null)
            {
                _directionAnimationPagination = DirectionAnimationPagination.Aucune;
                await State.CreateAsync(commande, CancellationToken.None);
                _messageSucces = "L’élément de backlog a été créé avec succès.";
            }
            else
            {
                _directionAnimationPagination = DirectionAnimationPagination.Aucune;
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
            _directionAnimationPagination = DirectionAnimationPagination.Aucune;
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
        var titreBrut = _formulaire.Titre;
        var descriptionBrute = _formulaire.Description;
        var titre = BacklogInputRules.NormaliserTitre(titreBrut);
        var description = BacklogInputRules.NormaliserDescription(descriptionBrute);

        _formulaire.Titre = titre;
        _formulaire.Description = description;

        if (string.IsNullOrWhiteSpace(titre))
        {
            erreurs[nameof(BacklogItemFormModel.Titre)] = ["Le titre est obligatoire."];
        }
        else if (BacklogInputRules.ContientDesCaracteresInterditsPourTitre(titreBrut))
        {
            erreurs[nameof(BacklogItemFormModel.Titre)] = ["Le titre contient des caractères non pris en charge."];
        }
        else if (titre.Length > BacklogInputRules.TitreLongueurMaximale)
        {
            erreurs[nameof(BacklogItemFormModel.Titre)] =
                [$"Le titre ne peut pas dépasser {BacklogInputRules.TitreLongueurMaximale} caractères."];
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            erreurs[nameof(BacklogItemFormModel.Description)] = ["La description est obligatoire."];
        }
        else if (BacklogInputRules.ContientDesCaracteresInterditsPourDescription(descriptionBrute))
        {
            erreurs[nameof(BacklogItemFormModel.Description)] = ["La description contient des caractères non pris en charge."];
        }
        else if (description.Length > BacklogInputRules.DescriptionLongueurMaximale)
        {
            erreurs[nameof(BacklogItemFormModel.Description)] =
                [$"La description ne peut pas dépasser {BacklogInputRules.DescriptionLongueurMaximale:N0} caractères."];
        }

        _erreursFormulaire = erreurs;
        return erreurs.Count == 0;
    }

    private async Task PlanifierRechercheDebounceeAsync()
    {
        AnnulerRechercheDebouncee();
        var cts = new CancellationTokenSource();
        _rechercheDebounceCts = cts;

        try
        {
            await Task.Delay(DelaiRechercheDebounceEnMillisecondes, cts.Token);
            await State.ApplyFiltersAsync(CreerRequeteDepuisSaisie(resetPage: true), cts.Token);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            if (ReferenceEquals(_rechercheDebounceCts, cts))
            {
                _rechercheDebounceCts.Dispose();
                _rechercheDebounceCts = null;
            }
        }
    }

    private void AnnulerRechercheDebouncee()
    {
        _rechercheDebounceCts?.Cancel();
        _rechercheDebounceCts?.Dispose();
        _rechercheDebounceCts = null;
    }

    private BacklogItemsQueryDto CreerRequeteDepuisSaisie(bool resetPage) =>
        new()
        {
            Recherche = BacklogInputRules.NormaliserRecherche(_recherche),
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

    private static string ObtenirLibelleTri(BacklogItemSortField tri) => tri switch
    {
        BacklogItemSortField.DateCreation => "Date de création",
        BacklogItemSortField.Priorite => "Priorité",
        BacklogItemSortField.Statut => "Statut",
        BacklogItemSortField.Titre => "Titre",
        _ => tri.ToString()
    };

    private static string ObtenirLibelleDirection(DirectionTri direction) => direction switch
    {
        DirectionTri.Decroissante => "ordre décroissant",
        DirectionTri.Croissante => "ordre croissant",
        _ => direction.ToString()
    };

    private static string ObtenirLibelleLecture(BacklogItemDto item) => item.Statut switch
    {
        BacklogItemStatus.Nouveau when item.Priorite == BacklogItemPriority.Critique => "Point d’entrée urgent à qualifier",
        BacklogItemStatus.Nouveau => "Sujet encore à cadrer",
        BacklogItemStatus.Pret => "Sujet déjà structuré et mobilisable",
        BacklogItemStatus.EnCours => "Travail en progression active",
        BacklogItemStatus.Termine => "Livrable finalisé",
        BacklogItemStatus.Archive => "Historique conservé",
        _ => "Lecture standard"
    };

    private static string ObtenirLibelleActionRecommandee(BacklogItemDto item) => item.Statut switch
    {
        BacklogItemStatus.Nouveau when item.Priorite == BacklogItemPriority.Critique => "Qualifier immédiatement le besoin",
        BacklogItemStatus.Nouveau => "Préciser le périmètre",
        BacklogItemStatus.Pret => "Planifier l’engagement",
        BacklogItemStatus.EnCours => "Suivre l’exécution",
        BacklogItemStatus.Termine => "Préparer la clôture",
        BacklogItemStatus.Archive => "Conserver comme référence",
        _ => "Aucune action suggérée"
    };

    private sealed class BacklogItemFormModel
    {
        public string Titre { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }

    private enum DirectionAnimationPagination
    {
        Aucune,
        Suivante,
        Precedente
    }
}

using BlazorEnterpriseStarter.Components.Common;
using Microsoft.AspNetCore.Components;

namespace BlazorEnterpriseStarter.Components;

/// <summary>
/// Représente une liste déroulante simple et réutilisable.
/// </summary>
public partial class AppSelect : ComponentBase
{
    /// <summary>
    /// Identifiant HTML du champ. Une valeur est générée automatiquement si ce paramètre n’est pas renseigné.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Nom HTML du champ.
    /// </summary>
    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// Libellé associé au champ.
    /// </summary>
    [Parameter]
    public string? Label { get; set; }

    /// <summary>
    /// Texte d’aide affiché sous le champ.
    /// </summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// Message d’erreur affiché sous le champ.
    /// </summary>
    [Parameter]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Valeur actuellement sélectionnée.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Callback invoqué lorsque la valeur sélectionnée change.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Options affichées dans la liste déroulante.
    /// </summary>
    [Parameter]
    public IReadOnlyList<AppSelectOption> Options { get; set; } = [];

    /// <summary>
    /// Libellé de l’option vide initiale.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; } = "Sélectionner une option";

    /// <summary>
    /// Indique si la sélection est obligatoire.
    /// </summary>
    [Parameter]
    public bool Required { get; set; }

    /// <summary>
    /// Indique si le champ est désactivé.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Indique si le champ affiche un état de chargement.
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <summary>
    /// Classe CSS additionnelle appliquée au conteneur racine.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Attributs HTML supplémentaires propagés vers l’élément select.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    private string _resolvedId = $"app-select-{Guid.NewGuid():N}";

    private string ResolvedId => Id ?? _resolvedId;

    private string DescriptionId => $"{ResolvedId}-description";

    private string ErrorId => $"{ResolvedId}-error";

    private string? AriaDescribedBy => string.Join(
        ' ',
        new[]
        {
            !string.IsNullOrWhiteSpace(Description) ? DescriptionId : null,
            HasError ? ErrorId : null
        }.Where(value => !string.IsNullOrWhiteSpace(value)));

    private string RootClass => new CssClassBuilder()
        .Add("app-field")
        .AddIf("app-field--invalid", HasError)
        .AddIf("app-field--disabled", Disabled)
        .Add(Class)
        .ToString();

    private string SelectClass => new CssClassBuilder()
        .Add("app-field__control")
        .Add("app-field__control--select")
        .AddIf("app-field__control--loading", IsLoading)
        .ToString();

    private async Task HandleChangeAsync(ChangeEventArgs args)
    {
        await ValueChanged.InvokeAsync(args.Value?.ToString());
    }
}

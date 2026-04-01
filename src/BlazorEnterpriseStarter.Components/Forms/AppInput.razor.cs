using BlazorEnterpriseStarter.Components.Common;
using Microsoft.AspNetCore.Components;

namespace BlazorEnterpriseStarter.Components;

/// <summary>
/// Représente un champ texte stylisé pour la saisie simple.
/// </summary>
public partial class AppInput : ComponentBase
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
    /// Valeur actuelle du champ.
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Callback invoqué lorsque la valeur change.
    /// </summary>
    [Parameter]
    public EventCallback<string?> ValueChanged { get; set; }

    /// <summary>
    /// Texte d’exemple affiché lorsque le champ est vide.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Type de saisie HTML du champ.
    /// </summary>
    [Parameter]
    public AppInputType InputType { get; set; } = AppInputType.Text;

    /// <summary>
    /// Valeur de l’attribut autocomplete.
    /// </summary>
    [Parameter]
    public string? Autocomplete { get; set; }

    /// <summary>
    /// Valeur de l’attribut inputmode.
    /// </summary>
    [Parameter]
    public string? InputMode { get; set; }

    /// <summary>
    /// Nombre maximal de caractères autorisés.
    /// </summary>
    [Parameter]
    public int? MaxLength { get; set; }

    /// <summary>
    /// Indique si la saisie est obligatoire.
    /// </summary>
    [Parameter]
    public bool Required { get; set; }

    /// <summary>
    /// Indique si le champ est en lecture seule.
    /// </summary>
    [Parameter]
    public bool ReadOnly { get; set; }

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
    /// Attributs HTML supplémentaires propagés vers l’élément input.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    private string _resolvedId = $"app-input-{Guid.NewGuid():N}";

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

    private string ResolvedInputType => InputType.ToString().ToLowerInvariant();

    private string RootClass => new CssClassBuilder()
        .Add("app-field")
        .AddIf("app-field--invalid", HasError)
        .AddIf("app-field--disabled", Disabled)
        .Add(Class)
        .ToString();

    private string InputClass => new CssClassBuilder()
        .Add("app-field__control")
        .Add("app-field__control--input")
        .AddIf("app-field__control--loading", IsLoading)
        .ToString();

    private async Task HandleInputAsync(ChangeEventArgs args)
    {
        await ValueChanged.InvokeAsync(args.Value?.ToString());
    }
}

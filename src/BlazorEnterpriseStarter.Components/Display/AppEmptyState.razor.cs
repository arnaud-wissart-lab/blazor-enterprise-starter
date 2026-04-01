using BlazorEnterpriseStarter.Components.Common;
using Microsoft.AspNetCore.Components;

namespace BlazorEnterpriseStarter.Components;

/// <summary>
/// Représente un état vide prêt à l’emploi pour guider l’utilisateur lorsque des données manquent.
/// </summary>
public partial class AppEmptyState : ComponentBase
{
    /// <summary>
    /// Surtitre facultatif affiché au-dessus du titre.
    /// </summary>
    [Parameter]
    public string? Eyebrow { get; set; }

    /// <summary>
    /// Titre principal de l’état vide.
    /// </summary>
    [Parameter, EditorRequired]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Texte descriptif affiché sous le titre.
    /// </summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// Tonalité visuelle dominante de l’état vide.
    /// </summary>
    [Parameter]
    public AppTone Tone { get; set; } = AppTone.Neutral;

    /// <summary>
    /// Contenu visuel illustratif optionnel.
    /// </summary>
    [Parameter]
    public RenderFragment? VisualContent { get; set; }

    /// <summary>
    /// Zone d’actions affichée sous le texte.
    /// </summary>
    [Parameter]
    public RenderFragment? Actions { get; set; }

    /// <summary>
    /// Contenu complémentaire affiché en bas du composant.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Classe CSS additionnelle.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    private string CssClass => new CssClassBuilder()
        .Add("app-empty-state")
        .Add($"app-empty-state--{Tone.ToString().ToLowerInvariant()}")
        .Add(Class)
        .ToString();
}

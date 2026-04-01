using BlazorEnterpriseStarter.Components.Common;
using Microsoft.AspNetCore.Components;

namespace BlazorEnterpriseStarter.Components;

/// <summary>
/// Représente un badge visuel destiné à signaler un état ou une catégorie.
/// </summary>
public partial class AppBadge : ComponentBase
{
    /// <summary>
    /// Contenu textuel principal du badge.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Tonalité visuelle appliquée au badge.
    /// </summary>
    [Parameter]
    public AppTone Tone { get; set; } = AppTone.Neutral;

    /// <summary>
    /// Taille du badge.
    /// </summary>
    [Parameter]
    public AppControlSize Size { get; set; } = AppControlSize.Medium;

    /// <summary>
    /// Indique si le badge doit utiliser un rendu discret.
    /// </summary>
    [Parameter]
    public bool IsSubtle { get; set; } = true;

    /// <summary>
    /// Contenu optionnel affiché avant le libellé.
    /// </summary>
    [Parameter]
    public RenderFragment? LeadingContent { get; set; }

    /// <summary>
    /// Classe CSS additionnelle.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    private string CssClass => new CssClassBuilder()
        .Add("app-badge")
        .Add($"app-badge--{Tone.ToString().ToLowerInvariant()}")
        .Add($"app-badge--{Size.ToString().ToLowerInvariant()}")
        .AddIf("app-badge--subtle", IsSubtle)
        .Add(Class)
        .ToString();
}

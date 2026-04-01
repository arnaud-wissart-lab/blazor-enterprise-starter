using BlazorEnterpriseStarter.Components.Common;
using Microsoft.AspNetCore.Components;

namespace BlazorEnterpriseStarter.Components;

/// <summary>
/// Représente une carte applicative réutilisable pour structurer du contenu éditorial ou fonctionnel.
/// </summary>
public partial class AppCard : ComponentBase
{
    /// <summary>
    /// Surtitre facultatif affiché au-dessus du titre.
    /// </summary>
    [Parameter]
    public string? Eyebrow { get; set; }

    /// <summary>
    /// Titre principal de la carte.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Description facultative de la carte.
    /// </summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// Tonalité visuelle dominante de la carte.
    /// </summary>
    [Parameter]
    public AppTone Tone { get; set; } = AppTone.Neutral;

    /// <summary>
    /// Indique si la carte doit utiliser une densité plus compacte.
    /// </summary>
    [Parameter]
    public bool IsCompact { get; set; }

    /// <summary>
    /// Zone d’actions affichée dans l’en-tête.
    /// </summary>
    [Parameter]
    public RenderFragment? Actions { get; set; }

    /// <summary>
    /// Zone affichée dans le pied de carte.
    /// </summary>
    [Parameter]
    public RenderFragment? Footer { get; set; }

    /// <summary>
    /// Contenu principal de la carte.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Classe CSS additionnelle.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    private string CssClass => new CssClassBuilder()
        .Add("app-card")
        .Add($"app-card--{Tone.ToString().ToLowerInvariant()}")
        .AddIf("app-card--compact", IsCompact)
        .Add(Class)
        .ToString();
}

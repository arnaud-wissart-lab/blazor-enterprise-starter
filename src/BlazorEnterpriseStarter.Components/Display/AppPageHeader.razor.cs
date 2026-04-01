using Microsoft.AspNetCore.Components;

namespace BlazorEnterpriseStarter.Components;

/// <summary>
/// Représente un en-tête de page réutilisable avec titre, description et zone d’actions.
/// </summary>
public partial class AppPageHeader : ComponentBase
{
    /// <summary>
    /// Surtitre facultatif affiché au-dessus du titre.
    /// </summary>
    [Parameter]
    public string? Eyebrow { get; set; }

    /// <summary>
    /// Titre principal de la page.
    /// </summary>
    [Parameter, EditorRequired]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description de contexte affichée sous le titre.
    /// </summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// Zone de badges ou d’indicateurs affichée à côté du titre.
    /// </summary>
    [Parameter]
    public RenderFragment? Badges { get; set; }

    /// <summary>
    /// Zone d’actions affichée à droite.
    /// </summary>
    [Parameter]
    public RenderFragment? Actions { get; set; }

    /// <summary>
    /// Contenu complémentaire affiché sous la description.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Classe CSS additionnelle.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    private string CssClass => string.IsNullOrWhiteSpace(Class)
        ? "app-page-header"
        : $"app-page-header {Class}";
}

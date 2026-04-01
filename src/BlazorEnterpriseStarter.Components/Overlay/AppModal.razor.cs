using BlazorEnterpriseStarter.Components.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorEnterpriseStarter.Components;

/// <summary>
/// Représente une fenêtre modale applicative simple, pensée pour des contenus courts et des confirmations.
/// </summary>
public partial class AppModal : ComponentBase
{
    /// <summary>
    /// Indique si la modale est ouverte.
    /// </summary>
    [Parameter]
    public bool IsOpen { get; set; }

    /// <summary>
    /// Titre principal de la modale.
    /// </summary>
    [Parameter, EditorRequired]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description facultative affichée sous le titre.
    /// </summary>
    [Parameter]
    public string? Description { get; set; }

    /// <summary>
    /// Taille de la modale.
    /// </summary>
    [Parameter]
    public AppControlSize Size { get; set; } = AppControlSize.Medium;

    /// <summary>
    /// Indique si un clic sur le fond doit fermer la modale.
    /// </summary>
    [Parameter]
    public bool CloseOnBackdropClick { get; set; } = true;

    /// <summary>
    /// Indique si la touche Échap doit fermer la modale.
    /// </summary>
    [Parameter]
    public bool CloseOnEscape { get; set; } = true;

    /// <summary>
    /// Indique si le bouton de fermeture doit être affiché.
    /// </summary>
    [Parameter]
    public bool ShowCloseButton { get; set; } = true;

    /// <summary>
    /// Libellé accessible du bouton de fermeture.
    /// </summary>
    [Parameter]
    public string CloseButtonLabel { get; set; } = "Fermer la fenêtre";

    /// <summary>
    /// Contenu principal de la modale.
    /// </summary>
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Zone d’actions affichée dans le pied de modale.
    /// </summary>
    [Parameter]
    public RenderFragment? FooterContent { get; set; }

    /// <summary>
    /// Callback invoqué lors d’une demande de fermeture.
    /// </summary>
    [Parameter]
    public EventCallback OnClose { get; set; }

    /// <summary>
    /// Classe CSS additionnelle.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    private string TitleId { get; } = $"app-modal-title-{Guid.NewGuid():N}";

    private string DescriptionId { get; } = $"app-modal-description-{Guid.NewGuid():N}";

    private string? AriaDescribedBy => !string.IsNullOrWhiteSpace(Description) ? DescriptionId : null;

    private string CssClass => new CssClassBuilder()
        .Add("app-modal")
        .Add($"app-modal--{Size.ToString().ToLowerInvariant()}")
        .Add(Class)
        .ToString();

    private async Task HandleBackdropClickAsync()
    {
        if (!CloseOnBackdropClick)
        {
            return;
        }

        await HandleCloseAsync();
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        if (!CloseOnEscape || !string.Equals(args.Key, "Escape", StringComparison.Ordinal))
        {
            return;
        }

        await HandleCloseAsync();
    }

    private async Task HandleCloseAsync()
    {
        await OnClose.InvokeAsync();
    }
}

using BlazorEnterpriseStarter.Components.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorEnterpriseStarter.Components;

/// <summary>
/// Représente un bouton applicatif réutilisable, capable de se comporter comme un bouton ou un lien.
/// </summary>
public partial class AppButton : ComponentBase
{
    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Variante visuelle du bouton.
    /// </summary>
    [Parameter]
    public AppButtonVariant Variant { get; set; } = AppButtonVariant.Primary;

    /// <summary>
    /// Taille du bouton.
    /// </summary>
    [Parameter]
    public AppControlSize Size { get; set; } = AppControlSize.Medium;

    /// <summary>
    /// Type HTML du bouton lorsque le composant n’est pas rendu comme un lien.
    /// </summary>
    [Parameter]
    public AppButtonType Type { get; set; } = AppButtonType.Button;

    /// <summary>
    /// Indique si le composant doit occuper toute la largeur disponible.
    /// </summary>
    [Parameter]
    public bool IsBlock { get; set; }

    /// <summary>
    /// Indique si le bouton est désactivé.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Indique si le bouton affiche un état de chargement.
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <summary>
    /// Libellé accessible utilisé pendant le chargement.
    /// </summary>
    [Parameter]
    public string LoadingText { get; set; } = "Chargement en cours";

    /// <summary>
    /// URL de navigation optionnelle. Si renseignée, le composant est rendu comme un lien.
    /// </summary>
    [Parameter]
    public string? Href { get; set; }

    /// <summary>
    /// Cible optionnelle du lien.
    /// </summary>
    [Parameter]
    public string? Target { get; set; }

    /// <summary>
    /// Valeur de l’attribut rel pour les liens.
    /// </summary>
    [Parameter]
    public string? Rel { get; set; }

    /// <summary>
    /// Contenu visuel affiché avant le libellé principal.
    /// </summary>
    [Parameter]
    public RenderFragment? LeadingContent { get; set; }

    /// <summary>
    /// Contenu visuel affiché après le libellé principal.
    /// </summary>
    [Parameter]
    public RenderFragment? TrailingContent { get; set; }

    /// <summary>
    /// Classe CSS additionnelle.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Callback invoqué au clic.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    private bool IsLink => !string.IsNullOrWhiteSpace(Href);

    private bool CanInteract => !Disabled && !IsLoading;

    private string ButtonTypeValue => Type switch
    {
        AppButtonType.Submit => "submit",
        AppButtonType.Reset => "reset",
        _ => "button"
    };

    private string? ComputedRel =>
        !string.IsNullOrWhiteSpace(Rel)
            ? Rel
            : string.Equals(Target, "_blank", StringComparison.OrdinalIgnoreCase)
                ? "noreferrer noopener"
                : null;

    private string CssClass => new CssClassBuilder()
        .Add("app-button")
        .Add($"app-button--{Variant.ToString().ToLowerInvariant()}")
        .Add($"app-button--{Size.ToString().ToLowerInvariant()}")
        .AddIf("app-button--block", IsBlock)
        .AddIf("app-button--loading", IsLoading)
        .AddIf("app-button--disabled", !CanInteract)
        .Add(Class)
        .ToString();

    private async Task HandleClickAsync(MouseEventArgs args)
    {
        if (!CanInteract)
        {
            return;
        }

        await OnClick.InvokeAsync(args);
    }
}

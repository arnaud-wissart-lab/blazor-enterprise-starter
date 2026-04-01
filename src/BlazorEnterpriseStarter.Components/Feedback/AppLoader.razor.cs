using BlazorEnterpriseStarter.Components.Common;
using Microsoft.AspNetCore.Components;

namespace BlazorEnterpriseStarter.Components;

/// <summary>
/// Représente un indicateur de chargement simple et réutilisable.
/// </summary>
public partial class AppLoader : ComponentBase
{
    /// <summary>
    /// Taille de l’indicateur.
    /// </summary>
    [Parameter]
    public AppControlSize Size { get; set; } = AppControlSize.Medium;

    /// <summary>
    /// Libellé accessible ou visuel accompagnant le loader.
    /// </summary>
    [Parameter]
    public string? Label { get; set; } = "Chargement";

    /// <summary>
    /// Indique si le composant doit rester dans le flux en ligne.
    /// </summary>
    [Parameter]
    public bool Inline { get; set; }

    /// <summary>
    /// Classe CSS additionnelle.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    private string CssClass => new CssClassBuilder()
        .Add("app-loader")
        .Add($"app-loader--{Size.ToString().ToLowerInvariant()}")
        .AddIf("app-loader--inline", Inline)
        .Add(Class)
        .ToString();
}

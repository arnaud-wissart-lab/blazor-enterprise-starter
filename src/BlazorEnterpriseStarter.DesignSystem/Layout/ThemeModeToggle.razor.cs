using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorEnterpriseStarter.DesignSystem.Layout;

/// <summary>
/// Fournit un sélecteur simple entre mode clair et mode sombre pour l’interface.
/// </summary>
public partial class ThemeModeToggle : ComponentBase
{
    private string _themeActuel = "light";

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    private string LibelleVisible => _themeActuel == "dark" ? "Mode sombre" : "Mode clair";

    private string LibelleAccessible => _themeActuel == "dark"
        ? "Activer le mode clair"
        : "Activer le mode sombre";

    private string InfoBulle => _themeActuel == "dark"
        ? "Passer en mode clair"
        : "Passer en mode sombre";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _themeActuel = await JSRuntime.InvokeAsync<string>("blazorEnterpriseStarterTheme.getCurrentTheme");
        StateHasChanged();
    }

    private async Task BasculerAsync()
    {
        var themeCible = _themeActuel == "dark" ? "light" : "dark";
        _themeActuel = await JSRuntime.InvokeAsync<string>("blazorEnterpriseStarterTheme.setTheme", themeCible);
    }
}

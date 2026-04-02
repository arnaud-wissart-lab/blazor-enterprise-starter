using Bunit;
using BlazorEnterpriseStarter.Components;
using BlazorEnterpriseStarter.Components.Common;

namespace BlazorEnterpriseStarter.Tests.Ui.Composants;

public sealed class AppButtonTests : IDisposable
{
    private readonly BunitContext _context = new();

    [Fact]
    public void AppButton_devrait_declencher_le_callback_quand_le_bouton_est_actif()
    {
        var clics = 0;

        var component = _context.Render<AppButton>(parameters => parameters
            .Add(button => button.ChildContent, "Créer")
            .Add(button => button.OnClick, () => clics++));

        component.Find("button").Click();

        Assert.Equal(1, clics);
    }

    [Fact]
    public void AppButton_devrait_se_rendre_comme_un_lien_avec_rel_de_securite()
    {
        var component = _context.Render<AppButton>(parameters => parameters
            .Add(button => button.ChildContent, "Voir la documentation")
            .Add(button => button.Href, "https://example.test/docs")
            .Add(button => button.Target, "_blank"));

        var lien = component.Find("a");

        Assert.Equal("https://example.test/docs", lien.GetAttribute("href"));
        Assert.Equal("_blank", lien.GetAttribute("target"));
        Assert.Equal("noreferrer noopener", lien.GetAttribute("rel"));
    }

    [Fact]
    public void AppButton_ne_devrait_pas_declencher_le_callback_pendant_le_chargement()
    {
        var clics = 0;

        var component = _context.Render<AppButton>(parameters => parameters
            .Add(button => button.ChildContent, "Enregistrer")
            .Add(button => button.IsLoading, true)
            .Add(button => button.LoadingText, "Enregistrement en cours")
            .Add(button => button.OnClick, () => clics++));

        var button = component.Find("button");

        button.Click();

        Assert.Equal(0, clics);
        Assert.True(button.HasAttribute("aria-busy"));
        Assert.Contains("app-button--loading", button.ClassList);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

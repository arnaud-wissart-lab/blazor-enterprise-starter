using Bunit;
using BlazorEnterpriseStarter.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorEnterpriseStarter.Tests.Ui.Composants;

public sealed class AppModalTests : IDisposable
{
    private readonly BunitContext _context = new();

    [Fact]
    public void AppModal_ne_devrait_rien_afficher_quand_la_modale_est_fermee()
    {
        var component = _context.Render<AppModal>(parameters => parameters
            .Add(modal => modal.IsOpen, false)
            .Add(modal => modal.Title, "Supprimer un élément")
            .Add(modal => modal.ChildContent, "Contenu"));

        Assert.Empty(component.Markup.Trim());
    }

    [Fact]
    public void AppModal_devrait_declencher_la_fermeture_sur_un_clic_de_fond()
    {
        var fermetures = 0;

        var component = _context.Render<AppModal>(parameters => parameters
            .Add(modal => modal.IsOpen, true)
            .Add(modal => modal.Title, "Supprimer un élément")
            .Add(modal => modal.ChildContent, "Contenu")
            .Add(modal => modal.OnClose, () => fermetures++));

        component.Find(".app-modal__backdrop").Click();

        Assert.Equal(1, fermetures);
    }

    [Fact]
    public void AppModal_devrait_ignorer_la_touche_echap_si_la_fermeture_clavier_est_desactivee()
    {
        var fermetures = 0;

        var component = _context.Render<AppModal>(parameters => parameters
            .Add(modal => modal.IsOpen, true)
            .Add(modal => modal.Title, "Supprimer un élément")
            .Add(modal => modal.ChildContent, "Contenu")
            .Add(modal => modal.CloseOnEscape, false)
            .Add(modal => modal.OnClose, () => fermetures++));

        component.Find(".app-modal").KeyDown(new KeyboardEventArgs { Key = "Escape" });

        Assert.Equal(0, fermetures);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

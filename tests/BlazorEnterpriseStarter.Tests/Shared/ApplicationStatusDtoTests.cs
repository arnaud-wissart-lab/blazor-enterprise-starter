using BlazorEnterpriseStarter.Shared.Contracts;

namespace BlazorEnterpriseStarter.Tests.Shared;

public class ApplicationStatusDtoTests
{
    [Fact]
    public void CreateHealthy_devrait_marquer_le_statut_comme_sain()
    {
        var before = DateTimeOffset.UtcNow;

        var status = ApplicationStatusDto.CreateHealthy(
            "BlazorEnterpriseStarter.Server",
            "Development",
            "Le backend répond correctement.");

        var after = DateTimeOffset.UtcNow;

        Assert.True(status.IsHealthy);
        Assert.Equal("BlazorEnterpriseStarter.Server", status.ApplicationName);
        Assert.Equal("Development", status.EnvironmentName);
        Assert.Equal("Le backend répond correctement.", status.Message);
        Assert.InRange(status.CheckedAtUtc, before, after);
    }

    [Fact]
    public void CreateUnavailable_devrait_marquer_le_statut_comme_indisponible()
    {
        var status = ApplicationStatusDto.CreateUnavailable(
            "BlazorEnterpriseStarter.Server",
            "Indisponible",
            "Impossible de joindre l’API.");

        Assert.False(status.IsHealthy);
        Assert.Equal("Indisponible", status.EnvironmentName);
        Assert.Equal("Impossible de joindre l’API.", status.Message);
    }
}

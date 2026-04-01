using BlazorEnterpriseStarter.Shared.Contracts;

namespace BlazorEnterpriseStarter.Server.Endpoints;

public static class SystemStatusEndpoints
{
    public static IEndpointRouteBuilder MapSystemStatusEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(ApiRoutes.System.Status, (IHostEnvironment environment) =>
        {
            var status = ApplicationStatusDto.CreateHealthy(
                "BlazorEnterpriseStarter.Server",
                environment.EnvironmentName,
                "Le backend est opérationnel et prêt à accueillir les futures fonctionnalités métier.");

            return Results.Ok(status);
        })
        .WithName("ObtenirEtatPlateforme");

        return endpoints;
    }
}

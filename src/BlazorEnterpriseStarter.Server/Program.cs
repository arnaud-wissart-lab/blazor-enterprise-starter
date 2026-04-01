using BlazorEnterpriseStarter.Server.Application.Backlog;
using BlazorEnterpriseStarter.Server.Endpoints;
using BlazorEnterpriseStarter.Server.Infrastructure.Backlog;

namespace BlazorEnterpriseStarter.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();
        builder.Services.AddProblemDetails();
        builder.Services.AddSingleton<IBacklogRepository, InMemoryBacklogRepository>();
        builder.Services.AddScoped<IBacklogService, BacklogService>();

        var app = builder.Build();

        app.UseExceptionHandler();

        if (!IsRunningInContainer())
        {
            app.UseHttpsRedirection();
        }

        app.MapGet("/", () =>
        {
            return Results.Ok(new
            {
                service = "BlazorEnterpriseStarter.Server",
                message = "Le backend est démarré. Utilisez l’endpoint de statut pour vérifier sa disponibilité."
            });
        });

        app.MapSystemStatusEndpoints();
        app.MapBacklogEndpoints();
        app.MapDefaultEndpoints();

        app.Run();
    }

    private static bool IsRunningInContainer() =>
        bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out var isRunningInContainer)
        && isRunningInContainer;
}

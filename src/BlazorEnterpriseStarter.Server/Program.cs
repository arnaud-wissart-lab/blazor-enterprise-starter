using BlazorEnterpriseStarter.Server.Application.Backlog;
using BlazorEnterpriseStarter.Server.Endpoints;
using BlazorEnterpriseStarter.Server.Infrastructure.Persistence;

namespace BlazorEnterpriseStarter.Server;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();
        builder.Services.AddProblemDetails();
        builder.Services.AddBacklogPersistence(builder.Configuration, builder.Environment);
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

        await BacklogDatabaseInitializer.InitializeAsync(app.Services, CancellationToken.None);

        await app.RunAsync();
    }

    private static bool IsRunningInContainer() =>
        bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out var isRunningInContainer)
        && isRunningInContainer;
}

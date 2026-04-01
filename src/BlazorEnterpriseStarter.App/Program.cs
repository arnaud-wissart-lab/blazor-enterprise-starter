using BlazorEnterpriseStarter.App.Components;
using BlazorEnterpriseStarter.App.Services;

namespace BlazorEnterpriseStarter.App;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var platformApiBaseUrl = builder.Configuration["PlatformApi:BaseUrl"];

        builder.Services.AddHttpClient<PlatformApiClient>(client =>
        {
            client.BaseAddress = Uri.TryCreate(platformApiBaseUrl, UriKind.Absolute, out var configuredBaseAddress)
                ? configuredBaseAddress
                : new Uri("https+http://server");
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

        if (!IsRunningInContainer())
        {
            app.UseHttpsRedirection();
        }

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<Components.App>()
            .AddInteractiveServerRenderMode();
        app.MapDefaultEndpoints();

        app.Run();
    }

    private static bool IsRunningInContainer() =>
        bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out var isRunningInContainer)
        && isRunningInContainer;
}

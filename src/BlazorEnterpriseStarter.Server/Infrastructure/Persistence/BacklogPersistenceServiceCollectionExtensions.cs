using BlazorEnterpriseStarter.Server.Application.Backlog;
using BlazorEnterpriseStarter.Server.Infrastructure.Backlog;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BlazorEnterpriseStarter.Server.Infrastructure.Persistence;

/// <summary>
/// Enregistre la persistence SQLite du backlog et normalise sa configuration.
/// </summary>
public static class BacklogPersistenceServiceCollectionExtensions
{
    private const string DefaultConnectionString = "Data Source=data/blazor-enterprise-starter.db";

    public static IServiceCollection AddBacklogPersistence(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var connectionString = ResolveConnectionString(
            configuration.GetConnectionString("BacklogDatabase"),
            environment.ContentRootPath);

        services.AddDbContext<BacklogDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IBacklogRepository, SqliteBacklogRepository>();

        return services;
    }

    private static string ResolveConnectionString(string? rawConnectionString, string contentRootPath)
    {
        var builder = new SqliteConnectionStringBuilder(rawConnectionString ?? DefaultConnectionString);

        if (string.IsNullOrWhiteSpace(builder.DataSource))
        {
            builder.DataSource = Path.Combine(contentRootPath, "data", "blazor-enterprise-starter.db");
        }
        else if (!Path.IsPathRooted(builder.DataSource))
        {
            builder.DataSource = Path.GetFullPath(Path.Combine(contentRootPath, builder.DataSource));
        }

        var directory = Path.GetDirectoryName(builder.DataSource);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return builder.ConnectionString;
    }
}

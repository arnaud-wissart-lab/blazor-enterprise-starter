using BlazorEnterpriseStarter.Server.Domain.Backlog;
using BlazorEnterpriseStarter.Server.Infrastructure.Backlog;
using BlazorEnterpriseStarter.Server.Infrastructure.Persistence;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorEnterpriseStarter.Tests.Backlog;

public sealed class BacklogSqlitePersistenceTests
{
    [Fact]
    public async Task SqliteBacklogRepository_devrait_persister_un_element_entre_deux_contextes()
    {
        var databasePath = CreateDatabasePath();
        var options = CreateOptions(databasePath);

        try
        {
            await using (var creationContext = new BacklogDbContext(options))
            {
                await creationContext.Database.EnsureCreatedAsync();
            }

            var item = BacklogItem.Create(
                "Préparer la revue de sprint",
                "Structurer les éléments à arbitrer avec l’équipe produit.",
                BacklogItemStatus.Pret,
                BacklogItemPriority.Haute,
                new DateTimeOffset(2026, 04, 02, 08, 30, 00, TimeSpan.Zero));

            await using (var writeContext = new BacklogDbContext(options))
            {
                var repository = new SqliteBacklogRepository(writeContext);
                await repository.AjouterAsync(item, CancellationToken.None);
            }

            await using (var readContext = new BacklogDbContext(options))
            {
                var repository = new SqliteBacklogRepository(readContext);
                var resultat = await repository.ListerAsync(CancellationToken.None);

                var persistedItem = Assert.Single(resultat);
                Assert.Equal(item.Id, persistedItem.Id);
                Assert.Equal("Préparer la revue de sprint", persistedItem.Titre);
                Assert.Equal(BacklogItemPriority.Haute, persistedItem.Priorite);
            }
        }
        finally
        {
            DeleteDatabasePath(databasePath);
        }
    }

    [Fact]
    public async Task BacklogDatabaseInitializer_devrait_creer_et_seeder_la_base_vide()
    {
        var databasePath = CreateDatabasePath();
        var services = new ServiceCollection();

        services.AddDbContext<BacklogDbContext>(options =>
            options.UseSqlite(CreateConnectionString(databasePath)));

        var provider = services.BuildServiceProvider();

        try
        {
            await BacklogDatabaseInitializer.InitializeAsync(provider, CancellationToken.None);

            await using var scope = provider.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BacklogDbContext>();

            var count = await dbContext.BacklogItems.CountAsync();

            Assert.True(count >= 10);
        }
        finally
        {
            await provider.DisposeAsync();
            DeleteDatabasePath(databasePath);
        }
    }

    private static DbContextOptions<BacklogDbContext> CreateOptions(string databasePath) =>
        new DbContextOptionsBuilder<BacklogDbContext>()
            .UseSqlite(CreateConnectionString(databasePath))
            .Options;

    private static string CreateConnectionString(string databasePath) =>
        $"Data Source={databasePath};Pooling=False";

    private static string CreateDatabasePath()
    {
        var directory = Path.Combine(Path.GetTempPath(), "bes-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, "backlog.db");
    }

    private static void DeleteDatabasePath(string databasePath)
    {
        var directory = Path.GetDirectoryName(databasePath);

        if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
        {
            return;
        }

        try
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                File.Delete(file);
            }

            Directory.Delete(directory, recursive: true);
        }
        catch (IOException)
        {
            // Le nettoyage reste opportuniste pour éviter les faux négatifs liés au verrouillage SQLite.
        }
    }
}

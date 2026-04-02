using Microsoft.EntityFrameworkCore;

namespace BlazorEnterpriseStarter.Server.Infrastructure.Persistence;

/// <summary>
/// Crée la base SQLite et injecte un jeu initial si aucun élément n’est encore présent.
/// </summary>
public static class BacklogDatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BacklogDbContext>();

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await dbContext.BacklogItems.AnyAsync(cancellationToken))
        {
            return;
        }

        await dbContext.BacklogItems.AddRangeAsync(BacklogSeedData.CreateInitialItems(), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

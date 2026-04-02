using BlazorEnterpriseStarter.Server.Application.Backlog;
using BlazorEnterpriseStarter.Server.Domain.Backlog;
using BlazorEnterpriseStarter.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BlazorEnterpriseStarter.Server.Infrastructure.Backlog;

/// <summary>
/// Implémentation SQLite du dépôt backlog au-dessus d’EF Core.
/// </summary>
public sealed class SqliteBacklogRepository(BacklogDbContext dbContext) : IBacklogRepository
{
    public async Task<IReadOnlyList<BacklogItem>> ListerAsync(CancellationToken cancellationToken)
    {
        var items = await dbContext.BacklogItems
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return items.Select(MapToDomain).ToArray();
    }

    public async Task<BacklogItem?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = await dbContext.BacklogItems
            .AsNoTracking()
            .FirstOrDefaultAsync(current => current.Id == id, cancellationToken);

        return item is null ? null : MapToDomain(item);
    }

    public async Task AjouterAsync(BacklogItem item, CancellationToken cancellationToken)
    {
        await dbContext.BacklogItems.AddAsync(MapToEntity(item), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task MettreAJourAsync(BacklogItem item, CancellationToken cancellationToken)
    {
        var existingItem = await dbContext.BacklogItems
            .FirstOrDefaultAsync(current => current.Id == item.Id, cancellationToken);

        if (existingItem is null)
        {
            return;
        }

        existingItem.Titre = item.Titre;
        existingItem.Description = item.Description;
        existingItem.Statut = item.Statut;
        existingItem.Priorite = item.Priorite;
        existingItem.DateCreation = item.DateCreation;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SupprimerAsync(Guid id, CancellationToken cancellationToken)
    {
        var existingItem = await dbContext.BacklogItems
            .FirstOrDefaultAsync(current => current.Id == id, cancellationToken);

        if (existingItem is null)
        {
            return false;
        }

        dbContext.BacklogItems.Remove(existingItem);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static BacklogItem MapToDomain(BacklogItemEntity entity) =>
        new(
            entity.Id,
            entity.Titre,
            entity.Description,
            entity.Statut,
            entity.Priorite,
            entity.DateCreation);

    private static BacklogItemEntity MapToEntity(BacklogItem item) =>
        new()
        {
            Id = item.Id,
            Titre = item.Titre,
            Description = item.Description,
            Statut = item.Statut,
            Priorite = item.Priorite,
            DateCreation = item.DateCreation
        };
}

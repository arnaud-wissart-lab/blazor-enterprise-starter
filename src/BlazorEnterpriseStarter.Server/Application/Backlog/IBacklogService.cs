using BlazorEnterpriseStarter.Shared.Contracts;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;

namespace BlazorEnterpriseStarter.Server.Application.Backlog;

/// <summary>
/// Orchestre les cas d’usage du backlog produit.
/// </summary>
public interface IBacklogService
{
    Task<PagedResultDto<BacklogItemDto>> ListerAsync(BacklogItemsQueryDto requete, CancellationToken cancellationToken);

    Task<BacklogItemDto?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken);

    Task<BacklogItemDto> CreerAsync(BacklogItemUpsertRequest commande, CancellationToken cancellationToken);

    Task<BacklogItemDto?> ModifierAsync(Guid id, BacklogItemUpsertRequest commande, CancellationToken cancellationToken);

    Task<bool> SupprimerAsync(Guid id, CancellationToken cancellationToken);
}

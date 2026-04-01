using BlazorEnterpriseStarter.Shared.Contracts;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;

namespace BlazorEnterpriseStarter.App.Services;

/// <summary>
/// Définit le contrat d’accès HTTP au module backlog.
/// </summary>
public interface IBacklogApiClient
{
    Task<PagedResultDto<BacklogItemDto>> ListerAsync(BacklogItemsQueryDto requete, CancellationToken cancellationToken);

    Task<BacklogItemDto> CreerAsync(BacklogItemUpsertRequest commande, CancellationToken cancellationToken);

    Task<BacklogItemDto> ModifierAsync(Guid id, BacklogItemUpsertRequest commande, CancellationToken cancellationToken);

    Task SupprimerAsync(Guid id, CancellationToken cancellationToken);
}

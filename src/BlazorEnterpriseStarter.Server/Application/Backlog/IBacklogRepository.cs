using BlazorEnterpriseStarter.Server.Domain.Backlog;

namespace BlazorEnterpriseStarter.Server.Application.Backlog;

/// <summary>
/// Abstraction de persistence des éléments de backlog.
/// </summary>
public interface IBacklogRepository
{
    Task<IReadOnlyList<BacklogItem>> ListerAsync(CancellationToken cancellationToken);

    Task<BacklogItem?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken);

    Task AjouterAsync(BacklogItem item, CancellationToken cancellationToken);

    Task MettreAJourAsync(BacklogItem item, CancellationToken cancellationToken);

    Task<bool> SupprimerAsync(Guid id, CancellationToken cancellationToken);
}

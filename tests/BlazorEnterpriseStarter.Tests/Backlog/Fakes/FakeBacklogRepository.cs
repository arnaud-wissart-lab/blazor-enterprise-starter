using BlazorEnterpriseStarter.Server.Application.Backlog;
using BlazorEnterpriseStarter.Server.Domain.Backlog;

namespace BlazorEnterpriseStarter.Tests.Backlog.Fakes;

internal sealed class FakeBacklogRepository : IBacklogRepository
{
    private readonly List<BacklogItem> _items;

    public FakeBacklogRepository(IEnumerable<BacklogItem>? items = null)
    {
        _items = items?.ToList() ?? [];
    }

    public Task<IReadOnlyList<BacklogItem>> ListerAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<BacklogItem>>(_items.ToArray());
    }

    public Task<BacklogItem?> ObtenirParIdAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_items.FirstOrDefault(item => item.Id == id));
    }

    public Task AjouterAsync(BacklogItem item, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _items.Add(item);
        return Task.CompletedTask;
    }

    public Task MettreAJourAsync(BacklogItem item, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var index = _items.FindIndex(current => current.Id == item.Id);

        if (index >= 0)
        {
            _items[index] = item;
        }

        return Task.CompletedTask;
    }

    public Task<bool> SupprimerAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_items.RemoveAll(item => item.Id == id) > 0);
    }
}

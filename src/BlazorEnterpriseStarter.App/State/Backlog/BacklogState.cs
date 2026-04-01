using BlazorEnterpriseStarter.App.Services;
using BlazorEnterpriseStarter.Shared.Contracts;
using BlazorEnterpriseStarter.Shared.Contracts.Backlog;

namespace BlazorEnterpriseStarter.App.State.Backlog;

/// <summary>
/// Porte l’état local du module backlog pour une session utilisateur.
/// </summary>
public sealed class BacklogState
{
    private readonly IBacklogApiClient _apiClient;

    public BacklogState(IBacklogApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public event Action? Changed;

    public BacklogItemsQueryDto Query { get; } = new();

    public PagedResultDto<BacklogItemDto>? Result { get; private set; }

    public BacklogRequestStatus ListStatus { get; private set; } = BacklogRequestStatus.Idle;

    public BacklogRequestStatus MutationStatus { get; private set; } = BacklogRequestStatus.Idle;

    public string? ListErrorMessage { get; private set; }

    public string? MutationErrorMessage { get; private set; }

    public IReadOnlyDictionary<string, string[]> MutationErrors { get; private set; } =
        new Dictionary<string, string[]>();

    public bool HasLoadedOnce { get; private set; }

    public IReadOnlyList<BacklogItemDto> Items => Result?.Elements ?? [];

    public int TotalCount => Result?.NombreTotal ?? 0;

    public int CurrentPage => Result?.NumeroPage ?? Query.NumeroPage;

    public int TotalPages => Result?.NombrePages ?? 0;

    public bool CanGoPrevious => CurrentPage > 1;

    public bool CanGoNext => Result is not null && CurrentPage < TotalPages;

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        if (HasLoadedOnce && Result is not null)
        {
            return;
        }

        await LoadAsync(cancellationToken);
    }

    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
    }

    public async Task ApplyFiltersAsync(BacklogItemsQueryDto source, CancellationToken cancellationToken)
    {
        CopyQuery(source, Query);
        Query.NumeroPage = 1;

        await LoadAsync(cancellationToken);
    }

    public async Task GoToPreviousPageAsync(CancellationToken cancellationToken)
    {
        if (!CanGoPrevious)
        {
            return;
        }

        Query.NumeroPage--;
        await LoadAsync(cancellationToken);
    }

    public async Task GoToNextPageAsync(CancellationToken cancellationToken)
    {
        if (!CanGoNext)
        {
            return;
        }

        Query.NumeroPage++;
        await LoadAsync(cancellationToken);
    }

    public async Task CreateAsync(BacklogItemUpsertRequest request, CancellationToken cancellationToken)
    {
        await ExecuteMutationAsync(
            async token => await _apiClient.CreerAsync(request, token),
            false,
            cancellationToken);
    }

    public async Task UpdateAsync(Guid id, BacklogItemUpsertRequest request, CancellationToken cancellationToken)
    {
        await ExecuteMutationAsync(
            async token => await _apiClient.ModifierAsync(id, request, token),
            false,
            cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var mustGoBackOnePage = Result is not null && Result.Elements.Count == 1 && CurrentPage > 1;

        await ExecuteMutationAsync(
            async token =>
            {
                await _apiClient.SupprimerAsync(id, token);
                return null;
            },
            mustGoBackOnePage,
            cancellationToken);
    }

    public void ResetMutationFeedback()
    {
        MutationStatus = BacklogRequestStatus.Idle;
        MutationErrorMessage = null;
        MutationErrors = new Dictionary<string, string[]>();
        NotifyChanged();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        ListStatus = BacklogRequestStatus.Loading;
        ListErrorMessage = null;
        NotifyChanged();

        try
        {
            Result = await _apiClient.ListerAsync(CloneQuery(Query), cancellationToken);
            ListStatus = BacklogRequestStatus.Success;
        }
        catch (BacklogApiException exception)
        {
            Result = null;
            ListStatus = BacklogRequestStatus.Error;
            ListErrorMessage = exception.Message;
        }
        finally
        {
            HasLoadedOnce = true;
            NotifyChanged();
        }
    }

    private async Task ExecuteMutationAsync(
        Func<CancellationToken, Task<object?>> action,
        bool decrementPageBeforeReload,
        CancellationToken cancellationToken)
    {
        MutationStatus = BacklogRequestStatus.Loading;
        MutationErrorMessage = null;
        MutationErrors = new Dictionary<string, string[]>();
        NotifyChanged();

        try
        {
            await action(cancellationToken);
            MutationStatus = BacklogRequestStatus.Success;

            if (decrementPageBeforeReload)
            {
                Query.NumeroPage--;
            }

            await LoadAsync(cancellationToken);
        }
        catch (BacklogApiException exception)
        {
            MutationStatus = BacklogRequestStatus.Error;
            MutationErrorMessage = exception.Message;
            MutationErrors = exception.Errors;
            NotifyChanged();
            throw;
        }
        finally
        {
            if (MutationStatus == BacklogRequestStatus.Success)
            {
                NotifyChanged();
            }
        }
    }

    private void NotifyChanged() => Changed?.Invoke();

    private static void CopyQuery(BacklogItemsQueryDto source, BacklogItemsQueryDto destination)
    {
        destination.Recherche = source.Recherche;
        destination.Statut = source.Statut;
        destination.Priorite = source.Priorite;
        destination.Tri = source.Tri;
        destination.Direction = source.Direction;
        destination.NumeroPage = source.NumeroPage;
        destination.TaillePage = source.TaillePage;
    }

    private static BacklogItemsQueryDto CloneQuery(BacklogItemsQueryDto source) =>
        new()
        {
            Recherche = source.Recherche,
            Statut = source.Statut,
            Priorite = source.Priorite,
            Tri = source.Tri,
            Direction = source.Direction,
            NumeroPage = source.NumeroPage,
            TaillePage = source.TaillePage
        };
}

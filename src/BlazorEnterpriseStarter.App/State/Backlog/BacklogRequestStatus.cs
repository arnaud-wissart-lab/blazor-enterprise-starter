namespace BlazorEnterpriseStarter.App.State.Backlog;

/// <summary>
/// Représente l’état courant d’une opération asynchrone du module backlog.
/// </summary>
public enum BacklogRequestStatus
{
    Idle,
    Loading,
    Success,
    Error
}

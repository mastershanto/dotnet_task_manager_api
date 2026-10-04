namespace Notifications.Domain;

public interface INotificationsRepository
{
    Task<NotificationsDataModel> GetNotificationsAsync(string? filter, CancellationToken cancellationToken = default);
    Task<bool> MarkSeenAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> DeleteNotificationAsync(int id, CancellationToken cancellationToken = default);
}

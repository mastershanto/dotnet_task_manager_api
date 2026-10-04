namespace Notifications.Domain;

public record NotificationItemModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string Type { get; init; } = "general";
    public object? Data { get; init; }
    public bool IsSeen { get; init; } = false;
    public string? SeenAt { get; init; }
    public string CreatedAt { get; init; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
}

public record NotificationsDataModel
{
    public int UnseenCount { get; init; }
    public List<NotificationItemModel> Notifications { get; init; } = new();
}

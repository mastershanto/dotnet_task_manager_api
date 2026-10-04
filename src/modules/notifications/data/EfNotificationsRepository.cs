using BuildingBlocks.Persistence;
using Notifications.Domain;

namespace Notifications.Data;

public class EfNotificationsRepository : INotificationsRepository
{
    private readonly AppDbContext _db;
    private static readonly List<NotificationItemModel> _inMemory = new()
    {
        new NotificationItemModel
        {
            Id = 1,
            Title = "Upcoming Class Reminder",
            Message = "You have Salsa Cubana Beginners starting in 2 hours at Main Dance Hall.",
            Type = "major",
            Data = new { class_id = 1, start_time = "18:00:00" },
            IsSeen = false,
            SeenAt = null,
            CreatedAt = DateTime.UtcNow.AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss")
        },
        new NotificationItemModel
        {
            Id = 2,
            Title = "Partner Request Accepted",
            Message = "Amara Williams accepted your dance partner request!",
            Type = "major",
            Data = new { partner_id = 4 },
            IsSeen = false,
            SeenAt = null,
            CreatedAt = DateTime.UtcNow.AddHours(-2).ToString("yyyy-MM-dd HH:mm:ss")
        },
        new NotificationItemModel
        {
            Id = 3,
            Title = "New Achievement Unlocked",
            Message = "Congratulations! You earned the Latin Rhythm Master badge.",
            Type = "minor",
            Data = new { achievement_id = 1 },
            IsSeen = true,
            SeenAt = DateTime.UtcNow.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss"),
            CreatedAt = DateTime.UtcNow.AddHours(-4).ToString("yyyy-MM-dd HH:mm:ss")
        }
    };

    public EfNotificationsRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<NotificationsDataModel> GetNotificationsAsync(string? filter, CancellationToken cancellationToken = default)
    {
        var filtered = _inMemory.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            if (filter.Equals("major", StringComparison.OrdinalIgnoreCase))
                filtered = filtered.Where(n => n.Type.Equals("major", StringComparison.OrdinalIgnoreCase));
            else if (filter.Equals("minor", StringComparison.OrdinalIgnoreCase))
                filtered = filtered.Where(n => n.Type.Equals("minor", StringComparison.OrdinalIgnoreCase));
        }

        var list = filtered.OrderByDescending(n => n.CreatedAt).ToList();
        var model = new NotificationsDataModel
        {
            UnseenCount = list.Count(n => !n.IsSeen),
            Notifications = list
        };

        return Task.FromResult(model);
    }

    public Task<bool> MarkSeenAsync(int id, CancellationToken cancellationToken = default)
    {
        var found = _inMemory.FirstOrDefault(n => n.Id == id);
        if (found != null)
        {
            var index = _inMemory.IndexOf(found);
            _inMemory[index] = found with { IsSeen = true, SeenAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") };
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> DeleteNotificationAsync(int id, CancellationToken cancellationToken = default)
    {
        var found = _inMemory.FirstOrDefault(n => n.Id == id);
        if (found != null)
        {
            _inMemory.Remove(found);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}

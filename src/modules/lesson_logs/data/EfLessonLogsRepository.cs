using System.Collections.Concurrent;
using LessonLogs.Domain;

namespace LessonLogs.Data;

public class EfLessonLogsRepository : ILessonLogsRepository
{
    private static readonly ConcurrentDictionary<int, LessonLogModel> _logs = new();
    private static int _nextId = 100;

    static EfLessonLogsRepository()
    {
        SeedDefaults();
    }

    private static void SeedDefaults()
    {
        var sample1 = new LessonLogModel
        {
            Id = 1,
            UserId = 4,
            CreatedById = 1,
            CreatorName = "Alex Rivera",
            Comment = "Worked on Rumba basic box step and Cuban motion. Great improvement in foot placement.",
            IsOwn = true,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };
        var sample2 = new LessonLogModel
        {
            Id = 2,
            UserId = 4,
            CreatedById = 1,
            CreatorName = "Alex Rivera",
            Comment = "Introduced Cha Cha lock step. Focus on rhythm and count for next session.",
            IsOwn = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _logs.TryAdd(sample1.Id, sample1);
        _logs.TryAdd(sample2.Id, sample2);
    }

    public Task<LessonLogsResponseData> GetLogsAsync(int? userId, string? date, CancellationToken cancellationToken = default)
    {
        var items = _logs.Values.AsEnumerable();
        if (userId.HasValue)
        {
            items = items.Where(l => l.UserId == userId.Value);
        }
        if (!string.IsNullOrWhiteSpace(date))
        {
            items = items.Where(l => l.CreatedAt.ToString("yyyy-MM-dd") == date);
        }

        var logList = items.OrderByDescending(l => l.CreatedAt).Select(l => new LessonLogItemDto(
            l.Id,
            l.Comment,
            l.CreatorName,
            l.IsOwn,
            l.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        )).ToList();

        var grouped = logList
            .GroupBy(l => l.CreatedAt.Split(' ')[0])
            .ToDictionary(g => g.Key, g => g.ToList());

        var res = new LessonLogsResponseData(userId, date, logList, grouped);
        return Task.FromResult(res);
    }

    public Task<LessonLogModel> CreateLogAsync(CreateLessonLogDto dto, CancellationToken cancellationToken = default)
    {
        var id = Interlocked.Increment(ref _nextId);
        var model = new LessonLogModel
        {
            Id = id,
            UserId = dto.UserId,
            CreatedById = 1,
            CreatorName = "Alex Rivera",
            Comment = dto.Comment,
            IsOwn = true,
            CreatedAt = DateTime.UtcNow
        };
        _logs[id] = model;
        return Task.FromResult(model);
    }

    public Task<LessonLogModel?> UpdateLogAsync(int id, UpdateLessonLogDto dto, CancellationToken cancellationToken = default)
    {
        if (_logs.TryGetValue(id, out var existing))
        {
            var updated = existing with
            {
                Comment = dto.Comment,
                UpdatedAt = DateTime.UtcNow
            };
            _logs[id] = updated;
            return Task.FromResult<LessonLogModel?>(updated);
        }
        return Task.FromResult<LessonLogModel?>(null);
    }

    public Task<bool> DeleteLogAsync(int id, CancellationToken cancellationToken = default)
    {
        _logs.TryRemove(id, out _);
        return Task.FromResult(true);
    }
}

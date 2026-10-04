using BuildingBlocks.Persistence;
using Progress.Domain;

namespace Progress.Data;

public class EfProgressRepository : IProgressRepository
{
    private readonly AppDbContext _db;
    private static string _aboutMe = "Passionate Latin dancer focusing on Bachata Sensual and Cuban Salsa styling.";

    public EfProgressRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<StudentProgressModel> GetProgressAsync(int? studentId, CancellationToken cancellationToken = default)
    {
        var model = new StudentProgressModel
        {
            Student = new(studentId ?? 4, "Amara Williams", "https://i.pravatar.cc/150?u=amara", "amara@example.com"),
            MemberSince = "January 2025",
            AboutMe = _aboutMe,
            Streak = new(7, 14, DateTime.UtcNow.ToString("yyyy-MM-dd")),
            LevelProgress = new("Intermediate II", "Advanced I", 70.0, 120),
            Rewards = new(1420, 9),
            RecentAchievements = new()
            {
                new(1, "Latin Rhythm Master", "Completed 10 Salsa and Bachata classes in a single month", "trophy", DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-dd")),
                new(2, "Perfect Attendance", "Attended all registered sessions for 4 consecutive weeks", "badge", DateTime.UtcNow.AddDays(-10).ToString("yyyy-MM-dd"))
            },
            RecentMilestones = new()
            {
                new(1, "Promoted to Intermediate II", DateTime.UtcNow.AddMonths(-1).ToString("yyyy-MM-dd"), "Level Up"),
                new(2, "First Showcase Performance", DateTime.UtcNow.AddMonths(-2).ToString("yyyy-MM-dd"), "Performance")
            }
        };

        return Task.FromResult(model);
    }

    public Task<IEnumerable<ProgressAchievementInfo>> GetRecentAchievementsAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<ProgressAchievementInfo>
        {
            new(1, "Latin Rhythm Master", "Completed 10 Salsa and Bachata classes in a single month", "trophy", DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-dd")),
            new(2, "Perfect Attendance", "Attended all registered sessions for 4 consecutive weeks", "badge", DateTime.UtcNow.AddDays(-10).ToString("yyyy-MM-dd"))
        };
        return Task.FromResult<IEnumerable<ProgressAchievementInfo>>(list);
    }

    public Task<IEnumerable<ProgressMilestoneInfo>> GetRecentMilestonesAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<ProgressMilestoneInfo>
        {
            new(1, "Promoted to Intermediate II", DateTime.UtcNow.AddMonths(-1).ToString("yyyy-MM-dd"), "Level Up"),
            new(2, "First Showcase Performance", DateTime.UtcNow.AddMonths(-2).ToString("yyyy-MM-dd"), "Performance")
        };
        return Task.FromResult<IEnumerable<ProgressMilestoneInfo>>(list);
    }

    public Task<ProgressStreakInfo> GetStreakAsync(CancellationToken cancellationToken = default)
    {
        var streak = new ProgressStreakInfo(7, 14, DateTime.UtcNow.ToString("yyyy-MM-dd"));
        return Task.FromResult(streak);
    }

    public Task<ProgressLevelProgressInfo> GetLevelAsync(CancellationToken cancellationToken = default)
    {
        var level = new ProgressLevelProgressInfo("Intermediate II", "Advanced I", 70.0, 120);
        return Task.FromResult(level);
    }

    public Task<string> GetAboutMeAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_aboutMe);
    }

    public Task<bool> UpdateAboutMeAsync(string aboutMe, CancellationToken cancellationToken = default)
    {
        _aboutMe = aboutMe;
        return Task.FromResult(true);
    }
}

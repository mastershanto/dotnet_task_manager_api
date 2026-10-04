namespace Progress.Domain;

public record ProgressStudentInfo(int Id, string Name, string? Avatar, string? Email);
public record ProgressStreakInfo(int CurrentStreak, int LongestStreak, string? LastAttendedDate);
public record ProgressLevelProgressInfo(string CurrentLevel, string NextLevel, double ProgressPercentage, int PointsToNextLevel);
public record ProgressRewardsInfo(int Points, int BadgesCount);
public record ProgressAchievementInfo(int Id, string Title, string Description, string? Icon, string UnlockedAt);
public record ProgressMilestoneInfo(int Id, string Title, string Date, string Category);

public record StudentProgressModel
{
    public ProgressStudentInfo? Student { get; init; }
    public string MemberSince { get; init; } = "January 2025";
    public string AboutMe { get; init; } = "Passionate Latin dancer focusing on Bachata Sensual and Cuban Salsa styling.";
    public ProgressStreakInfo Streak { get; init; } = new(5, 12, DateTime.UtcNow.ToString("yyyy-MM-dd"));
    public ProgressLevelProgressInfo LevelProgress { get; init; } = new("Intermediate II", "Advanced I", 65.0, 150);
    public ProgressRewardsInfo Rewards { get; init; } = new(1250, 8);
    public List<ProgressAchievementInfo> RecentAchievements { get; init; } = new();
    public List<ProgressMilestoneInfo> RecentMilestones { get; init; } = new();
}

public record UpdateAboutMeRequest(string AboutMe);

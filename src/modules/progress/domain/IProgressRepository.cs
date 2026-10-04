namespace Progress.Domain;

public interface IProgressRepository
{
    Task<StudentProgressModel> GetProgressAsync(int? studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProgressAchievementInfo>> GetRecentAchievementsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProgressMilestoneInfo>> GetRecentMilestonesAsync(CancellationToken cancellationToken = default);
    Task<ProgressStreakInfo> GetStreakAsync(CancellationToken cancellationToken = default);
    Task<ProgressLevelProgressInfo> GetLevelAsync(CancellationToken cancellationToken = default);
    Task<string> GetAboutMeAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateAboutMeAsync(string aboutMe, CancellationToken cancellationToken = default);
}

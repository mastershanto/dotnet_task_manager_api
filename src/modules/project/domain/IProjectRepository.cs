namespace Projects.Domain;

/// <summary>
/// প্রজেক্ট রিপোজিটরি ইন্টারফেস:
/// </summary>
public interface IProjectRepository
{
    Task<IEnumerable<ProjectModel>> ListAsync(CancellationToken cancellationToken = default);
    Task<ProjectModel?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProjectModel> CreateAsync(ProjectModel project, CancellationToken cancellationToken = default);
    Task<ProjectModel?> UpdateAsync(Guid id, ProjectModel project, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

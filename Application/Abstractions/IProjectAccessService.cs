namespace TodoApi.Application.Abstractions;

public interface IProjectAccessService
{
    Task<bool> ProjectExistsAsync(int projectId);
    Task<bool> UserHasProjectAccessAsync(int projectId, int userId);
}

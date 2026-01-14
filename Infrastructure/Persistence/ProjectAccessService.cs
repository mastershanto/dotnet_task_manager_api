using Microsoft.EntityFrameworkCore;
using TodoApi.Application.Abstractions;
using TodoApi.Data;

namespace TodoApi.Infrastructure.Persistence;

public sealed class ProjectAccessService : IProjectAccessService
{
    private readonly TodoContext _context;

    public ProjectAccessService(TodoContext context)
    {
        _context = context;
    }

    public Task<bool> ProjectExistsAsync(int projectId)
    {
        return _context.Projects.AnyAsync(p => p.Id == projectId);
    }

    public async Task<bool> UserHasProjectAccessAsync(int projectId, int userId)
    {
        var isOwnerOrPublic = await _context.Projects
            .Where(p => p.Id == projectId && (p.OwnerId == userId || p.IsPublic))
            .AnyAsync();

        if (isOwnerOrPublic)
            return true;

        return await _context.ProjectMembers
            .Where(pm => pm.ProjectId == projectId && pm.UserId == userId)
            .AnyAsync();
    }
}

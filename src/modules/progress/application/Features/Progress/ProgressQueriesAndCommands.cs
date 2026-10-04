using BuildingBlocks.Abstractions.CQRS;
using BuildingBlocks.Abstractions.Results;
using Progress.Domain;

namespace Progress.Application.Features.Progress;

public record GetStudentProgressQuery(int? StudentId) : IQuery<StudentProgressModel>;
public record GetRecentAchievementsQuery() : IQuery<IEnumerable<ProgressAchievementInfo>>;
public record GetRecentMilestonesQuery() : IQuery<IEnumerable<ProgressMilestoneInfo>>;
public record GetStreakQuery() : IQuery<ProgressStreakInfo>;
public record GetLevelQuery() : IQuery<ProgressLevelProgressInfo>;
public record GetAboutMeQuery() : IQuery<string>;
public record UpdateAboutMeCommand(string AboutMe) : ICommand<bool>;

public class ProgressHandlers :
    IQueryHandler<GetStudentProgressQuery, StudentProgressModel>,
    IQueryHandler<GetRecentAchievementsQuery, IEnumerable<ProgressAchievementInfo>>,
    IQueryHandler<GetRecentMilestonesQuery, IEnumerable<ProgressMilestoneInfo>>,
    IQueryHandler<GetStreakQuery, ProgressStreakInfo>,
    IQueryHandler<GetLevelQuery, ProgressLevelProgressInfo>,
    IQueryHandler<GetAboutMeQuery, string>,
    ICommandHandler<UpdateAboutMeCommand, bool>
{
    private readonly IProgressRepository _repo;

    public ProgressHandlers(IProgressRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<StudentProgressModel>> Handle(GetStudentProgressQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetProgressAsync(request.StudentId, cancellationToken);
        return Result<StudentProgressModel>.Success(data);
    }

    public async Task<Result<IEnumerable<ProgressAchievementInfo>>> Handle(GetRecentAchievementsQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetRecentAchievementsAsync(cancellationToken);
        return Result<IEnumerable<ProgressAchievementInfo>>.Success(data);
    }

    public async Task<Result<IEnumerable<ProgressMilestoneInfo>>> Handle(GetRecentMilestonesQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetRecentMilestonesAsync(cancellationToken);
        return Result<IEnumerable<ProgressMilestoneInfo>>.Success(data);
    }

    public async Task<Result<ProgressStreakInfo>> Handle(GetStreakQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetStreakAsync(cancellationToken);
        return Result<ProgressStreakInfo>.Success(data);
    }

    public async Task<Result<ProgressLevelProgressInfo>> Handle(GetLevelQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetLevelAsync(cancellationToken);
        return Result<ProgressLevelProgressInfo>.Success(data);
    }

    public async Task<Result<string>> Handle(GetAboutMeQuery request, CancellationToken cancellationToken)
    {
        var data = await _repo.GetAboutMeAsync(cancellationToken);
        return Result<string>.Success(data);
    }

    public async Task<Result<bool>> Handle(UpdateAboutMeCommand request, CancellationToken cancellationToken)
    {
        var ok = await _repo.UpdateAboutMeAsync(request.AboutMe, cancellationToken);
        return Result<bool>.Success(ok);
    }
}

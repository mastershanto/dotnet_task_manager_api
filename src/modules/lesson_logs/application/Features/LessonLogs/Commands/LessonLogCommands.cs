using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using LessonLogs.Domain;

namespace LessonLogs.Application.Features.LessonLogs.Commands;

public record CreateLessonLogCommand(CreateLessonLogDto Dto) : ICommand<LessonLogModel>;
public record UpdateLessonLogCommand(int Id, UpdateLessonLogDto Dto) : ICommand<LessonLogModel?>;
public record DeleteLessonLogCommand(int Id) : ICommand<bool>;

public class LessonLogCommandHandlers :
    ICommandHandler<CreateLessonLogCommand, LessonLogModel>,
    ICommandHandler<UpdateLessonLogCommand, LessonLogModel?>,
    ICommandHandler<DeleteLessonLogCommand, bool>
{
    private readonly ILessonLogsRepository _repository;

    public LessonLogCommandHandlers(ILessonLogsRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<LessonLogModel>> Handle(CreateLessonLogCommand request, CancellationToken cancellationToken)
    {
        var log = await _repository.CreateLogAsync(request.Dto, cancellationToken);
        return Result<LessonLogModel>.Success(log);
    }

    public async Task<Result<LessonLogModel?>> Handle(UpdateLessonLogCommand request, CancellationToken cancellationToken)
    {
        var log = await _repository.UpdateLogAsync(request.Id, request.Dto, cancellationToken);
        return log != null ? Result<LessonLogModel?>.Success(log) : Result<LessonLogModel?>.Failure("Lesson log not found");
    }

    public async Task<Result<bool>> Handle(DeleteLessonLogCommand request, CancellationToken cancellationToken)
    {
        var res = await _repository.DeleteLogAsync(request.Id, cancellationToken);
        return Result<bool>.Success(res);
    }
}


using System.ComponentModel;
using System.IO.Pipelines;
using System.Linq.Expressions;
using BuildingBlocks.Abstractions;
using BuildingBlocks.CQRS;
using Microsoft.VisualBasic;
using MyTasks.Domain;

namespace MyTasks.Application.Features.MyTasks
.Commands.CreateTask;

public class CreateMyTaskCommandHandler:ICommandHandler<CreateMyTaskCommandHandler, MyTaskItemModel>
{
    private readonly IMyTaskRepository _repository;

    public CreateMyTaskCommandHandler(IMyTaskRepository repo)
    {
        _repository=repo;
    }

    public async Task<ReadResult<MyTaskItemModel>> Handle(CreateMyTaskCommandHandler request, CancellationToken cancellationToken)
    {
        var task=new MyTaskItemModel{
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description =request.Description?.Trim()??string.Empty,
            Status = request.Status,
            Priority= request.Priority,
            DueDate = request.DueDate,
            CategoryId=request.CategoryId,
            AssignedUserId=request.AssinedUserId,
            CreatedAt=DateTime.UtcNow
                 };
        var created=await _repository.CreateAsync(task,cancellationToken);
        return Result<TaskItemModel>.Success(created);
    }
}
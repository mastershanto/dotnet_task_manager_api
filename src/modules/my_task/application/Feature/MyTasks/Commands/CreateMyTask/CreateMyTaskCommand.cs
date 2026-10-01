
using System.Windows.Input;
using BuildingBlocks.CQRS;
using MyTasks.Domain;

namespace MyTasks.Abstractions.Features.MyTasks.Commands.CreateMyTask;


public record CreateMyTaskCommand(
    string Title,
    string Description,
MyTaskItemStatus Status=MyTaskItemStaus.Todo,
MyTaskPriority Priority=MyTaskPriority.Medium,
DateTime? DueTime=null,
Guid? CategoryId=null,
Guid? AssingedUserId=null
):ICommand<MyTaskItemModel>;

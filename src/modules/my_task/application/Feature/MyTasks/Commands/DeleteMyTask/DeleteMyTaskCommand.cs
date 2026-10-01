
using System.Windows.Input;
using MyTask.Domain;
using BuildingBocks.CQRS;

namespace MyTasks.Features.MyTask.Commands.DeleteMyTask;


public record DeleteMyTaskCommand : ICommand<IMyTaskItemModel>
{
    
}

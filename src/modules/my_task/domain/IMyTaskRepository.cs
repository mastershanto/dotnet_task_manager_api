
namespace MyTasks.Domain;


public interface IMyTaskRepository
{
    Task<MyTaskItemModel> CreateAsync(MyTaskItemModel myTask, CancellationToken cancellationToken=default);
    Task<MyTaskItemModel> GetAsync(Guid id,CancellationToken cancellationToken=default);
    Task<IEnumerable<MyTaskItemModel>> ListAsync(CancellationToken cancellationToken=default);
    Task<MyTaskItemModel?> UpdateAsync(Guid id, MyTaskItemModel myTask,CancellationToken cancellationToken=dafault);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken=default);
}
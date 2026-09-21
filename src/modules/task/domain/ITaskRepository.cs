namespace Tasks.Domain;

/// <summary>
/// টাস্ক রিপোজিটরি ইন্টারফেস (Repository Contract):
/// ডাটাবেস পারসিস্টেন্স অপারেশন নির্ধারণ করে।
/// </summary>
public interface ITaskRepository
{
    Task<IEnumerable<TaskItemModel>> ListAsync(CancellationToken cancellationToken = default);
    Task<TaskItemModel?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskItemModel> CreateAsync(TaskItemModel task, CancellationToken cancellationToken = default);
    Task<TaskItemModel?> UpdateAsync(Guid id, TaskItemModel task, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

namespace LessonLogs.Domain;

public interface ILessonLogsRepository
{
    Task<LessonLogsResponseData> GetLogsAsync(int? userId, string? date, CancellationToken cancellationToken = default);
    Task<LessonLogModel> CreateLogAsync(CreateLessonLogDto dto, CancellationToken cancellationToken = default);
    Task<LessonLogModel?> UpdateLogAsync(int id, UpdateLessonLogDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteLogAsync(int id, CancellationToken cancellationToken = default);
}

namespace LessonLogs.Domain;

public record LessonLogModel
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public int CreatedById { get; init; } = 1;
    public string CreatorName { get; init; } = "Alex Rivera";
    public string Comment { get; init; } = string.Empty;
    public bool IsOwn { get; init; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; init; }
}

public record CreateLessonLogDto(int UserId, string Comment);
public record UpdateLessonLogDto(string Comment);

public record LessonLogsResponseData(
    int? UserId,
    string? Date,
    List<LessonLogItemDto> Logs,
    Dictionary<string, List<LessonLogItemDto>> GroupedLogs
);

public record LessonLogItemDto(
    int Id,
    string Comment,
    string CreatorName,
    bool IsOwn,
    string CreatedAt
);

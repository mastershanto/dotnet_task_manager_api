namespace InstructorClasses.Domain;

public record InstructorClassModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Type { get; init; } = "group"; // group | private
    public string StartTime { get; init; } = "18:00:00";
    public string EndTime { get; init; } = "19:00:00";
    public string Date { get; init; } = DateTime.UtcNow.ToString("yyyy-MM-dd");
    public bool IsCompleted { get; init; }
    public int StudioId { get; init; } = 1;
    public int StudentCount { get; init; } = 6;
    public StudioSummary? Studio { get; init; }
    public ProgramSummary? Program { get; init; }
    public CategorySummary? Category { get; init; }
    public List<ClassUserSummary> Users { get; init; } = new();
    public List<ClassUserSummary> Instructors { get; init; } = new();
}

public record StudioSummary(int Id, string Name, string? Address, string? City);
public record ProgramSummary(int Id, string Title, string? Description, decimal? Price);
public record CategorySummary(int Id, string Name, string? Color);
public record ClassUserSummary(int Id, string Name, string? Email, string? Avatar, bool? IsAttended);

public record RepeatPatternModel(string Key, string Label);

public record CreateClassDto(
    string Title,
    string Type,
    string Date,
    string StartTime,
    string EndTime,
    int StudioId,
    int? ProgramId,
    int? CategoryId,
    string? RepeatPattern,
    List<int>? StudentIds,
    List<int>? InstructorIds
);

public record AddInstructorDto(int LessonId, int InstructorId);
public record AddStudentDto(int LessonId, int StudentId);

public record UpdateAttendanceResult(int LessonId, int StudentId, bool IsCompleted);

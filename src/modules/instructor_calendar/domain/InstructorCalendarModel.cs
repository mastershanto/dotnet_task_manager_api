namespace InstructorCalendar.Domain;

public record CalendarEventModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Type { get; init; } = "group";
    public string Date { get; init; } = DateTime.UtcNow.ToString("yyyy-MM-dd");
    public string StartTime { get; init; } = "18:00:00";
    public string EndTime { get; init; } = "19:00:00";
    public int StudioId { get; init; } = 1;
    public string StudioName { get; init; } = "Main Ballroom";
    public int InstructorId { get; init; } = 1;
    public string InstructorName { get; init; } = "Alex Rivera";
    public string? CategoryColor { get; init; } = "#3B82F6";
    public int EnrolledStudents { get; init; } = 6;
}

public record CalendarInstructorItem(int Id, string Name, string? Avatar);
public record CalendarStudioItem(int Id, string Name, string? Address);

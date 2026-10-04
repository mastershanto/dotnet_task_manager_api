namespace StudentGroupClasses.Domain;

public record GroupStudentClassModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Type { get; init; } = "group";
    public string Status { get; init; } = "scheduled";
    public string StartTime { get; init; } = "18:00:00";
    public string EndTime { get; init; } = "19:00:00";
    public string Date { get; init; } = DateTime.UtcNow.ToString("yyyy-MM-dd");
    public GroupClassProgram? Program { get; init; }
    public GroupClassCategory? Category { get; init; }
    public GroupClassStudio? Studio { get; init; }
    public GroupClassSyllabusItem? SyllabusItem { get; init; }
    public List<GroupClassInstructor> Instructors { get; init; } = new();
    public int StudentsCount { get; init; } = 8;
    public bool IsEnrolled { get; init; }
    public bool IsRecurring { get; init; }
    public string? EnrollUrl { get; init; }
}

public record GroupClassProgram(int Id, string Name, string? Description);
public record GroupClassCategory(int Id, string Name, string? Color);
public record GroupClassStudio(int Id, string Name, string? Address);
public record GroupClassSyllabusItem(int Id, string Name, string? Level);
public record GroupClassInstructor(int Id, string Name, string? Avatar, string? Role);

public record ClassGuideModel(int Id, string Title, string Description, string? Image, string? Content);
public record PurchaseProgramModel(int Id, string Title, decimal Price, string Period, string Description, List<string> Features);
public record MyCalendarEvent(int Id, string Title, string Date, string StartTime, string EndTime, string StudioName, string InstructorName, string Type);

namespace InstructorSyllabus.Domain;

public record SyllabusSummaryModel(int Id, string Title, string? Description, string? Level);

public record StudentSyllabusItemModel
{
    public int Id { get; init; }
    public int StudentId { get; init; }
    public int SyllabusId { get; init; }
    public int SyllabusItemId { get; init; }
    public string ItemName { get; init; } = string.Empty;
    public string CategoryName { get; init; } = "Bronze Steps";
    public bool IsIntroduction { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; init; }
}

public record UpdateStudentSyllabusDto(
    int StudentId,
    int SyllabusId,
    int SyllabusItemId,
    bool IsIntroduction,
    bool IsCompleted
);

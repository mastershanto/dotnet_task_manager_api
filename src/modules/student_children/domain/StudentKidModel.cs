namespace StudentChildren.Domain;

public record StudentKidModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Avatar { get; init; }
    public string? Gender { get; init; }
    public string? Birthday { get; init; }
    public string? DateOfBirth { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? Type { get; init; } = "child";
    public int? ParentId { get; init; }
    public bool IsApproved { get; init; } = true;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public record CreateStudentKidDto(
    string Name,
    string? Email,
    string? Gender,
    string? DateOfBirth,
    string? Phone,
    string? Address
);

public record UpdateStudentKidDto(
    string? Name,
    string? Email,
    string? Gender,
    string? DateOfBirth,
    string? Phone,
    string? Address
);

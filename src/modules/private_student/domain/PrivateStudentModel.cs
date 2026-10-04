namespace PrivateStudent.Domain;

public record PrivateInstructorInfo(int Id, string Name, string? Avatar);
public record PrivateStudioInfo(int Id, string Name, string? Address);

public record PrivateClassModel
{
    public int Id { get; init; }
    public string Title { get; init; } = "Private Lesson";
    public string Date { get; init; } = DateTime.UtcNow.ToString("yyyy-MM-dd");
    public string StartTime { get; init; } = "15:00:00";
    public string EndTime { get; init; } = "16:00:00";
    public string Status { get; init; } = "confirmed";
    public PrivateInstructorInfo? Instructor { get; init; }
    public PrivateStudioInfo? Studio { get; init; }
}

public record AnnouncementModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Date { get; init; } = DateTime.UtcNow.ToString("yyyy-MM-dd");
    public bool IsRead { get; init; } = false;
}

public record PartnerUserInfo(int Id, string Name, string? Avatar, string? Email, string? Level);
public record PartnerRequestInfo(int RequestId, int UserId, string UserName, string? UserAvatar, DateTime RequestedAt);

public record StudentPartnerModel
{
    public int? PartnerId { get; init; }
    public bool HasPartner { get; init; }
    public PartnerUserInfo? Partner { get; init; }
    public List<PartnerRequestInfo> ReceivedRequests { get; init; } = new();
    public List<PartnerRequestInfo> SentRequests { get; init; } = new();
}

public record PrivatePackageModel(
    int Id,
    string Title,
    decimal Price,
    int LessonsCount,
    int UsedLessons,
    int RemainingLessons,
    int ExpiryDays,
    string? ExpiryDate,
    bool IsActive
);

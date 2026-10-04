namespace StudentEvents.Domain;

public record EventStudio(int Id, string Name, string? Address);

public record StudentEventModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Subtitle { get; init; }
    public string StartTime { get; init; } = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd HH:mm:ss");
    public string? BannerImage { get; init; }
    public string? BannerImageUrl { get; init; }
    public EventStudio? Studio { get; init; }
    public int Capacity { get; init; } = 50;
    public decimal Price { get; init; } = 25.00m;
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
    public bool IsPast { get; init; } = false;
    public int BookedSeatsCount { get; init; } = 22;
    public int RemainingSeats { get; init; } = 28;
    public bool IsBookedByMe { get; init; } = false;
    public int MyBookedSeats { get; init; } = 0;
}

public record EventParticipantModel(int Id, string Name, string? Avatar, int SeatsBooked, DateTime BookedAt);
public record DancerOfTheMonthModel(int Id, string Name, string? Avatar, string Month, int Year, string Bio, string Achievement);
public record SearchStudentModel(int Id, string Name, string? Avatar, string Email, string? Level);

public record BookEventRequest(int EventId, int SeatsCount, string? PaymentMethod);

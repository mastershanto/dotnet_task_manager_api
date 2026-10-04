namespace Renter.Domain;

public record RenterCityModel(int Id, string Name, string Slug, string? Image, int StudiosCount);

public record RenterStudioModel(
    int Id,
    string Name,
    string? Address,
    string? City,
    decimal HourlyRate,
    int Capacity,
    string? Description,
    List<string> Amenities,
    List<string> Images
);

public record CheckBookingDateItem(int Id, int BookingId, string BookingDate);
public record CheckBookingResult(bool Success, bool Available, string Message, List<CheckBookingDateItem> Data);

public record BookedStudioItem(
    int Id,
    int StudioId,
    int UserId,
    string BookingDate,
    string StartTime,
    string EndTime,
    decimal TotalPrice,
    string Status,
    string? Notes,
    bool IsRecurring,
    string? RepeatPattern,
    string? EndDate,
    RenterStudioModel? Studio
);

public record BookStudioResponse(
    int Count,
    bool IsRecurring,
    string? RepeatPattern,
    string? EndDate,
    string? Notice,
    BookedStudioItem? PrimaryBooking,
    List<BookedStudioItem> AllBookings
);

public record CheckBookingRequest(
    int StudioId,
    string BookingDate,
    string StartTime,
    string EndTime,
    bool IsRecurring,
    string? RepeatPattern,
    string? EndDate
);

public record BookStudioRequest(
    int StudioId,
    string BookingDate,
    string StartTime,
    string EndTime,
    string? Notes,
    bool IsRecurring,
    string? RepeatPattern,
    string? EndDate
);

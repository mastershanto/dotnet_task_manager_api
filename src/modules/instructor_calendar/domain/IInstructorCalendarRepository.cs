namespace InstructorCalendar.Domain;

public interface IInstructorCalendarRepository
{
    Task<IEnumerable<CalendarEventModel>> GetCalendarEventsAsync(int? instructorId, int? studioId, string? date, CancellationToken cancellationToken = default);
    Task<IEnumerable<CalendarInstructorItem>> GetCalendarInstructorsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<CalendarStudioItem>> GetCalendarStudiosAsync(CancellationToken cancellationToken = default);
}

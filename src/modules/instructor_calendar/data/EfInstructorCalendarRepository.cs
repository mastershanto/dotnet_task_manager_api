using InstructorCalendar.Domain;
using BuildingBlocks.Persistence;

namespace InstructorCalendar.Data;

public class EfInstructorCalendarRepository : IInstructorCalendarRepository
{
    private readonly AppDbContext _db;

    public EfInstructorCalendarRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<IEnumerable<CalendarEventModel>> GetCalendarEventsAsync(int? instructorId, int? studioId, string? date, CancellationToken cancellationToken = default)
    {
        var targetDate = string.IsNullOrWhiteSpace(date) ? DateTime.UtcNow.ToString("yyyy-MM-dd") : date;

        var sampleEvents = new List<CalendarEventModel>
        {
            new() { Id = 101, Title = "Salsa Intermediate", Type = "group", Date = targetDate, StartTime = "10:00:00", EndTime = "11:00:00", StudioId = 1, StudioName = "Main Studio A", InstructorId = 1, InstructorName = "Alex Rivera", CategoryColor = "#E11D48", EnrolledStudents = 8 },
            new() { Id = 102, Title = "Bachata Sensual", Type = "group", Date = targetDate, StartTime = "11:30:00", EndTime = "12:30:00", StudioId = 2, StudioName = "Studio B", InstructorId = 2, InstructorName = "Elena Rostova", CategoryColor = "#9333EA", EnrolledStudents = 12 },
            new() { Id = 103, Title = "Private Technique - John", Type = "private", Date = targetDate, StartTime = "14:00:00", EndTime = "15:00:00", StudioId = 1, StudioName = "Main Studio A", InstructorId = 1, InstructorName = "Alex Rivera", CategoryColor = "#2563EB", EnrolledStudents = 1 },
            new() { Id = 104, Title = "Tango Argentine", Type = "group", Date = targetDate, StartTime = "17:00:00", EndTime = "18:30:00", StudioId = 1, StudioName = "Main Studio A", InstructorId = 1, InstructorName = "Alex Rivera", CategoryColor = "#D97706", EnrolledStudents = 10 }
        };

        IEnumerable<CalendarEventModel> filtered = sampleEvents;
        if (instructorId.HasValue && instructorId.Value > 0)
        {
            filtered = filtered.Where(e => e.InstructorId == instructorId.Value);
        }
        if (studioId.HasValue && studioId.Value > 0)
        {
            filtered = filtered.Where(e => e.StudioId == studioId.Value);
        }

        return Task.FromResult(filtered);
    }

    public Task<IEnumerable<CalendarInstructorItem>> GetCalendarInstructorsAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<CalendarInstructorItem>
        {
            new(1, "Alex Rivera", "https://i.pravatar.cc/150?u=alex"),
            new(2, "Elena Rostova", "https://i.pravatar.cc/150?u=elena"),
            new(3, "Marcus Chen", "https://i.pravatar.cc/150?u=marcus")
        };
        return Task.FromResult<IEnumerable<CalendarInstructorItem>>(list);
    }

    public Task<IEnumerable<CalendarStudioItem>> GetCalendarStudiosAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<CalendarStudioItem>
        {
            new(1, "Main Studio A", "Floor 1, West Wing"),
            new(2, "Studio B", "Floor 2, East Wing"),
            new(3, "Private Rehearsal Room", "Floor 1, Suite 102")
        };
        return Task.FromResult<IEnumerable<CalendarStudioItem>>(list);
    }
}

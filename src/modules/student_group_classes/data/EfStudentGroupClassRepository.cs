using BuildingBlocks.Persistence;
using StudentGroupClasses.Domain;

namespace StudentGroupClasses.Data;

public class EfStudentGroupClassRepository : IStudentGroupClassRepository
{
    private readonly AppDbContext _db;

    public EfStudentGroupClassRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<IEnumerable<GroupStudentClassModel>> GetGroupClassesAsync(string? date, CancellationToken cancellationToken = default)
    {
        var targetDate = string.IsNullOrWhiteSpace(date) ? DateTime.UtcNow.ToString("yyyy-MM-dd") : date;

        var list = new List<GroupStudentClassModel>
        {
            new()
            {
                Id = 1,
                Title = "Salsa Cubana Beginners",
                Type = "group",
                Status = "open",
                StartTime = "18:00:00",
                EndTime = "19:00:00",
                Date = targetDate,
                Program = new(1, "Latin Basics", "Foundations of Latin rhythms"),
                Category = new(1, "Salsa", "#EF4444"),
                Studio = new(1, "Main Dance Hall", "100 Sunset Blvd"),
                SyllabusItem = new(1, "Basic Step & Cross Body Lead", "Beginner"),
                Instructors = new() { new(1, "Carlos Mendez", "https://i.pravatar.cc/150?u=carlos", "Lead Instructor") },
                StudentsCount = 14,
                IsEnrolled = false,
                IsRecurring = false,
                EnrollUrl = "/auth/student/group-classes/enroll"
            },
            new()
            {
                Id = 2,
                Title = "Bachata Sensual Intermediate",
                Type = "group",
                Status = "open",
                StartTime = "19:30:00",
                EndTime = "20:30:00",
                Date = targetDate,
                Program = new(2, "Sensual Progression", "Intermediate body movement"),
                Category = new(2, "Bachata", "#8B5CF6"),
                Studio = new(2, "Studio B", "100 Sunset Blvd, 2nd Fl"),
                SyllabusItem = new(2, "Body Wave & Head Roll Mechanics", "Intermediate"),
                Instructors = new() { new(2, "Elena Rostova", "https://i.pravatar.cc/150?u=elena", "Senior Instructor") },
                StudentsCount = 18,
                IsEnrolled = true,
                IsRecurring = true,
                EnrollUrl = "/auth/student/group-classes/enroll"
            }
        };

        return Task.FromResult<IEnumerable<GroupStudentClassModel>>(list);
    }

    public Task<IEnumerable<GroupStudentClassModel>> GetMyClassesAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<GroupStudentClassModel>
        {
            new()
            {
                Id = 2,
                Title = "Bachata Sensual Intermediate",
                Type = "group",
                Status = "confirmed",
                StartTime = "19:30:00",
                EndTime = "20:30:00",
                Date = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"),
                Program = new(2, "Sensual Progression", "Intermediate body movement"),
                Category = new(2, "Bachata", "#8B5CF6"),
                Studio = new(2, "Studio B", "100 Sunset Blvd, 2nd Fl"),
                SyllabusItem = new(2, "Body Wave & Head Roll Mechanics", "Intermediate"),
                Instructors = new() { new(2, "Elena Rostova", "https://i.pravatar.cc/150?u=elena", "Senior Instructor") },
                StudentsCount = 18,
                IsEnrolled = true,
                IsRecurring = true,
                EnrollUrl = null
            }
        };

        return Task.FromResult<IEnumerable<GroupStudentClassModel>>(list);
    }

    public Task<IEnumerable<GroupStudentClassModel>> GetClassHistoryAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<GroupStudentClassModel>
        {
            new()
            {
                Id = 99,
                Title = "Tango Essentials",
                Type = "group",
                Status = "completed",
                StartTime = "17:00:00",
                EndTime = "18:00:00",
                Date = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd"),
                Program = new(3, "Argentine Tango", "Classic Argentine styling"),
                Category = new(3, "Tango", "#F59E0B"),
                Studio = new(1, "Main Dance Hall", "100 Sunset Blvd"),
                SyllabusItem = new(3, "Ocho Cortado", "Intermediate"),
                Instructors = new() { new(1, "Carlos Mendez", "https://i.pravatar.cc/150?u=carlos", "Lead Instructor") },
                StudentsCount = 12,
                IsEnrolled = true,
                IsRecurring = false,
                EnrollUrl = null
            }
        };

        return Task.FromResult<IEnumerable<GroupStudentClassModel>>(list);
    }

    public Task<GroupStudentClassModel?> GetClassDetailsAsync(int classId, CancellationToken cancellationToken = default)
    {
        var model = new GroupStudentClassModel
        {
            Id = classId,
            Title = "Salsa Cubana Beginners",
            Type = "group",
            Status = "open",
            StartTime = "18:00:00",
            EndTime = "19:00:00",
            Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            Program = new(1, "Latin Basics", "Foundations of Latin rhythms"),
            Category = new(1, "Salsa", "#EF4444"),
            Studio = new(1, "Main Dance Hall", "100 Sunset Blvd"),
            SyllabusItem = new(1, "Basic Step & Cross Body Lead", "Beginner"),
            Instructors = new() { new(1, "Carlos Mendez", "https://i.pravatar.cc/150?u=carlos", "Lead Instructor") },
            StudentsCount = 14,
            IsEnrolled = false,
            IsRecurring = false,
            EnrollUrl = "/auth/student/group-classes/enroll"
        };

        return Task.FromResult<GroupStudentClassModel?>(model);
    }

    public Task<bool> EnrollClassAsync(int classId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<bool> CancelClassAsync(int classId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<bool> CancelRecurringClassAsync(int classId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<IEnumerable<ClassGuideModel>> GetClassGuidesAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<ClassGuideModel>
        {
            new(1, "Attire & Dance Shoes Guide", "What to wear for social vs ballroom dancing.", "https://images.unsplash.com/photo-1547153760-18fc86324498?w=400", "Wear comfortable shoes with suede or smooth leather soles. Avoid rubber-soled athletic sneakers which can strain your knees on wooden floors."),
            new(2, "Etiquette On The Social Floor", "Best practices for asking dancers and floor safety.", "https://images.unsplash.com/photo-1518834107812-67b0b7c58434?w=400", "Always respect physical boundaries, apologize quickly for inadvertent collisions, and smile!")
        };
        return Task.FromResult<IEnumerable<ClassGuideModel>>(list);
    }

    public Task<ClassGuideModel?> GetClassGuideDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var guide = new ClassGuideModel(
            id,
            "Attire & Dance Shoes Guide",
            "What to wear for social vs ballroom dancing.",
            "https://images.unsplash.com/photo-1547153760-18fc86324498?w=400",
            "Wear comfortable shoes with suede or smooth leather soles. Avoid rubber-soled athletic sneakers which can strain your knees on wooden floors."
        );
        return Task.FromResult<ClassGuideModel?>(guide);
    }

    public Task<IEnumerable<PurchaseProgramModel>> GetPurchaseProgramsAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<PurchaseProgramModel>
        {
            new(1, "Group Class 10-Pack", 150.00m, "Per 10 Classes", "Access any group dance session on the calendar", new() { "Valid for 6 months", "All instructors included", "Flexible booking" }),
            new(2, "Monthly Unlimited Dance Pass", 220.00m, "Per Month", "Unlimited attendance to all group classes", new() { "Unlimited sessions", "Free social dance entrance", "10% store discount" })
        };
        return Task.FromResult<IEnumerable<PurchaseProgramModel>>(list);
    }

    public Task<bool> PurchaseProgramAsync(int id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<IEnumerable<MyCalendarEvent>> GetMyCalendarEventsAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<MyCalendarEvent>
        {
            new(1, "Salsa Cubana Beginners", DateTime.UtcNow.ToString("yyyy-MM-dd"), "18:00:00", "19:00:00", "Main Dance Hall", "Carlos Mendez", "group"),
            new(2, "Bachata Sensual Intermediate", DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd"), "19:30:00", "20:30:00", "Studio B", "Elena Rostova", "group")
        };
        return Task.FromResult<IEnumerable<MyCalendarEvent>>(list);
    }

    public Task<object> SyncGoogleCalendarAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<object>(new
        {
            synced = true,
            synced_at = DateTime.UtcNow,
            events_count = 2,
            calendar_url = "https://calendar.google.com/calendar/r"
        });
    }
}

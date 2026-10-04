using BuildingBlocks.Persistence;
using StudentEvents.Domain;

namespace StudentEvents.Data;

public class EfStudentEventsRepository : IStudentEventsRepository
{
    private readonly AppDbContext _db;

    public EfStudentEventsRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<IEnumerable<StudentEventModel>> GetEventsAsync(string? type, CancellationToken cancellationToken = default)
    {
        var list = new List<StudentEventModel>
        {
            new()
            {
                Id = 1,
                Title = "Sunset Summer Salsa Gala",
                Subtitle = "An unforgettable night of Latin music, performances, and open dance floor.",
                StartTime = DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-dd HH:mm:ss"),
                BannerImage = "gala_banner.jpg",
                BannerImageUrl = "https://images.unsplash.com/photo-1508700115892-45ecd05ae2ad?w=800",
                Studio = new(1, "Main Ballroom", "100 Sunset Blvd"),
                Capacity = 100,
                Price = 35.00m,
                Description = "Join us for our signature annual Summer Gala! Featuring live salsa orchestra, student showcases, and guest performances.",
                IsActive = true,
                IsPast = false,
                BookedSeatsCount = 48,
                RemainingSeats = 52,
                IsBookedByMe = true,
                MyBookedSeats = 2
            },
            new()
            {
                Id = 2,
                Title = "Bachata Sensual Intensive Workshop",
                Subtitle = "Master connection, body movement, and head rolls with guest artists.",
                StartTime = DateTime.UtcNow.AddDays(12).ToString("yyyy-MM-dd HH:mm:ss"),
                BannerImage = "bachata_workshop.jpg",
                BannerImageUrl = "https://images.unsplash.com/photo-1545959570-a9438e88867a?w=800",
                Studio = new(2, "Studio B", "100 Sunset Blvd, 2nd Fl"),
                Capacity = 40,
                Price = 50.00m,
                Description = "A 3-hour deep dive workshop covering leading, following, musicality, and advanced social patterns.",
                IsActive = true,
                IsPast = false,
                BookedSeatsCount = 28,
                RemainingSeats = 12,
                IsBookedByMe = false,
                MyBookedSeats = 0
            }
        };

        return Task.FromResult<IEnumerable<StudentEventModel>>(list);
    }

    public Task<StudentEventModel?> GetEventDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = new StudentEventModel
        {
            Id = id,
            Title = "Sunset Summer Salsa Gala",
            Subtitle = "An unforgettable night of Latin music, performances, and open dance floor.",
            StartTime = DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-dd HH:mm:ss"),
            BannerImage = "gala_banner.jpg",
            BannerImageUrl = "https://images.unsplash.com/photo-1508700115892-45ecd05ae2ad?w=800",
            Studio = new(1, "Main Ballroom", "100 Sunset Blvd"),
            Capacity = 100,
            Price = 35.00m,
            Description = "Join us for our signature annual Summer Gala! Featuring live salsa orchestra, student showcases, and guest performances.",
            IsActive = true,
            IsPast = false,
            BookedSeatsCount = 48,
            RemainingSeats = 52,
            IsBookedByMe = true,
            MyBookedSeats = 2
        };
        return Task.FromResult<StudentEventModel?>(item);
    }

    public Task<IEnumerable<EventParticipantModel>> GetParticipantsAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var list = new List<EventParticipantModel>
        {
            new(1, "Sophia Laurent", "https://i.pravatar.cc/150?u=sophia", 2, DateTime.UtcNow.AddDays(-2)),
            new(2, "David Kim", "https://i.pravatar.cc/150?u=david", 1, DateTime.UtcNow.AddDays(-1)),
            new(3, "Amara Williams", "https://i.pravatar.cc/150?u=amara", 1, DateTime.UtcNow.AddHours(-12))
        };
        return Task.FromResult<IEnumerable<EventParticipantModel>>(list);
    }

    public Task<bool> BookEventAsync(BookEventRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<DancerOfTheMonthModel?> GetDancerOfTheMonthAsync(CancellationToken cancellationToken = default)
    {
        var model = new DancerOfTheMonthModel(
            1,
            "Amara Williams",
            "https://i.pravatar.cc/150?u=amara",
            "September",
            2026,
            "Amara demonstrated outstanding dedication to mastering Argentine Tango and helped inspire new beginner dancers at social nights!",
            "Completed 24 classes with a 100% attendance rate"
        );
        return Task.FromResult<DancerOfTheMonthModel?>(model);
    }

    public Task<IEnumerable<SearchStudentModel>> SearchStudentsAsync(string? query, CancellationToken cancellationToken = default)
    {
        var list = new List<SearchStudentModel>
        {
            new(1, "Carlos Mendez", "https://i.pravatar.cc/150?u=carlos", "carlos@sunsetdance.com", "Instructor"),
            new(2, "Elena Rostova", "https://i.pravatar.cc/150?u=elena", "elena@sunsetdance.com", "Instructor"),
            new(3, "Amara Williams", "https://i.pravatar.cc/150?u=amara", "amara@example.com", "Advanced Student")
        };

        if (!string.IsNullOrWhiteSpace(query))
        {
            list = list.Where(s => s.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || s.Email.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return Task.FromResult<IEnumerable<SearchStudentModel>>(list);
    }
}

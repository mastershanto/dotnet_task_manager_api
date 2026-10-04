using BuildingBlocks.Persistence;
using PrivateStudent.Domain;

namespace PrivateStudent.Data;

public class EfPrivateStudentRepository : IPrivateStudentRepository
{
    private readonly AppDbContext _db;

    public EfPrivateStudentRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<IEnumerable<PrivateClassModel>> GetClassesAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<PrivateClassModel>
        {
            new()
            {
                Id = 1,
                Title = "Private Tango Choreography",
                Date = DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd"),
                StartTime = "14:00:00",
                EndTime = "15:00:00",
                Status = "confirmed",
                Instructor = new(1, "Carlos Mendez", "https://i.pravatar.cc/150?u=carlos"),
                Studio = new(1, "Private Room 1", "100 Sunset Blvd, Suite A")
            },
            new()
            {
                Id = 2,
                Title = "Advanced Body Movement Technique",
                Date = DateTime.UtcNow.AddDays(5).ToString("yyyy-MM-dd"),
                StartTime = "16:00:00",
                EndTime = "17:00:00",
                Status = "confirmed",
                Instructor = new(2, "Elena Rostova", "https://i.pravatar.cc/150?u=elena"),
                Studio = new(2, "Studio B", "100 Sunset Blvd, 2nd Fl")
            }
        };

        return Task.FromResult<IEnumerable<PrivateClassModel>>(list);
    }

    public Task<PrivateClassModel?> GetNextPrivateClassAsync(CancellationToken cancellationToken = default)
    {
        var next = new PrivateClassModel
        {
            Id = 1,
            Title = "Private Tango Choreography",
            Date = DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd"),
            StartTime = "14:00:00",
            EndTime = "15:00:00",
            Status = "confirmed",
            Instructor = new(1, "Carlos Mendez", "https://i.pravatar.cc/150?u=carlos"),
            Studio = new(1, "Private Room 1", "100 Sunset Blvd, Suite A")
        };
        return Task.FromResult<PrivateClassModel?>(next);
    }

    public Task<IEnumerable<AnnouncementModel>> GetAnnouncementsAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<AnnouncementModel>
        {
            new()
            {
                Id = 1,
                Title = "Studio Floor Refurbishment Notice",
                Content = "Main Ballroom floor refinishing scheduled for this weekend. All lessons will be relocated to Studio B.",
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                IsRead = false
            },
            new()
            {
                Id = 2,
                Title = "Guest Artist Workshop Announced",
                Content = "World Tango Champions visiting next month! Early bird registrations now open.",
                Date = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-dd"),
                IsRead = true
            }
        };
        return Task.FromResult<IEnumerable<AnnouncementModel>>(list);
    }

    public Task<bool> MarkAnnouncementReadAsync(int id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<IEnumerable<PrivatePackageModel>> GetPurchaseProgramsAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<PrivatePackageModel>
        {
            new(1, "Starter Private 5-Pack", 450.00m, 5, 0, 5, 60, null, true),
            new(2, "Performance Intensive 10-Pack", 850.00m, 10, 0, 10, 90, null, true),
            new(3, "Master Mastery 20-Pack", 1600.00m, 20, 0, 20, 180, null, true)
        };
        return Task.FromResult<IEnumerable<PrivatePackageModel>>(list);
    }

    public Task<bool> PurchaseProgramAsync(int packageId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<IEnumerable<PrivatePackageModel>> GetMyPackagesAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<PrivatePackageModel>
        {
            new(1, "Starter Private 5-Pack", 450.00m, 5, 3, 2, 60, DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd"), true)
        };
        return Task.FromResult<IEnumerable<PrivatePackageModel>>(list);
    }

    public Task<PrivatePackageModel?> GetMyActivePackageAsync(CancellationToken cancellationToken = default)
    {
        var active = new PrivatePackageModel(1, "Starter Private 5-Pack", 450.00m, 5, 3, 2, 60, DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd"), true);
        return Task.FromResult<PrivatePackageModel?>(active);
    }

    public Task<StudentPartnerModel> GetPartnerInfoAsync(CancellationToken cancellationToken = default)
    {
        var info = new StudentPartnerModel
        {
            PartnerId = 4,
            HasPartner = true,
            Partner = new(4, "Amara Williams", "https://i.pravatar.cc/150?u=amara", "amara@example.com", "Advanced"),
            ReceivedRequests = new(),
            SentRequests = new()
        };
        return Task.FromResult(info);
    }

    public Task<bool> RequestPartnerAsync(int userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<bool> CancelPartnerRequestAsync(int userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<bool> AcceptPartnerAsync(int userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<bool> DeclinePartnerAsync(int userId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<bool> RemovePartnerAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }
}

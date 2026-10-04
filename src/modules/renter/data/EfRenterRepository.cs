using BuildingBlocks.Persistence;
using Renter.Domain;

namespace Renter.Data;

public class EfRenterRepository : IRenterRepository
{
    private readonly AppDbContext _db;
    private static readonly List<BookedStudioItem> _inMemoryBookings = new()
    {
        new BookedStudioItem(
            11,
            1,
            1,
            DateTime.UtcNow.AddDays(4).ToString("yyyy-MM-dd"),
            "10:00:00",
            "12:00:00",
            120.00m,
            "confirmed",
            "Dance rehearsal with partner",
            false,
            null,
            null,
            new RenterStudioModel(1, "Main Ballroom", "100 Sunset Blvd", "los_angeles", 60.00m, 50, "Spacious hardwood floor ballroom with sound system", new() { "Hardwood Floor", "Sound System", "Mirrors", "Air Conditioning" }, new() { "https://images.unsplash.com/photo-1547153760-18fc86324498?w=800" })
        )
    };

    public EfRenterRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<IEnumerable<RenterCityModel>> GetCitiesAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<RenterCityModel>
        {
            new(1, "Los Angeles", "los_angeles", "https://images.unsplash.com/photo-1580655653885-65763b2597d0?w=600", 4),
            new(2, "San Francisco", "san_francisco", "https://images.unsplash.com/photo-1501594907352-04cda38ebc29?w=600", 2),
            new(3, "San Diego", "san_diego", "https://images.unsplash.com/photo-1506744038136-46273834b3fb?w=600", 3)
        };
        return Task.FromResult<IEnumerable<RenterCityModel>>(list);
    }

    public Task<IEnumerable<string>> GetRepeatPatternsAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<string> { "daily", "weekly", "biweekly", "monthly" };
        return Task.FromResult<IEnumerable<string>>(list);
    }

    public Task<IEnumerable<RenterStudioModel>> GetStudiosAsync(string? city, CancellationToken cancellationToken = default)
    {
        var list = new List<RenterStudioModel>
        {
            new(1, "Main Ballroom", "100 Sunset Blvd", "los_angeles", 60.00m, 50, "Spacious hardwood floor ballroom with premium sound system", new() { "Hardwood Floor", "Sound System", "Full Length Mirrors", "Air Conditioning" }, new() { "https://images.unsplash.com/photo-1547153760-18fc86324498?w=800" }),
            new(2, "Studio B (Choreography Suite)", "100 Sunset Blvd, 2nd Fl", "los_angeles", 45.00m, 25, "Modern private studio with sprung wooden floors and mood lighting", new() { "Sprung Wood Floor", "Bluetooth Audio", "Mirrors", "Lounge Area" }, new() { "https://images.unsplash.com/photo-1518834107812-67b0b7c58434?w=800" }),
            new(3, "Private Rehearsal Studio", "100 Sunset Blvd, Suite 102", "los_angeles", 35.00m, 10, "Cozy studio ideal for private lessons and small group practice", new() { "Mirrors", "Audio Dock", "Ballet Barres" }, new() { "https://images.unsplash.com/photo-1508700115892-45ecd05ae2ad?w=800" })
        };

        if (!string.IsNullOrWhiteSpace(city))
        {
            list = list.Where(s => s.City != null && s.City.Equals(city, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return Task.FromResult<IEnumerable<RenterStudioModel>>(list);
    }

    public Task<CheckBookingResult> CheckBookingAsync(CheckBookingRequest request, CancellationToken cancellationToken = default)
    {
        var res = new CheckBookingResult(
            Success: true,
            Available: true,
            Message: "Selected time slot is available for booking.",
            Data: new List<CheckBookingDateItem>
            {
                new(1, 101, request.BookingDate)
            }
        );
        return Task.FromResult(res);
    }

    public Task<BookStudioResponse> BookStudioAsync(BookStudioRequest request, CancellationToken cancellationToken = default)
    {
        var newId = _inMemoryBookings.Count > 0 ? _inMemoryBookings.Max(b => b.Id) + 1 : 1;
        var booked = new BookedStudioItem(
            newId,
            request.StudioId,
            1,
            request.BookingDate,
            request.StartTime,
            request.EndTime,
            90.00m,
            "confirmed",
            request.Notes,
            request.IsRecurring,
            request.RepeatPattern,
            request.EndDate,
            new RenterStudioModel(request.StudioId, "Main Ballroom", "100 Sunset Blvd", "los_angeles", 60.00m, 50, "Hardwood floor ballroom", new() { "Sound System", "Mirrors" }, new())
        );

        _inMemoryBookings.Add(booked);

        var resp = new BookStudioResponse(
            Count: 1,
            IsRecurring: request.IsRecurring,
            RepeatPattern: request.RepeatPattern,
            EndDate: request.EndDate,
            Notice: "Studio booked successfully! Please arrive 10 minutes prior to your reservation.",
            PrimaryBooking: booked,
            AllBookings: new() { booked }
        );

        return Task.FromResult(resp);
    }

    public Task<IEnumerable<BookedStudioItem>> GetMyBookingsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<BookedStudioItem>>(_inMemoryBookings);
    }

    public Task<bool> CancelBookingAsync(int id, CancellationToken cancellationToken = default)
    {
        var found = _inMemoryBookings.FirstOrDefault(b => b.Id == id);
        if (found != null)
        {
            _inMemoryBookings.Remove(found);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> CancelRecurringBookingAsync(CancellationToken cancellationToken = default)
    {
        _inMemoryBookings.RemoveAll(b => b.IsRecurring);
        return Task.FromResult(true);
    }
}

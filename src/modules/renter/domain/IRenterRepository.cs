namespace Renter.Domain;

public interface IRenterRepository
{
    Task<IEnumerable<RenterCityModel>> GetCitiesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetRepeatPatternsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<RenterStudioModel>> GetStudiosAsync(string? city, CancellationToken cancellationToken = default);
    Task<CheckBookingResult> CheckBookingAsync(CheckBookingRequest request, CancellationToken cancellationToken = default);
    Task<BookStudioResponse> BookStudioAsync(BookStudioRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookedStudioItem>> GetMyBookingsAsync(CancellationToken cancellationToken = default);
    Task<bool> CancelBookingAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> CancelRecurringBookingAsync(CancellationToken cancellationToken = default);
}

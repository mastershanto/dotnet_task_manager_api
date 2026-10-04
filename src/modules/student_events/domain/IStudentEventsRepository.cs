namespace StudentEvents.Domain;

public interface IStudentEventsRepository
{
    Task<IEnumerable<StudentEventModel>> GetEventsAsync(string? type, CancellationToken cancellationToken = default);
    Task<StudentEventModel?> GetEventDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventParticipantModel>> GetParticipantsAsync(int eventId, CancellationToken cancellationToken = default);
    Task<bool> BookEventAsync(BookEventRequest request, CancellationToken cancellationToken = default);
    Task<DancerOfTheMonthModel?> GetDancerOfTheMonthAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SearchStudentModel>> SearchStudentsAsync(string? query, CancellationToken cancellationToken = default);
}

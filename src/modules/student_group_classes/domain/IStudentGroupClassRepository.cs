namespace StudentGroupClasses.Domain;

public interface IStudentGroupClassRepository
{
    Task<IEnumerable<GroupStudentClassModel>> GetGroupClassesAsync(string? date, CancellationToken cancellationToken = default);
    Task<IEnumerable<GroupStudentClassModel>> GetMyClassesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<GroupStudentClassModel>> GetClassHistoryAsync(CancellationToken cancellationToken = default);
    Task<GroupStudentClassModel?> GetClassDetailsAsync(int classId, CancellationToken cancellationToken = default);
    Task<bool> EnrollClassAsync(int classId, CancellationToken cancellationToken = default);
    Task<bool> CancelClassAsync(int classId, CancellationToken cancellationToken = default);
    Task<bool> CancelRecurringClassAsync(int classId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClassGuideModel>> GetClassGuidesAsync(CancellationToken cancellationToken = default);
    Task<ClassGuideModel?> GetClassGuideDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PurchaseProgramModel>> GetPurchaseProgramsAsync(CancellationToken cancellationToken = default);
    Task<bool> PurchaseProgramAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MyCalendarEvent>> GetMyCalendarEventsAsync(CancellationToken cancellationToken = default);
    Task<object> SyncGoogleCalendarAsync(CancellationToken cancellationToken = default);
}

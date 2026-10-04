namespace PrivateStudent.Domain;

public interface IPrivateStudentRepository
{
    Task<IEnumerable<PrivateClassModel>> GetClassesAsync(CancellationToken cancellationToken = default);
    Task<PrivateClassModel?> GetNextPrivateClassAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AnnouncementModel>> GetAnnouncementsAsync(CancellationToken cancellationToken = default);
    Task<bool> MarkAnnouncementReadAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<PrivatePackageModel>> GetPurchaseProgramsAsync(CancellationToken cancellationToken = default);
    Task<bool> PurchaseProgramAsync(int packageId, CancellationToken cancellationToken = default);
    Task<IEnumerable<PrivatePackageModel>> GetMyPackagesAsync(CancellationToken cancellationToken = default);
    Task<PrivatePackageModel?> GetMyActivePackageAsync(CancellationToken cancellationToken = default);
    Task<StudentPartnerModel> GetPartnerInfoAsync(CancellationToken cancellationToken = default);
    Task<bool> RequestPartnerAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> CancelPartnerRequestAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> AcceptPartnerAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> DeclinePartnerAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> RemovePartnerAsync(CancellationToken cancellationToken = default);
}

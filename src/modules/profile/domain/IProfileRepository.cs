namespace Profile.Domain;

public interface IProfileRepository
{
    Task<ProfileModel?> GetProfileAsync(int? id = null, CancellationToken cancellationToken = default);
    Task<ProfileModel> UpdateProfileAsync(ProfileUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> ToggleVisibilityAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdatePasswordAsync(UpdatePasswordDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAccountAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<UserCardModel>> GetCardsAsync(int? userId = null, CancellationToken cancellationToken = default);
    Task<UserCardModel> StoreCardAsync(StoreCardDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteCardAsync(int id, CancellationToken cancellationToken = default);
    Task<object> GetCreditsAsync(CancellationToken cancellationToken = default);
    Task<object> GetLowCreditCardAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ChildSummaryModel>> GetChildrenAsync(CancellationToken cancellationToken = default);
    Task<ProfileModel?> SwitchChildAsync(int childId, CancellationToken cancellationToken = default);
    Task<ProfileModel?> SwitchToParentAsync(CancellationToken cancellationToken = default);
}

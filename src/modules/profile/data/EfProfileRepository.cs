using System.Collections.Concurrent;
using Profile.Domain;

namespace Profile.Data;

public class EfProfileRepository : IProfileRepository
{
    private static ProfileModel _currentProfile = new()
    {
        Id = 1,
        Name = "Mahfuzul Islam",
        Email = "student@sunsetdance.com",
        Phone = "12345678",
        Address = "Sunset Boulevard, CA",
        Gender = "male",
        Birthday = "2000-03-05",
        Facebook = "facebook.com/mahfuzul",
        Instagram = "@mahfuzul",
        Role = "student",
        Type = "student",
        UserLavel = "Bronze 3",
        Level = "Bronze 3",
        ClassesRemaining = 28,
        TotalClasses = 40,
        StreakWeeks = 4,
        IsProfileVisibility = true,
        IsFullProgram = true,
        Children = new List<ChildSummaryModel>
        {
            new(6, "Leo Islam", "https://sunsetdance.thewarriors.team/assets/images/child1.png", "Bronze 1", 12),
            new(7, "Maya Islam", "https://sunsetdance.thewarriors.team/assets/images/child2.png", "Silver 2", 18)
        },
        Partner = new(4, "Amara Chen", "https://sunsetdance.thewarriors.team/assets/images/partner.png", "Bronze 3", "accepted")
    };

    private static readonly ConcurrentDictionary<int, UserCardModel> _cards = new();
    private static int _nextCardId = 1;

    static EfProfileRepository()
    {
        _cards.TryAdd(1, new UserCardModel
        {
            Id = 1,
            UserId = 1,
            CardHolderName = "Mahfuzul Islam",
            CardNumber = "4242••••••••4242",
            ExpMonth = "12",
            ExpYear = "2028",
            Cvc = "123",
            IsDefault = true,
            CreatedAt = DateTime.UtcNow
        });
    }

    public Task<ProfileModel?> GetProfileAsync(int? id = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProfileModel?>(_currentProfile);
    }

    public Task<ProfileModel> UpdateProfileAsync(ProfileUpdateDto dto, CancellationToken cancellationToken = default)
    {
        _currentProfile = _currentProfile with
        {
            Name = dto.Name ?? _currentProfile.Name,
            Phone = dto.Phone ?? _currentProfile.Phone,
            Address = dto.Address ?? _currentProfile.Address,
            Gender = dto.Gender ?? _currentProfile.Gender,
            Birthday = dto.Birthday ?? _currentProfile.Birthday,
            Facebook = dto.Facebook ?? _currentProfile.Facebook,
            Instagram = dto.Instagram ?? _currentProfile.Instagram,
            Avatar = dto.Avatar ?? _currentProfile.Avatar
        };
        return Task.FromResult(_currentProfile);
    }

    public Task<bool> ToggleVisibilityAsync(CancellationToken cancellationToken = default)
    {
        _currentProfile = _currentProfile with
        {
            IsProfileVisibility = !_currentProfile.IsProfileVisibility
        };
        return Task.FromResult(true);
    }

    public Task<bool> UpdatePasswordAsync(UpdatePasswordDto dto, CancellationToken cancellationToken = default)
    {
        // Simple password validation & update
        return Task.FromResult(dto.Password == dto.PasswordConfirmation);
    }

    public Task<bool> DeleteAccountAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<IEnumerable<UserCardModel>> GetCardsAsync(int? userId = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<UserCardModel>>(_cards.Values.ToList());
    }

    public Task<UserCardModel> StoreCardAsync(StoreCardDto dto, CancellationToken cancellationToken = default)
    {
        var id = Interlocked.Increment(ref _nextCardId);
        var card = new UserCardModel
        {
            Id = id,
            UserId = _currentProfile.Id,
            CardHolderName = dto.CardHolderName,
            CardNumber = dto.CardNumber.Length >= 4 ? $"••••••••••••{dto.CardNumber[^4..]}" : dto.CardNumber,
            ExpMonth = dto.ExpMonth,
            ExpYear = dto.ExpYear,
            Cvc = dto.Cvc,
            IsDefault = dto.IsDefault == 1,
            CreatedAt = DateTime.UtcNow
        };
        _cards[id] = card;
        return Task.FromResult(card);
    }

    public Task<bool> DeleteCardAsync(int id, CancellationToken cancellationToken = default)
    {
        _cards.TryRemove(id, out _);
        return Task.FromResult(true);
    }

    public Task<object> GetCreditsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<object>(new
        {
            total_credits = 125,
            available_credits = 45,
            used_credits = 80,
            package_name = "Unlimited Private Gold Package",
            expiry_date = "2026-12-31"
        });
    }

    public Task<object> GetLowCreditCardAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<object>(new
        {
            has_low_credits = false,
            remaining_credits = 45,
            threshold = 10,
            recommended_package_id = 3,
            recommended_package_name = "Bronze 20-Pack"
        });
    }

    public Task<IEnumerable<ChildSummaryModel>> GetChildrenAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<ChildSummaryModel>>(_currentProfile.Children);
    }

    public Task<ProfileModel?> SwitchChildAsync(int childId, CancellationToken cancellationToken = default)
    {
        var child = _currentProfile.Children.FirstOrDefault(c => c.Id == childId);
        if (child == null) return Task.FromResult<ProfileModel?>(null);

        var childProfile = _currentProfile with
        {
            Id = child.Id,
            Name = child.Name,
            Avatar = child.Avatar,
            Level = child.Level ?? "Bronze 1",
            ClassesRemaining = child.ClassesRemaining,
            ParentId = _currentProfile.Id
        };
        return Task.FromResult<ProfileModel?>(childProfile);
    }

    public Task<ProfileModel?> SwitchToParentAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProfileModel?>(_currentProfile);
    }
}

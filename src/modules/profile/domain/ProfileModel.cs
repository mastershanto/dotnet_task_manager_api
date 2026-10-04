namespace Profile.Domain;

public record ProfileModel
{
    public int Id { get; init; } = 1;
    public string Name { get; init; } = "Sunset Dancer";
    public string Email { get; init; } = "student@sunsetdance.com";
    public string? Avatar { get; init; } = "https://sunsetdance.thewarriors.team/assets/images/default-avatar.png";
    public string? Address { get; init; } = "123 Sunset Blvd, Los Angeles, CA";
    public string? Phone { get; init; } = "+1 234 567 8900";
    public string? Gender { get; init; } = "male";
    public string? Birthday { get; init; } = "1998-05-15";
    public string? Facebook { get; init; } = "facebook.com/sunsetdancer";
    public string? Instagram { get; init; } = "@sunsetdancer";
    public string Role { get; init; } = "student";
    public string Type { get; init; } = "student";
    public int? ParentId { get; init; }
    public int? PartnerId { get; init; }
    public bool IsApproved { get; init; } = true;
    public string UserLavel { get; init; } = "Bronze 3";
    public string Level { get; init; } = "Bronze 3";
    public int ClassesRemaining { get; init; } = 28;
    public int TotalClasses { get; init; } = 40;
    public int StreakWeeks { get; init; } = 4;
    public bool IsProfileVisibility { get; init; } = true;
    public bool IsFullProgram { get; init; } = true;
    public List<ChildSummaryModel> Children { get; init; } = new();
    public PartnerSummaryModel? Partner { get; init; }
}

public record ChildSummaryModel(int Id, string Name, string? Avatar, string? Level, int ClassesRemaining);
public record PartnerSummaryModel(int Id, string Name, string? Avatar, string? Level, string? Status);

public record UserCardModel
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string CardHolderName { get; init; } = string.Empty;
    public string CardNumber { get; init; } = string.Empty;
    public string ExpMonth { get; init; } = string.Empty;
    public string ExpYear { get; init; } = string.Empty;
    public string Cvc { get; init; } = string.Empty;
    public bool IsDefault { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; init; }
}

public record ProfileUpdateDto(
    string? Name,
    string? Phone,
    string? Address,
    string? Gender,
    string? Birthday,
    string? Facebook,
    string? Instagram,
    string? Avatar
);

public record UpdatePasswordDto(
    string CurrentPassword,
    string Password,
    string PasswordConfirmation
);

public record StoreCardDto(
    string CardHolderName,
    string CardNumber,
    string ExpMonth,
    string ExpYear,
    string Cvc,
    int IsDefault = 0
);

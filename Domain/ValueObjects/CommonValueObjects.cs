using TodoApi.Domain.Common;

namespace TodoApi.Domain.ValueObjects;

/// <summary>
/// Value Object for Email - Enterprise DDD pattern with validation
/// </summary>
public record Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<Email>.Failure("Email cannot be empty");

        if (!System.Text.RegularExpressions.Regex.IsMatch(email, 
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return Result<Email>.Failure("Invalid email format");

        if (email.Length > 100)
            return Result<Email>.Failure("Email cannot exceed 100 characters");

        return Result<Email>.Success(new Email(email));
    }

    public static implicit operator string(Email email) => email.Value;
}

/// <summary>
/// Value Object for Task Progress - ensures business rules
/// </summary>
public record TaskProgress
{
    public double Value { get; }

    private TaskProgress(double value)
    {
        Value = value;
    }

    public static Result<TaskProgress> Create(double progress)
    {
        if (progress < 0 || progress > 100)
            return Result<TaskProgress>.Failure("Progress must be between 0 and 100");

        return Result<TaskProgress>.Success(new TaskProgress(progress));
    }

    public static implicit operator double(TaskProgress progress) => progress.Value;
}

/// <summary>
/// Value Object for Hours - ensures positive values
/// </summary>
public record Hours
{
    public int Value { get; }

    private Hours(int value)
    {
        Value = value;
    }

    public static Result<Hours> Create(int hours)
    {
        if (hours < 0)
            return Result<Hours>.Failure("Hours cannot be negative");

        if (hours > 1000)
            return Result<Hours>.Failure("Hours cannot exceed 1000");

        return Result<Hours>.Success(new Hours(hours));
    }

    public static implicit operator int(Hours hours) => hours.Value;
}

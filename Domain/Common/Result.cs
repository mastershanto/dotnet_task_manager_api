namespace TodoApi.Domain.Common;

/// <summary>
/// Result pattern for explicit success/failure without throwing exceptions.
/// Placed in Domain to avoid Domain depending on Application.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    public List<string> Errors { get; }

    protected Result(bool isSuccess, string error, List<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error ?? string.Empty;
        Errors = errors ?? new List<string>();
    }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(List<string> errors) => new(false, string.Empty, errors);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, string error, List<string>? errors = null)
        : base(isSuccess, error, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, string.Empty);
    public new static Result<T> Failure(string error) => new(false, default, error);
    public new static Result<T> Failure(List<string> errors) => new(false, default, string.Empty, errors);
}

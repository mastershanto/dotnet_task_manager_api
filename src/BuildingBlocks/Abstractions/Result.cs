namespace BuildingBlocks.Abstractions;

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string[] Errors { get; }
    public T? Value { get; }

    private Result(T? value, bool success, string[] errors)
    {
        IsSuccess = success;
        Errors = errors;
        Value = value;
    }

    public static Result<T> Success(T value) => new(value, true, Array.Empty<string>());
    public static Result<T> Failure(params string[] errors) => new(default, false, errors);
}

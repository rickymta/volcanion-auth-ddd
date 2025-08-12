namespace Volcanion.Auth.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? ErrorMessage { get; private set; }
    public IEnumerable<string> Errors { get; private set; } = new List<string>();

    protected Result(bool isSuccess, T? data, string? errorMessage, IEnumerable<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        Errors = errors ?? new List<string>();
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, data, null);
    }

    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T>(false, default, errorMessage);
    }

    public static Result<T> Failure(IEnumerable<string> errors)
    {
        return new Result<T>(false, default, "Multiple errors occurred", errors);
    }
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public IEnumerable<string> Errors { get; private set; } = new List<string>();

    protected Result(bool isSuccess, string? errorMessage, IEnumerable<string>? errors = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Errors = errors ?? new List<string>();
    }

    public static Result Success()
    {
        return new Result(true, null);
    }

    public static Result Failure(string errorMessage)
    {
        return new Result(false, errorMessage);
    }

    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, "Multiple errors occurred", errors);
    }
}

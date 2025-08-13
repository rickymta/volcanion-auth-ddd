namespace Volcanion.Auth.Domain.Common;

public class SimpleResult<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; private set; }
    public string Error { get; private set; }

    private SimpleResult(bool isSuccess, T value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static SimpleResult<T> Success(T value)
    {
        return new SimpleResult<T>(true, value, string.Empty);
    }

    public static SimpleResult<T> Failure(string error)
    {
        return new SimpleResult<T>(false, default(T)!, error);
    }
}

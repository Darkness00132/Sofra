namespace Sofra.Application.Common.Results;

public record Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }

    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);

    public static Result Failure(Error error) => new(false, error);
}
public record Result<TResponse>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public TResponse? Response { get; }
    public Error? Error { get; }

    private Result(TResponse response)
    {
        IsSuccess = true;
        Response = response;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static Result<TResponse> Success(TResponse response) =>
        new(response);

    public static Result<TResponse> Failure(Error error) =>
        new(error);

    public static implicit operator Result<TResponse>(TResponse response) =>
        Success(response);
}
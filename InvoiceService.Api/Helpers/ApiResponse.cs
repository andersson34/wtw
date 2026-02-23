namespace InvoiceService.Api.Helpers;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string Message { get; init; } = string.Empty;
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    public static ApiResponse<T> Ok(T data, string message = "") => new()
    {
        Success = true,
        Data = data,
        Message = message,
        Errors = Array.Empty<string>()
    };

    public static ApiResponse<T> Fail(string message, IReadOnlyList<string>? errors = null) => new()
    {
        Success = false,
        Data = default,
        Message = message,
        Errors = errors ?? Array.Empty<string>()
    };
}

public static class ApiResponse
{
    public static ApiResponse<object?> Ok(object? data, string message = "") => new()
    {
        Success = true,
        Data = data,
        Message = message,
        Errors = Array.Empty<string>()
    };

    public static ApiResponse<object?> Fail(string message, IReadOnlyList<string>? errors = null) => new()
    {
        Success = false,
        Data = null,
        Message = message,
        Errors = errors ?? Array.Empty<string>()
    };
}

namespace InvoiceService.Api.Helpers;

public sealed class ApiHttpException : Exception
{
    public int StatusCode { get; }
    public IReadOnlyList<string> Errors { get; }

    public ApiHttpException(int statusCode, string message, IReadOnlyList<string>? errors = null) : base(message)
    {
        StatusCode = statusCode;
        Errors = errors ?? Array.Empty<string>();
    }
}

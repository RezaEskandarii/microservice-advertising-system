namespace AdvertisingSystem.Api.Middlewares;

public readonly record struct RequestLog
{
    public string TraceId { get; init; }
    public string Method { get; init; }
    public string Path { get; init; }
    public string QueryString { get; init; }
    public string UserAgent { get; init; }
    public string IpAddress { get; init; }
    public int? ResponseStatusCode { get; init; }
    public long ResponseTimeMs { get; init; }
    public string UserId { get; init; }
}
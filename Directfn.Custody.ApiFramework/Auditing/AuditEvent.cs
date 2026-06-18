namespace Directfn.Custody.ApiFramework.Auditing;

public sealed class AuditEvent
{
    public string AuditId { get; init; } = Guid.NewGuid().ToString("N");

    public DateTimeOffset OccurredAtUtc { get; init; } = DateTimeOffset.UtcNow;

    public string? UserId { get; init; }

    public string? UserName { get; init; }

    public string? SessionId { get; init; }

    public string? CorrelationId { get; init; }

    public string? HttpMethod { get; init; }

    public string? Path { get; init; }

    public string? ControllerName { get; init; }

    public string? ActionName { get; init; }

    public string? AuditAction { get; init; }

    public int? StatusCode { get; init; }

    public bool Succeeded { get; init; }

    public long DurationMs { get; init; }

    public string? IpAddress { get; init; }

    public string? UserAgent { get; init; }

    public string? RequestSummary { get; init; }

    public string? ResponseSummary { get; init; }

    public string? ErrorMessage { get; init; }
}
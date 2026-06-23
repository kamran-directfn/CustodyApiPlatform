using Microsoft.Extensions.Logging;

namespace Directfn.Custody.ApiFramework.Auditing;

public sealed class NullAuditWriter : IAuditWriter
{
    private readonly ILogger<NullAuditWriter> _logger;

    public NullAuditWriter(ILogger<NullAuditWriter> logger)
    {
        _logger = logger;
    }

    public Task WriteAsync(AuditEvent auditEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("AUDIT {AuditAction} UserId={UserId} Controller={Controller} Action={Action} Status={StatusCode} DurationMs={DurationMs} CorrelationId={CorrelationId}", auditEvent.AuditAction, auditEvent.UserId, auditEvent.ControllerName, auditEvent.ActionName, auditEvent.StatusCode, auditEvent.DurationMs, auditEvent.CorrelationId);

        return Task.CompletedTask;
    }
}
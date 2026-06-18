namespace Directfn.Custody.ApiFramework.Auditing;

public interface IAuditWriter
{
    Task WriteAsync(AuditEvent auditEvent, CancellationToken cancellationToken);
}
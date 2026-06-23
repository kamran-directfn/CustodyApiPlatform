using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace Directfn.Custody.ApiFramework.Auditing.SQLite;

public sealed class SQLiteAuditWriter : IAuditWriter
{
    private readonly AuditOptions _options;

    public SQLiteAuditWriter(IOptions<AuditOptions> options)
    {
        _options = options.Value;
    }

    public async Task WriteAsync(AuditEvent auditEvent, CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            return;
        }

        await using var connection = new SqliteConnection(GetConnectionString());

        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.CommandText = """
        INSERT INTO audit_logs (
            audit_id,
            occurred_at_utc,
            user_id,
            user_name,
            session_id,
            correlation_id,
            http_method,
            path,
            controller_name,
            action_name,
            audit_action,
            status_code,
            succeeded,
            duration_ms,
            ip_address,
            user_agent,
            request_summary,
            response_summary,
            error_message
        )
        VALUES (
            $audit_id,
            $occurred_at_utc,
            $user_id,
            $user_name,
            $session_id,
            $correlation_id,
            $http_method,
            $path,
            $controller_name,
            $action_name,
            $audit_action,
            $status_code,
            $succeeded,
            $duration_ms,
            $ip_address,
            $user_agent,
            $request_summary,
            $response_summary,
            $error_message
        );
        """;

        command.Parameters.AddWithValue("$audit_id", auditEvent.AuditId);
        command.Parameters.AddWithValue("$occurred_at_utc", auditEvent.OccurredAtUtc.ToString("O"));
        command.Parameters.AddWithValue("$user_id", ToDbValue(auditEvent.UserId));
        command.Parameters.AddWithValue("$user_name", ToDbValue(auditEvent.UserName));
        command.Parameters.AddWithValue("$session_id", ToDbValue(auditEvent.SessionId));
        command.Parameters.AddWithValue("$correlation_id", ToDbValue(auditEvent.CorrelationId));
        command.Parameters.AddWithValue("$http_method", ToDbValue(auditEvent.HttpMethod));
        command.Parameters.AddWithValue("$path", ToDbValue(auditEvent.Path));
        command.Parameters.AddWithValue("$controller_name", ToDbValue(auditEvent.ControllerName));
        command.Parameters.AddWithValue("$action_name", ToDbValue(auditEvent.ActionName));
        command.Parameters.AddWithValue("$audit_action", ToDbValue(auditEvent.AuditAction));
        command.Parameters.AddWithValue("$status_code", ToDbValue(auditEvent.StatusCode));
        command.Parameters.AddWithValue("$succeeded", auditEvent.Succeeded ? 1 : 0);
        command.Parameters.AddWithValue("$duration_ms", auditEvent.DurationMs);
        command.Parameters.AddWithValue("$ip_address", ToDbValue(auditEvent.IpAddress));
        command.Parameters.AddWithValue("$user_agent", ToDbValue(auditEvent.UserAgent));
        command.Parameters.AddWithValue("$request_summary", ToDbValue(auditEvent.RequestSummary));
        command.Parameters.AddWithValue("$response_summary", ToDbValue(auditEvent.ResponseSummary));
        command.Parameters.AddWithValue("$error_message", ToDbValue(auditEvent.ErrorMessage));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private string GetConnectionString()
    {
        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            throw new InvalidOperationException("Audit SQLite connection string is missing.");
        }

        return _options.ConnectionString;
    }

    private static object ToDbValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? DBNull.Value : value;
    }

    private static object ToDbValue(int? value)
    {
        return value is null ? DBNull.Value : value.Value;
    }
}
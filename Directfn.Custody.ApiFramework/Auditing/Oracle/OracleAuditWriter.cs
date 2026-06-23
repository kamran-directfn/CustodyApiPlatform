using System.Data;
using Directfn.Custody.ApiFramework.Database;
using Oracle.ManagedDataAccess.Client;

namespace Directfn.Custody.ApiFramework.Auditing.Oracle;

public sealed class OracleAuditWriter : IAuditWriter
{
    private readonly IDbConnectionFactory _connectionFactory;

    public OracleAuditWriter(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task WriteAsync(AuditEvent auditEvent, CancellationToken cancellationToken)
    {
        await using var connection = (OracleConnection)_connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.BindByName = true;
        command.CommandType = CommandType.Text;
        command.CommandText = """
        INSERT INTO AUDIT_LOGS (
            AUDIT_ID,
            OCCURRED_AT_UTC,
            USER_ID,
            USER_NAME,
            SESSION_ID,
            CORRELATION_ID,
            HTTP_METHOD,
            PATH,
            CONTROLLER_NAME,
            ACTION_NAME,
            AUDIT_ACTION,
            STATUS_CODE,
            SUCCEEDED,
            DURATION_MS,
            IP_ADDRESS,
            USER_AGENT,
            REQUEST_SUMMARY,
            RESPONSE_SUMMARY,
            ERROR_MESSAGE
        )
        VALUES (
            :AUDIT_ID,
            :OCCURRED_AT_UTC,
            :USER_ID,
            :USER_NAME,
            :SESSION_ID,
            :CORRELATION_ID,
            :HTTP_METHOD,
            :PATH,
            :CONTROLLER_NAME,
            :ACTION_NAME,
            :AUDIT_ACTION,
            :STATUS_CODE,
            :SUCCEEDED,
            :DURATION_MS,
            :IP_ADDRESS,
            :USER_AGENT,
            :REQUEST_SUMMARY,
            :RESPONSE_SUMMARY,
            :ERROR_MESSAGE
        )
        """;

        command.Parameters.Add("AUDIT_ID", OracleDbType.Varchar2).Value = auditEvent.AuditId;
        command.Parameters.Add("OCCURRED_AT_UTC", OracleDbType.TimeStamp).Value = auditEvent.OccurredAtUtc.UtcDateTime;
        command.Parameters.Add("USER_ID", OracleDbType.Varchar2).Value = ToDbValue(auditEvent.UserId);
        command.Parameters.Add("USER_NAME", OracleDbType.Varchar2).Value = ToDbValue(auditEvent.UserName);
        command.Parameters.Add("SESSION_ID", OracleDbType.Varchar2).Value = ToDbValue(auditEvent.SessionId);
        command.Parameters.Add("CORRELATION_ID", OracleDbType.Varchar2).Value = ToDbValue(auditEvent.CorrelationId);
        command.Parameters.Add("HTTP_METHOD", OracleDbType.Varchar2).Value = ToDbValue(auditEvent.HttpMethod);
        command.Parameters.Add("PATH", OracleDbType.Varchar2).Value = ToDbValue(auditEvent.Path);
        command.Parameters.Add("CONTROLLER_NAME", OracleDbType.Varchar2).Value = ToDbValue(auditEvent.ControllerName);
        command.Parameters.Add("ACTION_NAME", OracleDbType.Varchar2).Value = ToDbValue(auditEvent.ActionName);
        command.Parameters.Add("AUDIT_ACTION", OracleDbType.Varchar2).Value = ToDbValue(auditEvent.AuditAction);
        command.Parameters.Add("STATUS_CODE", OracleDbType.Int32).Value = ToDbValue(auditEvent.StatusCode);
        command.Parameters.Add("SUCCEEDED", OracleDbType.Int32).Value = auditEvent.Succeeded ? 1 : 0;
        command.Parameters.Add("DURATION_MS", OracleDbType.Int64).Value = auditEvent.DurationMs;
        command.Parameters.Add("IP_ADDRESS", OracleDbType.Varchar2).Value = ToDbValue(auditEvent.IpAddress);
        command.Parameters.Add("USER_AGENT", OracleDbType.Varchar2).Value = ToDbValue(auditEvent.UserAgent);
        command.Parameters.Add("REQUEST_SUMMARY", OracleDbType.Clob).Value = ToDbValue(auditEvent.RequestSummary);
        command.Parameters.Add("RESPONSE_SUMMARY", OracleDbType.Clob).Value = ToDbValue(auditEvent.ResponseSummary);
        command.Parameters.Add("ERROR_MESSAGE", OracleDbType.Clob).Value = ToDbValue(auditEvent.ErrorMessage);

        await command.ExecuteNonQueryAsync(cancellationToken);
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
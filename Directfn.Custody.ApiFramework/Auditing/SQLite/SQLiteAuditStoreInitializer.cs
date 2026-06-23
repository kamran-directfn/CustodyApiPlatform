using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace Directfn.Custody.ApiFramework.Auditing.SQLite;

public sealed class SQLiteAuditStoreInitializer
{
    private readonly AuditOptions _options;

    public SQLiteAuditStoreInitializer(IOptions<AuditOptions> options)
    {
        _options = options.Value;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || !_options.InitializeDatabase || _options.Provider != AuditStoreProvider.SQLite)
        {
            return;
        }

        string connectionString = GetConnectionString();

        EnsureDatabaseDirectory(connectionString);

        await using var connection = new SqliteConnection(connectionString);

        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.CommandText = """
        CREATE TABLE IF NOT EXISTS audit_logs (
            audit_id TEXT NOT NULL PRIMARY KEY,
            occurred_at_utc TEXT NOT NULL,
            user_id TEXT NULL,
            user_name TEXT NULL,
            session_id TEXT NULL,
            correlation_id TEXT NULL,
            http_method TEXT NULL,
            path TEXT NULL,
            controller_name TEXT NULL,
            action_name TEXT NULL,
            audit_action TEXT NULL,
            status_code INTEGER NULL,
            succeeded INTEGER NOT NULL,
            duration_ms INTEGER NOT NULL,
            ip_address TEXT NULL,
            user_agent TEXT NULL,
            request_summary TEXT NULL,
            response_summary TEXT NULL,
            error_message TEXT NULL
        );

        CREATE INDEX IF NOT EXISTS ix_audit_logs_occurred_at ON audit_logs (occurred_at_utc);
        CREATE INDEX IF NOT EXISTS ix_audit_logs_user_id ON audit_logs (user_id);
        CREATE INDEX IF NOT EXISTS ix_audit_logs_session_id ON audit_logs (session_id);
        CREATE INDEX IF NOT EXISTS ix_audit_logs_correlation_id ON audit_logs (correlation_id);
        CREATE INDEX IF NOT EXISTS ix_audit_logs_audit_action ON audit_logs (audit_action);
        """;

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

    private static void EnsureDatabaseDirectory(string connectionString)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString);

        if (string.IsNullOrWhiteSpace(builder.DataSource))
        {
            return;
        }

        string? directory = Path.GetDirectoryName(builder.DataSource);

        if (string.IsNullOrWhiteSpace(directory))
        {
            return;
        }

        Directory.CreateDirectory(directory);
    }
}
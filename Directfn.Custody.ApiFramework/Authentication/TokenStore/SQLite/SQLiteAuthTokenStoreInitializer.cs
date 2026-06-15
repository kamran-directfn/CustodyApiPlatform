using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace Directfn.Custody.ApiFramework.Authentication.TokenStore.SQLite;

public sealed class SQLiteAuthTokenStoreInitializer
{
    private readonly AuthTokenStoreOptions _options;

    public SQLiteAuthTokenStoreInitializer(IOptions<AuthTokenStoreOptions> options)
    {
        _options = options.Value;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.InitializeDatabase)
        {
            return;
        }

        string connectionString = GetConnectionString();

        EnsureDatabaseDirectory(connectionString);

        await using var connection = new SqliteConnection(connectionString);

        await connection.OpenAsync(cancellationToken);

        string sql = """
        CREATE TABLE IF NOT EXISTS auth_sessions (
            session_id TEXT NOT NULL PRIMARY KEY,
            user_id TEXT NOT NULL,
            user_name TEXT NULL,
            created_at_utc TEXT NOT NULL,
            expires_at_utc TEXT NOT NULL,
            revoked_at_utc TEXT NULL,
            ip_address TEXT NULL,
            user_agent TEXT NULL
        );

        CREATE TABLE IF NOT EXISTS auth_refresh_tokens (
            refresh_token_id TEXT NOT NULL PRIMARY KEY,
            session_id TEXT NOT NULL,
            user_id TEXT NOT NULL,
            token_hash TEXT NOT NULL UNIQUE,
            created_at_utc TEXT NOT NULL,
            expires_at_utc TEXT NOT NULL,
            used_at_utc TEXT NULL,
            revoked_at_utc TEXT NULL,
            replaced_by_token_hash TEXT NULL,
            created_by_ip TEXT NULL,
            user_agent TEXT NULL,
            FOREIGN KEY (session_id) REFERENCES auth_sessions(session_id)
        );

        CREATE INDEX IF NOT EXISTS ix_auth_refresh_tokens_token_hash ON auth_refresh_tokens(token_hash);
        CREATE INDEX IF NOT EXISTS ix_auth_refresh_tokens_session_id ON auth_refresh_tokens(session_id);
        CREATE INDEX IF NOT EXISTS ix_auth_sessions_user_id ON auth_sessions(user_id);
        """;

        await using var command = connection.CreateCommand();

        command.CommandText = sql;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private string GetConnectionString()
    {
        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            throw new InvalidOperationException("AuthTokenStore connection string is missing.");
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
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace Directfn.Custody.ApiFramework.Authentication.TokenStore.SQLite;

public sealed class SQLiteAuthTokenStore : IAuthTokenStore
{
    private readonly AuthTokenStoreOptions _options;

    public SQLiteAuthTokenStore(IOptions<AuthTokenStoreOptions> options)
    {
        _options = options.Value;
    }

    public async Task CreateSessionAsync(AuthSessionRecord session, CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(GetConnectionString());

        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.CommandText = """
        INSERT INTO auth_sessions (
            session_id,
            user_id,
            user_name,
            created_at_utc,
            expires_at_utc,
            revoked_at_utc,
            ip_address,
            user_agent
        )
        VALUES (
            $session_id,
            $user_id,
            $user_name,
            $created_at_utc,
            $expires_at_utc,
            $revoked_at_utc,
            $ip_address,
            $user_agent
        );
        """;

        AddSessionParameters(command, session);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task StoreRefreshTokenAsync(RefreshTokenRecord refreshToken, CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(GetConnectionString());

        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.CommandText = """
        INSERT INTO auth_refresh_tokens (
            refresh_token_id,
            session_id,
            user_id,
            token_hash,
            created_at_utc,
            expires_at_utc,
            used_at_utc,
            revoked_at_utc,
            replaced_by_token_hash,
            created_by_ip,
            user_agent
        )
        VALUES (
            $refresh_token_id,
            $session_id,
            $user_id,
            $token_hash,
            $created_at_utc,
            $expires_at_utc,
            $used_at_utc,
            $revoked_at_utc,
            $replaced_by_token_hash,
            $created_by_ip,
            $user_agent
        );
        """;

        AddRefreshTokenParameters(command, refreshToken);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<RefreshTokenValidationResult> ValidateRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(GetConnectionString());

        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.CommandText = """
        SELECT
            rt.refresh_token_id,
            rt.session_id,
            rt.user_id,
            rt.token_hash,
            rt.created_at_utc,
            rt.expires_at_utc,
            rt.used_at_utc,
            rt.revoked_at_utc,
            rt.replaced_by_token_hash,
            rt.created_by_ip,
            rt.user_agent,
            s.user_name,
            s.created_at_utc AS session_created_at_utc,
            s.expires_at_utc AS session_expires_at_utc,
            s.revoked_at_utc AS session_revoked_at_utc,
            s.ip_address AS session_ip_address,
            s.user_agent AS session_user_agent
        FROM auth_refresh_tokens rt
        INNER JOIN auth_sessions s ON s.session_id = rt.session_id
        WHERE rt.token_hash = $token_hash
        LIMIT 1;
        """;

        command.Parameters.AddWithValue("$token_hash", refreshTokenHash);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return RefreshTokenValidationResult.Fail(RefreshTokenValidationStatus.NotFound);
        }

        var session = new AuthSessionRecord
        {
            SessionId = reader.GetString(reader.GetOrdinal("session_id")),
            UserId = reader.GetString(reader.GetOrdinal("user_id")),
            UserName = GetNullableString(reader, "user_name"),
            CreatedAtUtc = GetDateTimeOffset(reader, "session_created_at_utc"),
            ExpiresAtUtc = GetDateTimeOffset(reader, "session_expires_at_utc"),
            RevokedAtUtc = GetNullableDateTimeOffset(reader, "session_revoked_at_utc"),
            IpAddress = GetNullableString(reader, "session_ip_address"),
            UserAgent = GetNullableString(reader, "session_user_agent")
        };

        var refreshToken = new RefreshTokenRecord
        {
            RefreshTokenId = reader.GetString(reader.GetOrdinal("refresh_token_id")),
            SessionId = reader.GetString(reader.GetOrdinal("session_id")),
            UserId = reader.GetString(reader.GetOrdinal("user_id")),
            TokenHash = reader.GetString(reader.GetOrdinal("token_hash")),
            CreatedAtUtc = GetDateTimeOffset(reader, "created_at_utc"),
            ExpiresAtUtc = GetDateTimeOffset(reader, "expires_at_utc"),
            UsedAtUtc = GetNullableDateTimeOffset(reader, "used_at_utc"),
            RevokedAtUtc = GetNullableDateTimeOffset(reader, "revoked_at_utc"),
            ReplacedByTokenHash = GetNullableString(reader, "replaced_by_token_hash"),
            CreatedByIp = GetNullableString(reader, "created_by_ip"),
            UserAgent = GetNullableString(reader, "user_agent")
        };

        if (!session.IsActive)
        {
            return RefreshTokenValidationResult.Fail(RefreshTokenValidationStatus.SessionInvalid);
        }

        if (refreshToken.ExpiresAtUtc <= DateTimeOffset.UtcNow)
        {
            return RefreshTokenValidationResult.Fail(RefreshTokenValidationStatus.Expired);
        }

        if (refreshToken.RevokedAtUtc is not null)
        {
            return RefreshTokenValidationResult.Fail(RefreshTokenValidationStatus.Revoked);
        }

        if (refreshToken.UsedAtUtc is not null)
        {
            return RefreshTokenValidationResult.Fail(RefreshTokenValidationStatus.AlreadyUsed);
        }

        return RefreshTokenValidationResult.Valid(session, refreshToken);
    }

    public async Task RotateRefreshTokenAsync(string currentRefreshTokenHash, RefreshTokenRecord newRefreshToken, CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(GetConnectionString());

        await connection.OpenAsync(cancellationToken);

        await using var transaction = (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken);

        await using var updateCommand = connection.CreateCommand();

        updateCommand.Transaction = transaction;
        updateCommand.CommandText = """
        UPDATE auth_refresh_tokens
        SET
            used_at_utc = $used_at_utc,
            replaced_by_token_hash = $replaced_by_token_hash
        WHERE token_hash = $current_token_hash
          AND used_at_utc IS NULL
          AND revoked_at_utc IS NULL;
        """;

        updateCommand.Parameters.AddWithValue("$used_at_utc", DateTimeOffset.UtcNow.ToString("O"));
        updateCommand.Parameters.AddWithValue("$replaced_by_token_hash", newRefreshToken.TokenHash);
        updateCommand.Parameters.AddWithValue("$current_token_hash", currentRefreshTokenHash);

        int affectedRows = await updateCommand.ExecuteNonQueryAsync(cancellationToken);

        if (affectedRows != 1)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new InvalidOperationException("Refresh token rotation failed. Current token may already be used or revoked.");
        }

        await using var insertCommand = connection.CreateCommand();

        insertCommand.Transaction = transaction;
        insertCommand.CommandText = """
        INSERT INTO auth_refresh_tokens (
            refresh_token_id,
            session_id,
            user_id,
            token_hash,
            created_at_utc,
            expires_at_utc,
            used_at_utc,
            revoked_at_utc,
            replaced_by_token_hash,
            created_by_ip,
            user_agent
        )
        VALUES (
            $refresh_token_id,
            $session_id,
            $user_id,
            $token_hash,
            $created_at_utc,
            $expires_at_utc,
            $used_at_utc,
            $revoked_at_utc,
            $replaced_by_token_hash,
            $created_by_ip,
            $user_agent
        );
        """;

        AddRefreshTokenParameters(insertCommand, newRefreshToken);

        await insertCommand.ExecuteNonQueryAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task RevokeSessionAsync(string sessionId, CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(GetConnectionString());

        await connection.OpenAsync(cancellationToken);

        await using var transaction = (SqliteTransaction)await connection.BeginTransactionAsync(cancellationToken);

        string revokedAtUtc = DateTimeOffset.UtcNow.ToString("O");

        await using var sessionCommand = connection.CreateCommand();

        sessionCommand.Transaction = transaction;
        sessionCommand.CommandText = """
        UPDATE auth_sessions
        SET revoked_at_utc = $revoked_at_utc
        WHERE session_id = $session_id
          AND revoked_at_utc IS NULL;
        """;

        sessionCommand.Parameters.AddWithValue("$revoked_at_utc", revokedAtUtc);
        sessionCommand.Parameters.AddWithValue("$session_id", sessionId);

        await sessionCommand.ExecuteNonQueryAsync(cancellationToken);

        await using var tokenCommand = connection.CreateCommand();

        tokenCommand.Transaction = transaction;
        tokenCommand.CommandText = """
        UPDATE auth_refresh_tokens
        SET revoked_at_utc = $revoked_at_utc
        WHERE session_id = $session_id
          AND revoked_at_utc IS NULL;
        """;

        tokenCommand.Parameters.AddWithValue("$revoked_at_utc", revokedAtUtc);
        tokenCommand.Parameters.AddWithValue("$session_id", sessionId);

        await tokenCommand.ExecuteNonQueryAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<bool> IsSessionActiveAsync(string userId, string sessionId, CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(GetConnectionString());

        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.CommandText = """
        SELECT COUNT(1)
        FROM auth_sessions
        WHERE session_id = $session_id
          AND user_id = $user_id
          AND revoked_at_utc IS NULL
          AND expires_at_utc > $now_utc;
        """;

        command.Parameters.AddWithValue("$session_id", sessionId);
        command.Parameters.AddWithValue("$user_id", userId);
        command.Parameters.AddWithValue("$now_utc", DateTimeOffset.UtcNow.ToString("O"));

        object? result = await command.ExecuteScalarAsync(cancellationToken);

        return Convert.ToInt32(result) > 0;
    }

    private string GetConnectionString()
    {
        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            throw new InvalidOperationException("AuthTokenStore connection string is missing.");
        }

        return _options.ConnectionString;
    }

    private static void AddSessionParameters(SqliteCommand command, AuthSessionRecord session)
    {
        command.Parameters.AddWithValue("$session_id", session.SessionId);
        command.Parameters.AddWithValue("$user_id", session.UserId);
        command.Parameters.AddWithValue("$user_name", ToDbValue(session.UserName));
        command.Parameters.AddWithValue("$created_at_utc", session.CreatedAtUtc.ToString("O"));
        command.Parameters.AddWithValue("$expires_at_utc", session.ExpiresAtUtc.ToString("O"));
        command.Parameters.AddWithValue("$revoked_at_utc", ToDbValue(session.RevokedAtUtc));
        command.Parameters.AddWithValue("$ip_address", ToDbValue(session.IpAddress));
        command.Parameters.AddWithValue("$user_agent", ToDbValue(session.UserAgent));
    }

    private static void AddRefreshTokenParameters(SqliteCommand command, RefreshTokenRecord refreshToken)
    {
        command.Parameters.AddWithValue("$refresh_token_id", refreshToken.RefreshTokenId);
        command.Parameters.AddWithValue("$session_id", refreshToken.SessionId);
        command.Parameters.AddWithValue("$user_id", refreshToken.UserId);
        command.Parameters.AddWithValue("$token_hash", refreshToken.TokenHash);
        command.Parameters.AddWithValue("$created_at_utc", refreshToken.CreatedAtUtc.ToString("O"));
        command.Parameters.AddWithValue("$expires_at_utc", refreshToken.ExpiresAtUtc.ToString("O"));
        command.Parameters.AddWithValue("$used_at_utc", ToDbValue(refreshToken.UsedAtUtc));
        command.Parameters.AddWithValue("$revoked_at_utc", ToDbValue(refreshToken.RevokedAtUtc));
        command.Parameters.AddWithValue("$replaced_by_token_hash", ToDbValue(refreshToken.ReplacedByTokenHash));
        command.Parameters.AddWithValue("$created_by_ip", ToDbValue(refreshToken.CreatedByIp));
        command.Parameters.AddWithValue("$user_agent", ToDbValue(refreshToken.UserAgent));
    }

    private static object ToDbValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? DBNull.Value : value;
    }

    private static object ToDbValue(DateTimeOffset? value)
    {
        return value is null ? DBNull.Value : value.Value.ToString("O");
    }

    private static string? GetNullableString(SqliteDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);

        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }

    private static DateTimeOffset GetDateTimeOffset(SqliteDataReader reader, string columnName)
    {
        string value = reader.GetString(reader.GetOrdinal(columnName));

        return DateTimeOffset.Parse(value);
    }

    private static DateTimeOffset? GetNullableDateTimeOffset(SqliteDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);

        if (reader.IsDBNull(ordinal))
        {
            return null;
        }

        return DateTimeOffset.Parse(reader.GetString(ordinal));
    }
}
using System.Data;
using Directfn.Custody.ApiFramework.Database;
using Oracle.ManagedDataAccess.Client;

namespace Directfn.Custody.ApiFramework.Authentication.TokenStore.Oracle;

public sealed class OracleAuthTokenStore : IAuthTokenStore
{
    private readonly IDbConnectionFactory _connectionFactory;

    public OracleAuthTokenStore(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CreateSessionAsync(AuthSessionRecord session, CancellationToken cancellationToken)
    {
        await using var connection = (OracleConnection)_connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.BindByName = true;
        command.CommandType = CommandType.Text;
        command.CommandText = """
        INSERT INTO AUTH_SESSIONS (
            SESSION_ID,
            USER_ID,
            USER_NAME,
            CREATED_AT_UTC,
            EXPIRES_AT_UTC,
            REVOKED_AT_UTC,
            IP_ADDRESS,
            USER_AGENT
        )
        VALUES (
            :SESSION_ID,
            :USER_ID,
            :USER_NAME,
            :CREATED_AT_UTC,
            :EXPIRES_AT_UTC,
            :REVOKED_AT_UTC,
            :IP_ADDRESS,
            :USER_AGENT
        )
        """;

        command.Parameters.Add("SESSION_ID", OracleDbType.Varchar2).Value = session.SessionId;
        command.Parameters.Add("USER_ID", OracleDbType.Varchar2).Value = session.UserId;
        command.Parameters.Add("USER_NAME", OracleDbType.Varchar2).Value = ToDbValue(session.UserName);
        command.Parameters.Add("CREATED_AT_UTC", OracleDbType.TimeStamp).Value = session.CreatedAtUtc.UtcDateTime;
        command.Parameters.Add("EXPIRES_AT_UTC", OracleDbType.TimeStamp).Value = session.ExpiresAtUtc.UtcDateTime;
        command.Parameters.Add("REVOKED_AT_UTC", OracleDbType.TimeStamp).Value = ToDbValue(session.RevokedAtUtc);
        command.Parameters.Add("IP_ADDRESS", OracleDbType.Varchar2).Value = ToDbValue(session.IpAddress);
        command.Parameters.Add("USER_AGENT", OracleDbType.Varchar2).Value = ToDbValue(session.UserAgent);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task StoreRefreshTokenAsync(RefreshTokenRecord refreshToken, CancellationToken cancellationToken)
    {
        await using var connection = (OracleConnection)_connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.BindByName = true;
        command.CommandType = CommandType.Text;
        command.CommandText = """
        INSERT INTO AUTH_REFRESH_TOKENS (
            REFRESH_TOKEN_ID,
            SESSION_ID,
            USER_ID,
            TOKEN_HASH,
            CREATED_AT_UTC,
            EXPIRES_AT_UTC,
            USED_AT_UTC,
            REVOKED_AT_UTC,
            REPLACED_BY_TOKEN_HASH,
            CREATED_BY_IP,
            USER_AGENT
        )
        VALUES (
            :REFRESH_TOKEN_ID,
            :SESSION_ID,
            :USER_ID,
            :TOKEN_HASH,
            :CREATED_AT_UTC,
            :EXPIRES_AT_UTC,
            :USED_AT_UTC,
            :REVOKED_AT_UTC,
            :REPLACED_BY_TOKEN_HASH,
            :CREATED_BY_IP,
            :USER_AGENT
        )
        """;

        command.Parameters.Add("REFRESH_TOKEN_ID", OracleDbType.Varchar2).Value = refreshToken.RefreshTokenId;
        command.Parameters.Add("SESSION_ID", OracleDbType.Varchar2).Value = refreshToken.SessionId;
        command.Parameters.Add("USER_ID", OracleDbType.Varchar2).Value = refreshToken.UserId;
        command.Parameters.Add("TOKEN_HASH", OracleDbType.Varchar2).Value = refreshToken.TokenHash;
        command.Parameters.Add("CREATED_AT_UTC", OracleDbType.TimeStamp).Value = refreshToken.CreatedAtUtc.UtcDateTime;
        command.Parameters.Add("EXPIRES_AT_UTC", OracleDbType.TimeStamp).Value = refreshToken.ExpiresAtUtc.UtcDateTime;
        command.Parameters.Add("USED_AT_UTC", OracleDbType.TimeStamp).Value = ToDbValue(refreshToken.UsedAtUtc);
        command.Parameters.Add("REVOKED_AT_UTC", OracleDbType.TimeStamp).Value = ToDbValue(refreshToken.RevokedAtUtc);
        command.Parameters.Add("REPLACED_BY_TOKEN_HASH", OracleDbType.Varchar2).Value = ToDbValue(refreshToken.ReplacedByTokenHash);
        command.Parameters.Add("CREATED_BY_IP", OracleDbType.Varchar2).Value = ToDbValue(refreshToken.CreatedByIp);
        command.Parameters.Add("USER_AGENT", OracleDbType.Varchar2).Value = ToDbValue(refreshToken.UserAgent);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<bool> IsSessionActiveAsync(string userId, string sessionId, CancellationToken cancellationToken)
    {
        await using var connection = (OracleConnection)_connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.BindByName = true;
        command.CommandType = CommandType.Text;
        command.CommandText = """
        SELECT COUNT(1)
        FROM AUTH_SESSIONS
        WHERE SESSION_ID = :SESSION_ID
          AND USER_ID = :USER_ID
          AND REVOKED_AT_UTC IS NULL
          AND EXPIRES_AT_UTC > :NOW_UTC
        """;

        command.Parameters.Add("SESSION_ID", OracleDbType.Varchar2).Value = sessionId;
        command.Parameters.Add("USER_ID", OracleDbType.Varchar2).Value = userId;
        command.Parameters.Add("NOW_UTC", OracleDbType.TimeStamp).Value = DateTime.UtcNow;

        object? result = await command.ExecuteScalarAsync(cancellationToken);

        return Convert.ToInt32(result) > 0;
    }

    public async Task<RefreshTokenValidationResult> ValidateRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken)
    {
        await using var connection = (OracleConnection)_connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.BindByName = true;
        command.CommandType = CommandType.Text;
        command.CommandText = """
    SELECT
        rt.REFRESH_TOKEN_ID,
        rt.SESSION_ID,
        rt.USER_ID,
        rt.TOKEN_HASH,
        rt.CREATED_AT_UTC,
        rt.EXPIRES_AT_UTC,
        rt.USED_AT_UTC,
        rt.REVOKED_AT_UTC,
        rt.REPLACED_BY_TOKEN_HASH,
        rt.CREATED_BY_IP,
        rt.USER_AGENT,
        s.USER_NAME,
        s.CREATED_AT_UTC AS SESSION_CREATED_AT_UTC,
        s.EXPIRES_AT_UTC AS SESSION_EXPIRES_AT_UTC,
        s.REVOKED_AT_UTC AS SESSION_REVOKED_AT_UTC,
        s.IP_ADDRESS AS SESSION_IP_ADDRESS,
        s.USER_AGENT AS SESSION_USER_AGENT
    FROM AUTH_REFRESH_TOKENS rt
    INNER JOIN AUTH_SESSIONS s ON s.SESSION_ID = rt.SESSION_ID
    WHERE rt.TOKEN_HASH = :TOKEN_HASH
    FETCH FIRST 1 ROWS ONLY
    """;

        command.Parameters.Add("TOKEN_HASH", OracleDbType.Varchar2).Value = refreshTokenHash;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return RefreshTokenValidationResult.Fail(RefreshTokenValidationStatus.NotFound);
        }

        var session = new AuthSessionRecord
        {
            SessionId = reader.GetString(reader.GetOrdinal("SESSION_ID")),
            UserId = reader.GetString(reader.GetOrdinal("USER_ID")),
            UserName = GetNullableString(reader, "USER_NAME"),
            CreatedAtUtc = GetDateTimeOffset(reader, "SESSION_CREATED_AT_UTC"),
            ExpiresAtUtc = GetDateTimeOffset(reader, "SESSION_EXPIRES_AT_UTC"),
            RevokedAtUtc = GetNullableDateTimeOffset(reader, "SESSION_REVOKED_AT_UTC"),
            IpAddress = GetNullableString(reader, "SESSION_IP_ADDRESS"),
            UserAgent = GetNullableString(reader, "SESSION_USER_AGENT")
        };

        var refreshToken = new RefreshTokenRecord
        {
            RefreshTokenId = reader.GetString(reader.GetOrdinal("REFRESH_TOKEN_ID")),
            SessionId = reader.GetString(reader.GetOrdinal("SESSION_ID")),
            UserId = reader.GetString(reader.GetOrdinal("USER_ID")),
            TokenHash = reader.GetString(reader.GetOrdinal("TOKEN_HASH")),
            CreatedAtUtc = GetDateTimeOffset(reader, "CREATED_AT_UTC"),
            ExpiresAtUtc = GetDateTimeOffset(reader, "EXPIRES_AT_UTC"),
            UsedAtUtc = GetNullableDateTimeOffset(reader, "USED_AT_UTC"),
            RevokedAtUtc = GetNullableDateTimeOffset(reader, "REVOKED_AT_UTC"),
            ReplacedByTokenHash = GetNullableString(reader, "REPLACED_BY_TOKEN_HASH"),
            CreatedByIp = GetNullableString(reader, "CREATED_BY_IP"),
            UserAgent = GetNullableString(reader, "USER_AGENT")
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
        await using var connection = (OracleConnection)_connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var transaction = connection.BeginTransaction();

        try
        {
            await using var updateCommand = connection.CreateCommand();

            updateCommand.Transaction = transaction;
            updateCommand.BindByName = true;
            updateCommand.CommandType = CommandType.Text;
            updateCommand.CommandText = """
        UPDATE AUTH_REFRESH_TOKENS
        SET
            USED_AT_UTC = :USED_AT_UTC,
            REPLACED_BY_TOKEN_HASH = :REPLACED_BY_TOKEN_HASH
        WHERE TOKEN_HASH = :CURRENT_TOKEN_HASH
          AND USED_AT_UTC IS NULL
          AND REVOKED_AT_UTC IS NULL
        """;

            updateCommand.Parameters.Add("USED_AT_UTC", OracleDbType.TimeStamp).Value = DateTime.UtcNow;
            updateCommand.Parameters.Add("REPLACED_BY_TOKEN_HASH", OracleDbType.Varchar2).Value = newRefreshToken.TokenHash;
            updateCommand.Parameters.Add("CURRENT_TOKEN_HASH", OracleDbType.Varchar2).Value = currentRefreshTokenHash;

            int affectedRows = await updateCommand.ExecuteNonQueryAsync(cancellationToken);

            if (affectedRows != 1)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new InvalidOperationException("Refresh token rotation failed. Current token may already be used or revoked.");
            }

            await using var insertCommand = connection.CreateCommand();

            insertCommand.Transaction = transaction;
            insertCommand.BindByName = true;
            insertCommand.CommandType = CommandType.Text;
            insertCommand.CommandText = """
        INSERT INTO AUTH_REFRESH_TOKENS (
            REFRESH_TOKEN_ID,
            SESSION_ID,
            USER_ID,
            TOKEN_HASH,
            CREATED_AT_UTC,
            EXPIRES_AT_UTC,
            USED_AT_UTC,
            REVOKED_AT_UTC,
            REPLACED_BY_TOKEN_HASH,
            CREATED_BY_IP,
            USER_AGENT
        )
        VALUES (
            :REFRESH_TOKEN_ID,
            :SESSION_ID,
            :USER_ID,
            :TOKEN_HASH,
            :CREATED_AT_UTC,
            :EXPIRES_AT_UTC,
            :USED_AT_UTC,
            :REVOKED_AT_UTC,
            :REPLACED_BY_TOKEN_HASH,
            :CREATED_BY_IP,
            :USER_AGENT
        )
        """;

            AddRefreshTokenParameters(insertCommand, newRefreshToken);

            await insertCommand.ExecuteNonQueryAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task RevokeSessionAsync(string sessionId, CancellationToken cancellationToken)
    {
        await using var connection = (OracleConnection)_connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var transaction = connection.BeginTransaction();

        try
        {
            DateTime revokedAtUtc = DateTime.UtcNow;

            await using var sessionCommand = connection.CreateCommand();

            sessionCommand.Transaction = transaction;
            sessionCommand.BindByName = true;
            sessionCommand.CommandType = CommandType.Text;
            sessionCommand.CommandText = """
        UPDATE AUTH_SESSIONS
        SET REVOKED_AT_UTC = :REVOKED_AT_UTC
        WHERE SESSION_ID = :SESSION_ID
          AND REVOKED_AT_UTC IS NULL
        """;

            sessionCommand.Parameters.Add("REVOKED_AT_UTC", OracleDbType.TimeStamp).Value = revokedAtUtc;
            sessionCommand.Parameters.Add("SESSION_ID", OracleDbType.Varchar2).Value = sessionId;

            await sessionCommand.ExecuteNonQueryAsync(cancellationToken);

            await using var tokenCommand = connection.CreateCommand();

            tokenCommand.Transaction = transaction;
            tokenCommand.BindByName = true;
            tokenCommand.CommandType = CommandType.Text;
            tokenCommand.CommandText = """
        UPDATE AUTH_REFRESH_TOKENS
        SET REVOKED_AT_UTC = :REVOKED_AT_UTC
        WHERE SESSION_ID = :SESSION_ID
          AND REVOKED_AT_UTC IS NULL
        """;

            tokenCommand.Parameters.Add("REVOKED_AT_UTC", OracleDbType.TimeStamp).Value = revokedAtUtc;
            tokenCommand.Parameters.Add("SESSION_ID", OracleDbType.Varchar2).Value = sessionId;

            await tokenCommand.ExecuteNonQueryAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    private static void AddRefreshTokenParameters(OracleCommand command, RefreshTokenRecord refreshToken)
    {
        command.Parameters.Add("REFRESH_TOKEN_ID", OracleDbType.Varchar2).Value = refreshToken.RefreshTokenId;
        command.Parameters.Add("SESSION_ID", OracleDbType.Varchar2).Value = refreshToken.SessionId;
        command.Parameters.Add("USER_ID", OracleDbType.Varchar2).Value = refreshToken.UserId;
        command.Parameters.Add("TOKEN_HASH", OracleDbType.Varchar2).Value = refreshToken.TokenHash;
        command.Parameters.Add("CREATED_AT_UTC", OracleDbType.TimeStamp).Value = refreshToken.CreatedAtUtc.UtcDateTime;
        command.Parameters.Add("EXPIRES_AT_UTC", OracleDbType.TimeStamp).Value = refreshToken.ExpiresAtUtc.UtcDateTime;
        command.Parameters.Add("USED_AT_UTC", OracleDbType.TimeStamp).Value = ToDbValue(refreshToken.UsedAtUtc);
        command.Parameters.Add("REVOKED_AT_UTC", OracleDbType.TimeStamp).Value = ToDbValue(refreshToken.RevokedAtUtc);
        command.Parameters.Add("REPLACED_BY_TOKEN_HASH", OracleDbType.Varchar2).Value = ToDbValue(refreshToken.ReplacedByTokenHash);
        command.Parameters.Add("CREATED_BY_IP", OracleDbType.Varchar2).Value = ToDbValue(refreshToken.CreatedByIp);
        command.Parameters.Add("USER_AGENT", OracleDbType.Varchar2).Value = ToDbValue(refreshToken.UserAgent);
    }
    private static string? GetNullableString(OracleDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);

        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }

    private static DateTimeOffset GetDateTimeOffset(OracleDataReader reader, string columnName)
    {
        DateTime value = reader.GetDateTime(reader.GetOrdinal(columnName));

        return new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc));
    }

    private static DateTimeOffset? GetNullableDateTimeOffset(OracleDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);

        if (reader.IsDBNull(ordinal))
        {
            return null;
        }

        DateTime value = reader.GetDateTime(ordinal);

        return new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc));
    }

    private static object ToDbValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? DBNull.Value : value;
    }

    private static object ToDbValue(DateTimeOffset? value)
    {
        return value is null ? DBNull.Value : value.Value.UtcDateTime;
    }
}
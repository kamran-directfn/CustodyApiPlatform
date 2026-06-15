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

    public Task<RefreshTokenValidationResult> ValidateRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RotateRefreshTokenAsync(string currentRefreshTokenHash, RefreshTokenRecord newRefreshToken, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task RevokeSessionAsync(string sessionId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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
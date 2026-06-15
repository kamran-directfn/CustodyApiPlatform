namespace Directfn.Custody.ApiFramework.Authentication.TokenStore;

public sealed class AuthTokenStoreOptions
{
    public const string SectionName = "AuthTokenStore";

    public AuthTokenStoreProvider Provider { get; init; } = AuthTokenStoreProvider.SQLite;

    public string? ConnectionString { get; init; }

    public string? ConnectionStringName { get; init; }

    public bool InitializeDatabase { get; init; } = true;
}
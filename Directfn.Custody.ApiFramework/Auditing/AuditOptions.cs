namespace Directfn.Custody.ApiFramework.Auditing;

public sealed class AuditOptions
{
    public const string SectionName = "Audit";

    public bool Enabled { get; init; } = true;

    public AuditStoreProvider Provider { get; init; } = AuditStoreProvider.SQLite;

    public string? ConnectionString { get; init; }

    public string? ConnectionStringName { get; init; }

    public bool InitializeDatabase { get; init; } = true;

    public bool IncludeRequestBody { get; init; } = false;

    public bool IncludeResponseBody { get; init; } = false;

    public int MaxBodyLength { get; init; } = 4000;
}
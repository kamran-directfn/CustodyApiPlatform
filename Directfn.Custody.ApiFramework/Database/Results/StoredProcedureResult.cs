namespace Directfn.Custody.ApiFramework.Database.Results;

public sealed class StoredProcedureResult
{
    public int RowsAffected { get; init; }

    public IReadOnlyDictionary<string, object?> OutputParameters { get; init; } = new Dictionary<string, object?>();
}
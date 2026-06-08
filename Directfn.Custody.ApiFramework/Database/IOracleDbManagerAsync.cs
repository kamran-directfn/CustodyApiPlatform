using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Directfn.Custody.ApiFramework.Database
{
    public interface IOracleDbManagerAsync
    {
        Task<DataTable> GetQueryResultAsync(string query, IEnumerable<OracleParameter>? parameters = null, CancellationToken cancellationToken = default);

        Task<DataTable> GetStoredProcedureResultAsync(string procedureName, IEnumerable<OracleParameter>? parameters = null, CancellationToken cancellationToken = default);

        Task<DataTable> GetStoredProcedureRefCursorAsync(string procedureName, IEnumerable<OracleParameter> parameters, string refCursorParameterName, CancellationToken cancellationToken = default);

        Task<int> ExecuteNonQueryAsync(string query, IEnumerable<OracleParameter>? parameters = null, CancellationToken cancellationToken = default);

        Task<object?> ExecuteScalarAsync(string query, IEnumerable<OracleParameter>? parameters = null, CancellationToken cancellationToken = default);

        Task<List<T>> GetStoredProcedureRefCursorAsync<T>(string procedureName, IEnumerable<OracleParameter> parameters, string refCursorParameterName, CancellationToken cancellationToken = default) where T : class, new();
    }
}
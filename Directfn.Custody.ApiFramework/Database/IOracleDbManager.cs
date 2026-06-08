using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Directfn.Custody.ApiFramework.Database
{
    public interface IOracleDbManager
    {
        DataTable GetQueryResult(string query, IEnumerable<OracleParameter>? parameters = null);

        DataTable GetStoredProcedureResult(string procedureName, IEnumerable<OracleParameter>? parameters = null);

        DataSet GetStoredProcedureDataSetResult(string procedureName, IEnumerable<OracleParameter>? parameters = null);

        int ExecuteNonQuery(string query, IEnumerable<OracleParameter>? parameters = null);

        object? ExecuteScalar(string query, IEnumerable<OracleParameter>? parameters = null);
    }
}
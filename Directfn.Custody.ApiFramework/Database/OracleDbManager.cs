using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Directfn.Custody.ApiFramework.Database;

public sealed class OracleDbManager : IOracleDbManager
{
    private readonly IDbConnectionFactory _connectionFactory;

    public OracleDbManager(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public DataTable GetQueryResult(
        string query,
        IEnumerable<OracleParameter>? parameters = null)
    {
        using var connection = (OracleConnection)_connectionFactory.CreateConnection();
        using var command = connection.CreateCommand();

        command.CommandType = CommandType.Text;
        command.CommandText = query;

        AddParameters(command, parameters);

        using var adapter = new OracleDataAdapter(command);
        var dataSet = new DataSet();

        connection.Open();
        adapter.Fill(dataSet);

        return dataSet.Tables.Count > 0
            ? dataSet.Tables[0]
            : new DataTable();
    }

    public DataTable GetStoredProcedureResult(
        string procedureName,
        IEnumerable<OracleParameter>? parameters = null)
    {
        using var connection = (OracleConnection)_connectionFactory.CreateConnection();
        using var command = connection.CreateCommand();

        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = procedureName;

        AddParameters(command, parameters);

        using var adapter = new OracleDataAdapter(command);
        var dataSet = new DataSet();

        connection.Open();
        adapter.Fill(dataSet);

        return dataSet.Tables.Count > 0
            ? dataSet.Tables[0]
            : new DataTable();
    }

    public DataSet GetStoredProcedureDataSetResult(
        string procedureName,
        IEnumerable<OracleParameter>? parameters = null)
    {
        using var connection = (OracleConnection)_connectionFactory.CreateConnection();
        using var command = connection.CreateCommand();

        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = procedureName;

        AddParameters(command, parameters);

        using var adapter = new OracleDataAdapter(command);
        var dataSet = new DataSet();

        connection.Open();
        adapter.Fill(dataSet);

        return dataSet;
    }

    public int ExecuteNonQuery(
        string query,
        IEnumerable<OracleParameter>? parameters = null)
    {
        using var connection = (OracleConnection)_connectionFactory.CreateConnection();
        using var command = connection.CreateCommand();

        command.CommandType = CommandType.Text;
        command.CommandText = query;

        AddParameters(command, parameters);

        connection.Open();

        return command.ExecuteNonQuery();
    }

    public object? ExecuteScalar(
        string query,
        IEnumerable<OracleParameter>? parameters = null)
    {
        using var connection = (OracleConnection)_connectionFactory.CreateConnection();
        using var command = connection.CreateCommand();

        command.CommandType = CommandType.Text;
        command.CommandText = query;

        AddParameters(command, parameters);

        connection.Open();

        return command.ExecuteScalar();
    }

    private static void AddParameters(
        OracleCommand command,
        IEnumerable<OracleParameter>? parameters)
    {
        if (parameters is null)
        {
            return;
        }

        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }
    }
}
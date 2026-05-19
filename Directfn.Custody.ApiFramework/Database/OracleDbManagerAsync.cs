using System.Data;
using Oracle.ManagedDataAccess.Client;
using Directfn.Custody.ApiFramework.Database.Mapping;
namespace Directfn.Custody.ApiFramework.Database;

public sealed class OracleDbManagerAsync : IOracleDbManagerAsync
{
    private readonly IDbConnectionFactory _connectionFactory;

    public OracleDbManagerAsync(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<DataTable> GetQueryResultAsync(
        string query,
        IEnumerable<OracleParameter>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using var connection = (OracleConnection)_connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();

        command.CommandType = CommandType.Text;
        command.CommandText = query;

        AddParameters(command, parameters);

        await connection.OpenAsync(cancellationToken);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var dataTable = new DataTable();
        dataTable.Load(reader);

        return dataTable;
    }

    public async Task<DataTable> GetStoredProcedureResultAsync(
        string procedureName,
        IEnumerable<OracleParameter>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using var connection = (OracleConnection)_connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();

        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = procedureName;

        AddParameters(command, parameters);

        await connection.OpenAsync(cancellationToken);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var dataTable = new DataTable();
        dataTable.Load(reader);

        return dataTable;
    }

    public async Task<int> ExecuteNonQueryAsync(
        string query,
        IEnumerable<OracleParameter>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using var connection = (OracleConnection)_connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();

        command.CommandType = CommandType.Text;
        command.CommandText = query;

        AddParameters(command, parameters);

        await connection.OpenAsync(cancellationToken);

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<object?> ExecuteScalarAsync(
        string query,
        IEnumerable<OracleParameter>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using var connection = (OracleConnection)_connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();

        command.CommandType = CommandType.Text;
        command.CommandText = query;

        AddParameters(command, parameters);

        await connection.OpenAsync(cancellationToken);

        return await command.ExecuteScalarAsync(cancellationToken);
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

    public async Task<DataTable> GetStoredProcedureRefCursorAsync(
    string procedureName,
    IEnumerable<OracleParameter> parameters,
    string refCursorParameterName,
    CancellationToken cancellationToken = default)
    {
        await using var connection = (OracleConnection)_connectionFactory.CreateConnection();
        await using var command = connection.CreateCommand();

        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = procedureName;
        command.BindByName = true;

        AddParameters(command, parameters);

        if (!command.Parameters.Contains(refCursorParameterName))
        {
            command.Parameters.Add(
                refCursorParameterName,
                OracleDbType.RefCursor,
                ParameterDirection.Output);
        }

        await connection.OpenAsync(cancellationToken);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var dataTable = new DataTable();
        dataTable.Load(reader);

        return dataTable;
    }

    public async Task<List<T>> GetStoredProcedureRefCursorAsync<T>(
    string procedureName,
    IEnumerable<OracleParameter> parameters,
    string refCursorParameterName,
    CancellationToken cancellationToken = default)
    where T : class, new()
    {
        var dataTable = await GetStoredProcedureRefCursorAsync(
            procedureName,
            parameters,
            refCursorParameterName,
            cancellationToken);

        return DataTableMapper.ToList<T>(dataTable);
    }

}
using System.Data;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.DTOs.User;
using Oracle.ManagedDataAccess.Client;

namespace Directfn.Custody.ApiFramework.Repositories.User;

public sealed class UserRepository : IUserRepository
{
    private readonly IOracleDbManagerAsync _dbManager;

    public UserRepository(IOracleDbManagerAsync dbManager)
    {
        _dbManager = dbManager;
    }

    public async Task<LoginUserRecord?> GetUserForLoginAsync(
        string loginId,
        long rf48Code,
        CancellationToken cancellationToken)
    {
        var parameters = new List<OracleParameter>
        {
            new("pview", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            },
            new("P_login", OracleDbType.Varchar2)
            {
                Direction = ParameterDirection.Input,
                Value = loginId
            },
            new("p_rf48_code", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.Input,
                Value = rf48Code
            }
        };

        var users = await _dbManager.GetStoredProcedureRefCursorAsync<LoginUserRecord>(
            "Pkg_UM02_USERS.User_Login",
            parameters,
            "pview",
            cancellationToken);

        return users.FirstOrDefault();
    }
}
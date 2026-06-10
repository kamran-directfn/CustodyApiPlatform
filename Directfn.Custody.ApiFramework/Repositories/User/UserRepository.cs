using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.DTOs.User;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Directfn.Custody.ApiFramework.Repositories.User
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;

        public UserRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<LoginUserRecord?> GetUserForLoginAsync(string loginId, long rf48Code, CancellationToken cancellationToken)
        {
            List<OracleParameter> parameters = new() { new OracleParameter("pview", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }, new OracleParameter("P_login", OracleDbType.Varchar2) { Direction = ParameterDirection.Input, Value = loginId }, new OracleParameter("p_rf48_code", OracleDbType.Decimal) { Direction = ParameterDirection.Input, Value = rf48Code } };

            List<LoginUserRecord> users = await _dbManager.GetStoredProcedureRefCursorAsync<LoginUserRecord>("Pkg_UM02_USERS.User_Login", parameters, "pview", cancellationToken);

            return users.FirstOrDefault();
        }
    }
}

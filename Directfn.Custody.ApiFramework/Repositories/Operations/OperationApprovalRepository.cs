using System.Data;
using Directfn.Custody.ApiFramework.Database;
using Directfn.Custody.ApiFramework.DTOs.Approvals;
using Oracle.ManagedDataAccess.Client;

namespace Directfn.Custody.ApiFramework.Repositories.Operations;

public sealed class OperationApprovalRepository : IOperationApprovalRepository
{
    private readonly IOracleDbManagerAsync _dbManager;

    public OperationApprovalRepository(IOracleDbManagerAsync dbManager)
    {
        _dbManager = dbManager;
    }

    public async Task<CanApproveDisapproveResponse?> CheckUserCanPerformOperationAsync(long userId, long memberCodeId, string screen, string recordIds, CancellationToken cancellationToken)
    {
        var parameters = new List<OracleParameter>
        {
            new("pview", OracleDbType.RefCursor)
            {
                Direction = ParameterDirection.Output
            },
            new("p_user_id", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.Input,
                Value = userId
            },
            new("p_rf48_id", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.Input,
                Value = memberCodeId
            },
            new("p_screen_name", OracleDbType.Varchar2)
            {
                Direction = ParameterDirection.Input,
                Value = screen
            },
            new("p_action_type", OracleDbType.Varchar2)
            {
                Direction = ParameterDirection.Input,
                Value = string.Empty
            },
            new("p_record_ids", OracleDbType.Varchar2)
            {
                Direction = ParameterDirection.Input,
                Value = recordIds
            },
            new("p_modified_by", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.Input,
                Value = 0
            }
        };

        var result = await _dbManager.GetStoredProcedureRefCursorAsync<CanApproveDisapproveResponse>(
            "pkg_approve_disapprove.can_approve_disapprove",
            parameters,
            "pview",
            cancellationToken);

        return result.FirstOrDefault();
    }
}
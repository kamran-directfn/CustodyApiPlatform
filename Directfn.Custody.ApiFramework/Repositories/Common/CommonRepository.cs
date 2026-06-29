using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Common.DTOs.Users;
using Directfn.Custody.ApiFramework.Database;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Common
{
    public sealed class CommonRepository : ICommonRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;

        public CommonRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<List<DropDowns>> GetRoles(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            OracleParameter Pview = new OracleParameter();
            Pview.ParameterName = "Pview";
            Pview.OracleDbType = OracleDbType.RefCursor;
            Pview.Direction = System.Data.ParameterDirection.Output;
            lstParams.Add(Pview);

            var data = await _dbManager.GetStoredProcedureRefCursorAsync<DropDowns>("Pkg_Dropdowns.Get_Roles_DropDown_New", lstParams, "Pview", cancellationToken);

            return data;
        }

        public async Task<List<DropDowns>> GetMemberCode(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            OracleParameter Pview = new OracleParameter();
            Pview.ParameterName = "Pview";
            Pview.OracleDbType = OracleDbType.RefCursor;
            Pview.Direction = System.Data.ParameterDirection.Output;
            lstParams.Add(Pview);

            var data = await _dbManager.GetStoredProcedureRefCursorAsync<DropDowns>("Pkg_Dropdowns.Get_MemberCode_DropDown_New", lstParams, "Pview", cancellationToken);

            return data;
        }

        public async Task<List<PortfoliosByMembers>> GetGroupsByMember(CancellationToken cancellationToken)
        {
            List<OracleParameter> lstParams = new List<OracleParameter>();

            OracleParameter Pview = new OracleParameter();
            Pview.ParameterName = "Pview";
            Pview.OracleDbType = OracleDbType.RefCursor;
            Pview.Direction = System.Data.ParameterDirection.Output;
            lstParams.Add(Pview);

            var data = await _dbManager.GetStoredProcedureRefCursorAsync<PortfoliosByMembers>("pkg_portfolio_groups.get_groups_by_member", lstParams, "Pview", cancellationToken);

            return data;
        }
    }
}

using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Common.DTOs.DashBoard;
using Directfn.Custody.ApiFramework.Database;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.DashBoard
{
    public sealed class DashBoardRepository : IDashBoardRepository
    {
        private readonly IOracleDbManagerAsync _dbManager;

        public DashBoardRepository(IOracleDbManagerAsync dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task<MarketTrades> GetTradeMessagesCountSummary(DateTime settlementDate, int memberCodeId, int groupId, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                List<MarketTrades> data = new List<MarketTrades>();
                List<OracleParameter> parameters = new List<OracleParameter>();

                parameters.Add(new OracleParameter() { ParameterName = "pview", Direction = System.Data.ParameterDirection.Output, OracleDbType = OracleDbType.RefCursor });
                parameters.Add(new OracleParameter() { ParameterName = "p_settlement_date", Value = settlementDate, Direction = System.Data.ParameterDirection.Input });
                parameters.Add(new OracleParameter() { ParameterName = "p_memberid", Value = memberCodeId, Direction = System.Data.ParameterDirection.Input });
                parameters.Add(new OracleParameter() { ParameterName = "p_group", Value = groupId, Direction = System.Data.ParameterDirection.Input });

                data = await _dbManager.GetStoredProcedureRefCursorAsync<MarketTrades>("pttp_dashboard.get_trades_message_det", parameters, "pview", cancellationToken);

                return data.FirstOrDefault();
            });
        }

        public async Task<Mt530PendingTradesDetail> GetMt530PendingTradeDetail(DateTime settlementDate, int memberCodeId, int groupId, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                List<Mt530PendingTradesDetail> data = new List<Mt530PendingTradesDetail>();
                List<OracleParameter> parameters = new List<OracleParameter>();

                parameters.Add(new OracleParameter() { ParameterName = "pview", Direction = System.Data.ParameterDirection.Output, OracleDbType = OracleDbType.RefCursor });
                parameters.Add(new OracleParameter() { ParameterName = "p_settlement_date", Value = settlementDate, Direction = System.Data.ParameterDirection.Input });
                parameters.Add(new OracleParameter() { ParameterName = "p_memberid", Value = memberCodeId, Direction = System.Data.ParameterDirection.Input });
                parameters.Add(new OracleParameter() { ParameterName = "p_group", Value = groupId, Direction = System.Data.ParameterDirection.Input });
                data = await _dbManager.GetStoredProcedureRefCursorAsync<Mt530PendingTradesDetail>(" pttp_dashboard.get_pend_trades_det", parameters, "pview", cancellationToken);

                return data.FirstOrDefault();
            });
        }

        public async Task<NetObligations> GetNetObligations(DateTime settlementDate, int memberCodeId, int groupId, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                List<NetObligations> data = new List<NetObligations>();
                List<OracleParameter> parameters = new List<OracleParameter>();

                parameters.Add(new OracleParameter() { ParameterName = "pview", Direction = System.Data.ParameterDirection.Output, OracleDbType = OracleDbType.RefCursor });
                parameters.Add(new OracleParameter() { ParameterName = "p_settlement_date", Value = settlementDate, Direction = System.Data.ParameterDirection.Input });
                parameters.Add(new OracleParameter() { ParameterName = "p_memberid", Value = memberCodeId, Direction = System.Data.ParameterDirection.Input });
                parameters.Add(new OracleParameter() { ParameterName = "p_group", Value = groupId, Direction = System.Data.ParameterDirection.Input });
                data = await _dbManager.GetStoredProcedureRefCursorAsync<NetObligations>(" pttp_dashboard.get_net_obligations", parameters, "pview", cancellationToken);

                return data.FirstOrDefault();
            });
        }

        public async Task<SettlementDetails> GetSettlementDetails(DateTime settlementDate, int memberCodeId, int groupId, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                List<SettlementDetails> data = new List<SettlementDetails>();
                List<OracleParameter> parameters = new List<OracleParameter>();

                parameters.Add(new OracleParameter() { ParameterName = "pview", Direction = System.Data.ParameterDirection.Output, OracleDbType = OracleDbType.RefCursor });
                parameters.Add(new OracleParameter() { ParameterName = "p_settlement_date", Value = settlementDate, Direction = System.Data.ParameterDirection.Input });
                parameters.Add(new OracleParameter() { ParameterName = "p_memberid", Value = memberCodeId, Direction = System.Data.ParameterDirection.Input });
                parameters.Add(new OracleParameter() { ParameterName = "p_group", Value = groupId, Direction = System.Data.ParameterDirection.Input });
                data = await _dbManager.GetStoredProcedureRefCursorAsync<SettlementDetails>(" pttp_dashboard.get_settle_trades_det", parameters, "pview", cancellationToken);

                return data.FirstOrDefault();
            });
        }

        public async Task<HeadRoomCalculations> GetForcastDetails(DateTime settlementDate, int memberCodeId, int groupId, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                List<HeadRoomCalculations> data = new List<HeadRoomCalculations>();
                List<OracleParameter> parameters = new List<OracleParameter>();

                parameters.Add(new OracleParameter() { ParameterName = "pview", Direction = System.Data.ParameterDirection.Output, OracleDbType = OracleDbType.RefCursor });
                parameters.Add(new OracleParameter() { ParameterName = "p_settlement_date", Value = settlementDate, Direction = System.Data.ParameterDirection.Input });
                parameters.Add(new OracleParameter() { ParameterName = "p_memberid", Value = memberCodeId, Direction = System.Data.ParameterDirection.Input });
                parameters.Add(new OracleParameter() { ParameterName = "p_group", Value = groupId, Direction = System.Data.ParameterDirection.Input });
                data = await _dbManager.GetStoredProcedureRefCursorAsync<HeadRoomCalculations>(" pttp_dashboard.GET_HEADROOM_CALCULATIONS", parameters, "pview", cancellationToken);

                return data.FirstOrDefault();
            });
        }

        public async Task<OtherSettlementInstructionsDetails> GetOtherSettlementInstructionDetails(DateTime settlementDate, int memberCodeId, int groupId, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                List<OtherSettlementInstructionsDetails> data = new List<OtherSettlementInstructionsDetails>();
                List<OracleParameter> parameters = new List<OracleParameter>();

                parameters.Add(new OracleParameter() { ParameterName = "pview", Direction = System.Data.ParameterDirection.Output, OracleDbType = OracleDbType.RefCursor });
                parameters.Add(new OracleParameter() { ParameterName = "p_settlement_date", Value = settlementDate, Direction = System.Data.ParameterDirection.Input });
                parameters.Add(new OracleParameter() { ParameterName = "p_memberid", Value = memberCodeId, Direction = System.Data.ParameterDirection.Input });
                parameters.Add(new OracleParameter() { ParameterName = "p_group", Value = groupId, Direction = System.Data.ParameterDirection.Input });
                data = await _dbManager.GetStoredProcedureRefCursorAsync<OtherSettlementInstructionsDetails>(" pttp_dashboard.GET_OTHER_SET_INS_DET", parameters, "pview", cancellationToken);

                return data.FirstOrDefault();
            });
        }
    }
}

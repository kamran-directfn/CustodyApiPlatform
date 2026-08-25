using Directfn.Custody.ApiFramework.Common.DTOs;
using Directfn.Custody.ApiFramework.Common.DTOs.GroupAccounts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Common
{
    public interface ICommonRepository
    {
        Task<List<DropDowns>> GetRoles(CancellationToken cancellationToken);
        Task<List<DropDowns>> GetMemberCode(CancellationToken cancellationToken);
        Task<List<PortfoliosByMembers>> GetGroupsByMember(CancellationToken cancellationToken);
        DataSet UploadFileDataSet(IFormFile file);
        Task<int> GetBatchID(string ScreenName, int rf48_id, CancellationToken cancellationToken);
        Task<List<DropDowns>> GetBanks(CancellationToken cancellationToken);
        Task<List<DropDowns>> GetMarkets(CancellationToken cancellationToken);
        Task<List<DropDowns>> GetCurrencies(CancellationToken cancellationToken);
        Task<List<DropDowns>> GetCountries(CancellationToken cancellationToken);
        Task<List<DropDowns>> GetEconomicSector(CancellationToken cancellationToken);
        Task<List<DropDowns>> GetMarketSector(CancellationToken cancellationToken);
        Task<List<DropDowns>> GetSubMarket(int marketId, CancellationToken cancellationToken);
        List<DropDowns> GetFopTransferTypesDropdown(CancellationToken cancellationToken);
        List<DropDowns> GetTradeTypes(CancellationToken cancellationToken);
        List<DropDowns> GetEdaaStatus(CancellationToken cancellationToken);
        Task<List<GroupAccounts>> GetPortfolioAccountsByUser(int portfolioId, int rf48Id, CancellationToken cancellationToken);

        string GetDateTimeForExportMsgs();
        string GetReqId();
        string EDAA_BIC();
        string GetMT5BasicHeaderBlock();
        string Get_Header_Block(string messageType, string memberCode);
    }
}

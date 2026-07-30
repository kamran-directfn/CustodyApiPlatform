using Directfn.Custody.ApiFramework.Common.DTOs.PortfolioGroup;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.Protocols;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.PortfolioGroup
{
    public interface IPortfolioGroupRepository
    {
        Task<List<PortfolioGroupViewModel>> GetPortfolio(int id, CancellationToken cancellationToken);
        Task<PortfolioGroupById> GetPortfolioByID(int um14_id, CancellationToken cancellationToken);
        Task<string> GetCheckNameMembercodeISExists(string _groupname, int _rf48_id, CancellationToken cancellationToken);
        Task<int> AddUpdatePortfolio(PortfolioGroupReqModel portfolioGroup, CancellationToken cancellationToken);
        Task<List<PortfolioGroupDeleteRes>> DeleteGroup(int um14_id, CancellationToken cancellationToken);
        List<PortfolioGroupValidate> GetPortfolioGroup(DataSet result, int batch_id, int rf48_id, int created_by, CancellationToken cancellationToken);
        Task<bool> BulkInsertPortfolioAccount(List<PortfolioGroupValidate> portfolios, CancellationToken cancellationToken);
        Task<List<PortfolioGroupValidate>> ValidatePortfolioAccounts(int batch_id, int rf48_id, CancellationToken cancellationToken);
    }
}

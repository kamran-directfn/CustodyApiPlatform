using Directfn.Custody.ApiFramework.Common.DTOs.Broker;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Broker
{
    public interface IBrokerRepository
    {
        Task<List<BrokerViewModel>> GetAllBrokersAsync(CancellationToken cancellationToken);
        Task<BrokerViewModel> GetBrokerById(int brokerId, CancellationToken cancellationToken);
        Task<List<BrokerViewModel>> UpdatePostStatus(int rf07_id, int isPosted, int user_id, CancellationToken cancellationToken);
        Task<List<BrokerViewModel>> DeleteBroker(int rf07_id, int user_id, CancellationToken cancellationToken);
        Task<List<BrokerCache>> CacheBrokerData(CancellationToken cancellationToken);
        Task<BrokerReqModel> SaveBroker(BrokerReqModel _broker, CancellationToken cancellationToken);
        Task<BrokerReqModel> UpdateBroker(BrokerReqModel _broker, CancellationToken cancellationToken);
    }
}
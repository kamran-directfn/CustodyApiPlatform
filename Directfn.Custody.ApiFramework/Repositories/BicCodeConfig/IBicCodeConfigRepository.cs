using Directfn.Custody.ApiFramework.Common.DTOs.BicCodeConfig;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.BicCodeConfig
{
    public interface IBicCodeConfigRepository
    {
        Task<List<BicCodeConfigViewModel>> GetAllBicCodeConfigsAsync(CancellationToken cancellationToken);
        Task<BicCodeConfigViewModel> GetBicCodeConfigById(int bicCodeId, CancellationToken cancellationToken);
        Task<List<BicCodeConfigViewModel>> UpdatePostStatus(int rf84_id, int isPosted, int user_id, CancellationToken cancellationToken);
        Task<List<BicCodeConfigViewModel>> DeleteBicCodeConfig(int rf84_id, int user_id, CancellationToken cancellationToken);
        Task<BicCodeConfigReqModel> AddBicCodeConfig(BicCodeConfigReqModel _bicCode, CancellationToken cancellationToken);
        Task<BicCodeConfigReqModel> UpdateBicCodeConfig(BicCodeConfigReqModel _bicCode, CancellationToken cancellationToken);
    }
}

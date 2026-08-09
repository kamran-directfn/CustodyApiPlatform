using Directfn.Custody.ApiFramework.Common.DTOs.HeadRoomConfig;

namespace Directfn.Custody.ApiFramework.Repositories.HeadRoomConfig
{
    public interface IHeadRoomConfigRepository
    {
        Task<List<HeadRoomConfigViewModel>> GetHeadRoomConfigsAsync(CancellationToken cancellationToken);
        Task<HeadRoomConfigReqModel> UpdateHeadRoomConfig(HeadRoomConfigReqModel _headRoom, CancellationToken cancellationToken);
    }
}

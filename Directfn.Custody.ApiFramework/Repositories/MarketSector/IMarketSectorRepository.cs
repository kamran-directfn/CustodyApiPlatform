using Directfn.Custody.ApiFramework.Common.DTOs.MarketSector;

namespace Directfn.Custody.ApiFramework.Repositories.MarketSector
{
    public interface IMarketSectorRepository 
    {
        Task<List<MarketSectorViewModel>> GetAllMarketSectorsAsync(CancellationToken cancellationToken);
        Task<MarketSectorViewModel> GetSectorById(int sectorId, CancellationToken cancellationToken);
        Task<List<MarketSectorViewModel>> UpdatePostStatus(int rf40_id, int isPosted, int user_id, CancellationToken cancellationToken);
        Task<List<MarketSectorViewModel>> DeleteSector(int rf40_id, int user_id, CancellationToken cancellationToken);
        Task<MarketSectorReqModel> AddMarketSector(MarketSectorReqModel _sector, CancellationToken cancellationToken);
        Task<MarketSectorReqModel> UpdateMarketSector(MarketSectorReqModel _sector, CancellationToken cancellationToken);
    }
}

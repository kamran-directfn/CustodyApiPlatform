using Directfn.Custody.ApiFramework.Common.DTOs.Cities;

namespace Directfn.Custody.ApiFramework.Repositories.Cities
{
    public interface ICitiesRepository
    {
        Task<List<CitiesViewModel>> GetAllCitiesAsync(CancellationToken cancellationToken);
        Task<CitiesViewModel> GetCityById(int cityId, CancellationToken cancellationToken);
        Task<List<CitiesViewModel>> UpdatePostStatus(int rf10_city_id, int isPosted, int user_id, CancellationToken cancellationToken);
        Task<List<CitiesViewModel>> DeleteCity(int rf10_city_id, int user_id, CancellationToken cancellationToken);
        Task<CitiesReqModel> SaveCity(CitiesReqModel _cities, CancellationToken cancellationToken);
        Task<CitiesReqModel> UpdateCity(CitiesReqModel _cities, CancellationToken cancellationToken);
    }
}

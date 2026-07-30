using Directfn.Custody.ApiFramework.Common.DTOs.Countries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Directfn.Custody.ApiFramework.Repositories.Countries
{
    public interface ICountriesRepository
    {
        Task<List<CountriesViewModel>> GetAllCountriesAsync(CancellationToken cancellationToken);
        Task<CountriesViewModel> GetCountryById(int countryId, CancellationToken cancellationToken);
        Task<List<CountriesViewModel>> UpdatePostStatus(int rf09_country_id, int isPosted, int user_id, CancellationToken cancellationToken);
        Task<List<CountriesViewModel>> DeleteCountry(int rf09_country_id, int user_id, CancellationToken cancellationToken);
        Task<CountriesReqModel> SaveCountry(CountriesReqModel _countries, CancellationToken cancellationToken);
        Task<CountriesReqModel> UpdateCountry(CountriesReqModel _countries, CancellationToken cancellationToken);
    }
}

using SkyCars.Core;
using SkyCars.Core.DomainEntity.City;
using SkyCars.Core.DomainEntity.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCars.Service.Cities
{
    public partial interface ICityServices
    {
         
        #region Methods
        Task<IPagedList<City>> GetAllAsync(GridRequestModel objGrid);
        Task<IList<City>> GetByIdsAsync(IList<int> ids);
        Task<City> GetByIdAsync(int Id);
        Task InsertAsync(City City, int CityId, string Cityname);
        Task UpdateAsync(City City, int CityId, string Cityname);
        Task UpdateAsync(IList<City> CityList, int CityId, string Cityname);
        Task<bool> IsNameExist(string CityName, int Id);
        #endregion
    }
}


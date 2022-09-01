using SkyCars.Core;
using SkyCars.Core.DomainEntity.City;
using SkyCars.Core.DomainEntity.Grid;
using SkyCars.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCars.Service.Cities
{
    public partial class CityService : ICityServices
    {
        #region Fields

        private readonly IRepository<City> _CityRepository;

        #endregion

        #region Ctor

        public CityService(IRepository<City> CityRepository)
        {
            _CityRepository = CityRepository;
        }

        #endregion
        public virtual async Task<IPagedList<City>> GetAllAsync(GridRequestModel objGrid)
        {

            var query = from d in _CityRepository.Table
                        where !d.IsDelete
                        select new City()
                        {
                            Id = d.Id,
                            CityName = d.CityName,
                            CreatedDate = d.CreatedDate,
                           
                        };

            return await _CityRepository.GetAllPagedAsync(objGrid, query);

        }

        public virtual async Task<City> GetByIdAsync(int Id)
        {
            return await _CityRepository.GetByIdAsync(Id);
        }

        public virtual async Task<IList<City>> GetByIdsAsync(IList<int> ids)
        {
            return await _CityRepository.GetByIdsAsync(ids);
        }

        public virtual async Task InsertAsync(City City, int CityId, string Cityname)
        {
            City.CreatedDate = System.DateTime.Now;
            await _CityRepository.InsertAsync(City);
        }

        public virtual async Task<bool> IsNameExist(string CityName, int Id)
        {
            var result = await _CityRepository.GetAllAsync(query =>
            {
                return query.Where(x => x.CityName == CityName && x.Id != Id);
            });
            return result.Count > 0;
        }

        public virtual async Task UpdateAsync(City City, int CityId, string Cityname)
        {
            City.UpdatedDate = System.DateTime.Now;
            await _CityRepository.UpdateAsync(City);
        }

        public virtual async Task UpdateAsync(IList<City> CityList, int CityId, string Cityname)
        {
            if (CityList.Count == 0)
                throw new ArgumentNullException(nameof(CityList));

            await _CityRepository.UpdateAsync(CityList);
        }
    }
}

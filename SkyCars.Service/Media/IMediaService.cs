using SkyCars.Core;
using SkyCars.Core.DomainEntity.CarMedia;
using SkyCars.Core.DomainEntity.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCars.Service.Media
{
    public partial interface IMediaService
    {

        #region Methods
        Task<IPagedList<CarMedia>> GetAllAsync(GridRequestModel objGrid);
        Task<IList<CarMedia>> GetByIdsAsync(IList<int> ids);
        Task<CarMedia> GetByIdAsync(int Id);
        Task InsertAsync(CarMedia CarMedia, int MediaId, string Medianame);
        Task UpdateAsync(CarMedia CarMedia, int MediaId, string Medianame);
        Task UpdateAsync(IList<CarMedia> CarMediaList, int MediaId, string Medianame);
        Task<bool> IsNameExist(string MediaName, int Id);
        #endregion
    }
}

using SkyCars.Core;
using SkyCars.Core.DomainEntity.CarMedia;
using SkyCars.Core.DomainEntity.Grid;
using SkyCars.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCars.Service.Media
{
    public partial class MediaService : IMediaService
    {

        private readonly IRepository<CarMedia> _CarMediaRepository;

       

        public MediaService(IRepository<CarMedia> CarMediaRepository)
        {
            _CarMediaRepository = CarMediaRepository;
        }
        public virtual async Task<IPagedList<CarMedia>> GetAllAsync(GridRequestModel objGrid)
        {

            var query = from d in _CarMediaRepository.Table
                        where !d.IsDelete
                        select new CarMedia()
                        {
                            Id = d.Id,
                            UserId = d.UserId,
                            MediaName = d.MediaName,
                            MediaType = d.MediaType,
                            MediaDescription = d.MediaDescription


                        };

            return await _CarMediaRepository.GetAllPagedAsync(objGrid, query);
        }

        public virtual async Task<CarMedia> GetByIdAsync(int Id)
        {
            return await _CarMediaRepository.GetByIdAsync(Id);
        }

        public virtual async Task<IList<CarMedia>> GetByIdsAsync(IList<int> ids)
        {
            return await _CarMediaRepository.GetByIdsAsync(ids);
        }

        public virtual async Task InsertAsync(CarMedia CarMedia, int MediaId, string Medianame)
        {

            //CarMedia.CreatedDate = System.DateTime.Now;
            await _CarMediaRepository.InsertAsync(CarMedia);
        }

        public virtual async Task<bool> IsNameExist(string MediaName, int Id)
        {
            var result = await _CarMediaRepository.GetAllAsync(query =>
            {
                return query.Where(x => x.MediaName == MediaName && x.Id != Id);
            });
            return result.Count > 0;
        }

        public virtual async Task UpdateAsync(CarMedia CarMedia, int MediaId, string Medianame)
        {
            //CarMedia.UpdatedDate = System.DateTime.Now;
            await _CarMediaRepository.UpdateAsync(CarMedia); ;
        }

        public virtual async Task UpdateAsync(IList<CarMedia> CarMediaList, int MediaId, string Medianame)
        {

            if (CarMediaList.Count == 0)
                throw new ArgumentNullException(nameof(CarMediaList));

            await _CarMediaRepository.UpdateAsync(CarMediaList);
        }
    }
}

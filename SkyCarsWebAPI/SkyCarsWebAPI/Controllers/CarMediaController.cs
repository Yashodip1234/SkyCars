using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkyCars.Core.DomainEntity.CarMedia;
using SkyCars.Core.DomainEntity.Grid;
using SkyCars.Service.Media;
using SkyCarsWebAPI.Extensions;
using SkyCarsWebAPI.Models;
using SkyCarsWebAPI.Models.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkyCarsWebAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class CarMediaController : BaseController
    {
        private readonly IMediaService _MediaService;

        public CarMediaController(IMediaService MediaService)
        {
            _MediaService = MediaService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ApiResponse> FiltersData(GridRequestModel objGrid)
        {
            var DocList = await _MediaService.GetAllAsync(objGrid);
            return ApiResponseHelper.GenerateResponse(ApiStatusCode.Status200OK, "Media", DocList.ToGrid());
        }


        [HttpGet("{id?}")]
        public async Task<ApiResponse> Get(int id)
        {
            if (id == 0)
                return ApiResponseHelper.GenerateResponse(ApiStatusCode.Status400BadRequest, "No Data Found");

            var data = await _MediaService.GetByIdAsync(id);
            return ApiResponseHelper.GenerateResponse(data.GetApiResponseStatusCodeByData(), data is null ? "NoDataFound" : string.Format("Media"), data);
        }
        [HttpPost]
        public async Task<ApiResponse> Post(CarMediaModel model)
        {
            if (await _MediaService.IsNameExist(model.MediaName, model.Id))
                return ApiResponseHelper.GenerateResponse(ApiStatusCode.Status400BadRequest, "Medianame already exists.");

            if (model.Id == 0)
            {
                await _MediaService.InsertAsync(model.MapTo<CarMedia>(), CurrentUserId, CurrentUserName);
            }
            else
            {
                await _MediaService.UpdateAsync(model.MapTo<CarMedia>(), CurrentUserId, CurrentUserName);
            }
            return ApiResponseHelper.GenerateResponse(ApiStatusCode.Status200OK, model.Id == 0 ? "Media saved successfully." : "Media updated successfully.");
        }

        [HttpDelete]
        public async Task<ApiResponse> Delete(IList<int> Ids)
        {
            if (Ids.Count == 0)
                return ApiResponseHelper.GenerateResponse(ApiStatusCode.Status422UnprocessableEntity, "Invalid Request Parmeters");

            var obj = await _MediaService.GetByIdsAsync(Ids).ConfigureAwait(false);
            if (obj == null)
                return ApiResponseHelper.GenerateResponse(ApiStatusCode.Status422UnprocessableEntity, "No Data Found");

            obj.ToList().ForEach(s => s.IsDelete = true);
            await _MediaService.UpdateAsync(obj, CurrentUserId, CurrentUserName);
            return ApiResponseHelper.GenerateResponse(ApiStatusCode.Status200OK, "Media daleted successfully.");

        }

    }
}

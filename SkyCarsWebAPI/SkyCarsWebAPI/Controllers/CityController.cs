using Google.Apis.Admin.Directory.directory_v1.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using SkyCars.Core.DomainEntity.City;
using SkyCars.Core.DomainEntity.Grid;
using SkyCars.Service.Cities;
using SkyCarsWebAPI.Extensions;
using SkyCarsWebAPI.Models.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkyCarsWebAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class CityController : BaseController
    { 
        private readonly ICityServices _CityServices;

        public CityController(ICityServices CityServices)
        {
            _CityServices=CityServices;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ApiResponse> FiltersData(GridRequestModel objGrid)
        {
            var DocList = await _CityServices.GetAllAsync(objGrid);
            return ApiResponseHelper.GenerateResponse(ApiStatusCode.Status200OK, "City", DocList.ToGrid());
        }


        [HttpGet("{id?}")]
        public async Task<ApiResponse> Get(int id)
        {
            if (id == 0)
                return ApiResponseHelper.GenerateResponse(ApiStatusCode.Status400BadRequest, "No Data Found");

            var data = await _CityServices.GetByIdAsync(id);
            return ApiResponseHelper.GenerateResponse(data.GetApiResponseStatusCodeByData(), data is null ? "NoDataFound" : string.Format("City"), data);
        }

        [HttpPost]
        public async Task<ApiResponse> Post(CityModel model)
        {
            if (await _CityServices.IsNameExist(model.CityName, model.Id))
                return ApiResponseHelper.GenerateResponse(ApiStatusCode.Status400BadRequest, "Cityname already exists.");

            if (model.Id == 0)
            {
                await _CityServices.InsertAsync(model.MapTo<City>(), CurrentUserId, CurrentUserName);
            }
            else
            {
                await _CityServices.UpdateAsync(model.MapTo<City>(), CurrentUserId, CurrentUserName);
            }
            return ApiResponseHelper.GenerateResponse(ApiStatusCode.Status200OK, model.Id == 0 ? "City saved successfully." : "City updated successfully.");
        }


        [HttpDelete]
        public async Task<ApiResponse> Delete(IList<int> Ids)
        {
            if (Ids.Count == 0)
                return ApiResponseHelper.GenerateResponse(ApiStatusCode.Status422UnprocessableEntity, "Invalid Request Parmeters");

            var obj = await _CityServices.GetByIdsAsync(Ids).ConfigureAwait(false);
            if (obj == null)
                return ApiResponseHelper.GenerateResponse(ApiStatusCode.Status422UnprocessableEntity, "No Data Found");

            obj.ToList().ForEach(s => s.IsDelete = true);
            await _CityServices.UpdateAsync(obj, CurrentUserId, CurrentUserName);
            return ApiResponseHelper.GenerateResponse(ApiStatusCode.Status200OK, "City daleted successfully.");

        }

    }
}

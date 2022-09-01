using AutoMapper;
using System.Linq;

using SkyCars.Core.DomainEntity.User;
using SkyCarsWebAPI.Models;
using SkyCarsWebAPI.Models.Common;
using SkyCars.Core.DomainEntity.City;
using SkyCars.Core.DomainEntity.CarMedia;

namespace SkyCarsWebAPI.Infrastructure
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {

            //#region :: Settings ::           
            //CreateMap<SystemMessageModel, SystemMessage>().ReverseMap().IgnoreAllPropertiesWithAnInaccessibleSetter();
            //CreateMap<SystemSettingModel, Settings>().ReverseMap().IgnoreAllPropertiesWithAnInaccessibleSetter();
            //#endregion

            #region :: Settings ::      
            CreateMap<UserModel, User>().ReverseMap().IgnoreAllPropertiesWithAnInaccessibleSetter();
            CreateMap<CityModel, City>().ReverseMap().IgnoreAllPropertiesWithAnInaccessibleSetter();
            CreateMap<CarMediaModel, CarMedia>().ReverseMap().IgnoreAllPropertiesWithAnInaccessibleSetter();
            #endregion
        }
    }
}

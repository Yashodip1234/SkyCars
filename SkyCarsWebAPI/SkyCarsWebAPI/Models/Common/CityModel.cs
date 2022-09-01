namespace SkyCarsWebAPI.Models.Common
{
    public class CityModel: BaseModel
    {
        public string CityName { get; set; }
       
        public bool IsDelete { get; set; }
    }
}

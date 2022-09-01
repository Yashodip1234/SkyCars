namespace SkyCarsWebAPI.Models
{
    public class CarMediaModel:BaseModel
    {
        public int UserId { get; set; }
        public string MediaName { get; set; }
        public string MediaType { get; set; }
        public string MediaDescription { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDelete { get; set; }
    }
}

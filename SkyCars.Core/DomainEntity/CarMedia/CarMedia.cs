using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCars.Core.DomainEntity.CarMedia
{
    public class CarMedia: BaseEntity
    {
        public int UserId { get; set; }
        public string MediaName { get; set; }
        public string MediaType { get; set; }
        public string MediaDescription { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDelete { get; set; }


    }
}

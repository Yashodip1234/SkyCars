using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SkyCarsWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public ClaimsPrincipal CurrentUser
        {
            get
            {
                return HttpContext.User;
            }
        }

        public int CurrentUserId
        {
            get
            {
                return 1;
                //if (CurrentUser != null && CurrentUser.HasClaim(c => c.Type == "UserId"))
                //{
                //    return int.Parse(CurrentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
                //}
                //else
                //    return 0;
            }
        }

        public int CurrentRoleId
        {
            get
            {
                return 1;
                //if (CurrentUser != null && CurrentUser.HasClaim(c => c.Type == "Role"))
                //{
                //    return int.Parse(CurrentUser.Claims.FirstOrDefault(c => c.Type == "Role").Value);
                //}
                //else
                //    return 0;
            }
        }
        public string CurrentUserName { get { return "Admin"; } }
    }

}

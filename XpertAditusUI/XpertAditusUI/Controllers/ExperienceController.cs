using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XpertAditusUI.Models;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExperienceController : BaseController
    {
        private readonly UserProfileService _userProfileService;

        public ExperienceController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, 
            UserProfileService userProfileService) : base(logger, userManager)
        {
            _userProfileService = userProfileService;
        }
        [HttpGet]
        public async Task<ActionResult> get()
        {
           var userid = this._userManager.GetUserId(this.User);
            var userInfo = _userProfileService.GetUserExperienceInfo(userid);
            return new JsonResult(userInfo);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Experience[] experience)
        {
            var userid = this._userManager.GetUserId(this.User);
            _userProfileService.SaveUserExperienceInfo(experience, userid);
            return new JsonResult("Success");
        }

        [HttpDelete]
        public ActionResult Delete([FromBody] Experience[] experience)
        {
            if (experience[0].ExperienceId != Guid.Empty)
            {
                _userProfileService.DeleteExperienceInfo(experience[0]);
                return new JsonResult("Success");
            }
            return new JsonResult("Fail");
        }
    }
}

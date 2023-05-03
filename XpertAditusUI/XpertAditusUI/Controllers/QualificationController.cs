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
    public class QualificationController : BaseController
    {
        private readonly UserProfileService _userProfileService;

        public QualificationController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
           UserProfileService userProfileService) : base(logger, userManager)
        {
            _userProfileService = userProfileService;
        }
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var userid = this._userManager.GetUserId(this.User);
            var userInfo = _userProfileService.GetUserQualificationInfo(userid);
            return new JsonResult(userInfo);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Qualification[] qualification)
        {
            var userid = this._userManager.GetUserId(this.User);
            _userProfileService.SaveUserQualificationInfo(qualification, userid);
            return new JsonResult("Success");
        }

        [HttpDelete]
        public ActionResult Delete([FromBody] Qualification[] qualification)
        {
            if (qualification[0].QualificaitonId != Guid.Empty)
            {
                _userProfileService.DeleteQualificationInfo(qualification[0].QualificaitonId.ToString());
                return new JsonResult("Success");
            }
            return new JsonResult("Fail");
        }
    }
}

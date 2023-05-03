using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XpertAditusUI.Data;
using XpertAditusUI.Models;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserProfileController : BaseController
    {
        private readonly UserProfileService _userProfileService;
        private readonly XpertAditusDbContext _context;

        public UserProfileController(ILogger<HomeController> logger, XpertAditusDbContext context, UserManager<IdentityUser> userManager,
           UserProfileService userProfileService) : base(logger, userManager)
        {
            _userProfileService = userProfileService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userid = this._userManager.GetUserId(this.User);
            UserProfile userProfile = _userProfileService.GetUserInfo(userid);
            GetEducation();
            return View(userProfile);
        }

        [HttpPost]
        public async Task<ActionResult> Profile([FromForm] UserProfile userProfile)
        {
            var userid = this._userManager.GetUserId(this.User);
            _userProfileService.SaveUserProfileAsync(userProfile, userid);
            GetEducation();
            return View(userProfile);
        }

        [HttpGet("DeleteResume")]

        public async Task<IActionResult> DeleteResume()
        {
            var userid = this._userManager.GetUserId(this.User);
            _userProfileService.deleteResume(userid);
            return RedirectToAction("Profile");
        }
        [NonAction]
        public void GetEducation()
        {
            ViewBag.Education = _context.EducationMaster.ToList();
        }
    }
}

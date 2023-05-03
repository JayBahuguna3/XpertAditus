using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using XpertAditusUI.Data;

namespace XpertAditusUI.Controllers
{
    [Authorize]
    public class AdmissionInfoDashboardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        UserManager<IdentityUser> _userManager;
        private readonly XpertAditusDbContext _context;

        public AdmissionInfoDashboardController(UserManager<IdentityUser> userManager, XpertAditusDbContext context)
        {
            _userManager = userManager;
            _context = context;

        }

        public IActionResult AdmissionInfoDashboard()
        {
            try
            {
                var userid = this._userManager.GetUserId(this.User);
                var userProfile = this._context.UserProfile.Where(u => u.LoginId == userid).FirstOrDefault();
                if (userProfile != null)
                {
                    var admissionInfo = _context.Admission.Where(e => e.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
                    var colgProfile = _context.CollegeProfile.Where(c => c.CollegeProfileId == admissionInfo.CollegeProfileId).FirstOrDefault();
                    var courseInfo = _context.CourseMaster.Where(n => n.CourseId == admissionInfo.CourseId).FirstOrDefault();
                    
                    ViewBag.CollegeName = colgProfile != null ? colgProfile.Name : "";
                    ViewBag.Fee = admissionInfo.FeeAmount == null ? 0 : admissionInfo.FeeAmount;
                    ViewBag.CourseName = courseInfo.Name;
                    ViewBag.ColgEmail = colgProfile != null ?  colgProfile.CollegeEmail : "";


                    ViewData["UserName"] = userProfile.FirstName + " " + userProfile.LastName;
                    ViewData["Photo"] = userProfile.PhotoPath;
                }
                else
                {
                    ViewData["UserName"] = "";
                    ViewData["Photo"] = "";
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }

    }
}

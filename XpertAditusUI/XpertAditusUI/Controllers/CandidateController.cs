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
    
    public class CandidateController : BaseController
    {
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly ILogger<HomeController> _logger;
        protected readonly CandidateService _candidateService;
        public CandidateController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
            CandidateService candidateService)
            : base (logger,userManager)
        {
            _logger = logger;
            _userManager = userManager;
            _candidateService = candidateService;
           
        }
        [NonAction]
        public void InitializeViewBag()
        {
            if (this.User != null)
            {
                ViewBag.UserProfile = _candidateService.GetUserProfile(this.User);
                ViewBag.ActiveUserCourses = _candidateService.GetActiveUserCourses(ViewBag.UserProfile);
                ViewBag.ActiveUserCourse = _candidateService.GetActiveUserCourse(ViewBag.ActiveUserCourses);
                if (ViewBag.ActiveUserCourses != null)
                {
                    ViewBag.IsTrainingComplete = _candidateService.IsTrainingComplete(ViewBag.ActiveUserCourses);
                }
                else
                {
                    ViewBag.IsTrainingComplete = false;
                }
            }
        }
    }
}

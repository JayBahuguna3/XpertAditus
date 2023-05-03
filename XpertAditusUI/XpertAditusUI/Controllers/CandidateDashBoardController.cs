//using AutoMapper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace XpertAditusUI.Controllers
{
    [Route("[controller]")]
    public class CandidateDashBoardController : BaseController
    {
        private readonly XpertAditusDbContext _db;
        private IConfiguration _configuration;
        public CandidateDashBoardController(XpertAditusDbContext db, IConfiguration configurationManager,
         ILogger<HomeController> logger, UserManager<IdentityUser> userManager)
         : base(logger, userManager)
        {
            _db = db;
            _configuration = configurationManager;
        }
        [HttpGet]
        public IActionResult CandidateDashBoard(int valuetake = 10, int valueskip = 0)
        {
            var empProfile = this._userManager.GetUserProfile(this.User, _db);
            List<CandidateDashBoard> candidateDashBoards = new List<CandidateDashBoard>();
            List<UserProfile> userProfiles = (from u in _db.UserProfile 
                                   join c in _db.UserCourses on u.UserProfileId equals c.UserProfileId
                                   join r in _db.CandidateResult on c.UserCoursesId equals r.UserCoursesId
                                   where
                                   u.UserProfileType == "Candidate" &&
                                   r.IsCleared == true
                                   select u
                                  ).Skip(valueskip).Take(valuetake).ToList();

            var districts = _db.DistrictMaster.ToList();
            ViewData["Districts"] = districts.Select(e => new SelectListItem(e.Name, e.DistrictId.ToString())).ToList();

            var jobDesignations = _db.JobMaster.Where(e => e.UserProfileId == empProfile.UserProfileId).ToList();

            ViewData["JobDesignations"] = jobDesignations.Select(e => new SelectListItem(e.JobDesignation, e.JobDesignation)).ToList();
            foreach (UserProfile userProfile in userProfiles)
            {
                try 
                {
                    CandidateDashBoard candidateDashBoard = new CandidateDashBoard();
                    candidateDashBoard.userProfileId = userProfile.UserProfileId.ToString();
                    candidateDashBoard.CandidateName = userProfile.FirstName + " " + userProfile.LastName;
                    candidateDashBoard.photoPath = userProfile.PhotoPath;

                    Address address = _db.Address.Where(r => r.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
                    if(address != null)
                    {
                        DistrictMaster districtMaster = _db.DistrictMaster.Where(r => r.DistrictId == address.DistrictId).FirstOrDefault();
                        if(districtMaster != null)
                            candidateDashBoard.CityName = districtMaster.Name;
                        else
                            candidateDashBoard.CityName = "";
                    }
                    else
                        candidateDashBoard.CityName = "";

                    UserCourses userCourses = _db.UserCourses.Where(r => r.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
                    if (userCourses != null)
                    {
                        CandidateResult candidateResult = _db.CandidateResult.Where(r => r.UserCoursesId == userCourses.UserCoursesId).FirstOrDefault();
                        
                        if (candidateResult != null)
                            candidateDashBoard.scrore = candidateResult.Score.ToString();
                        else
                            candidateDashBoard.scrore = "";

                    }
                    else
                        candidateDashBoard.scrore = "";

                    if (userCourses != null)
                    {
                        Course course = _db.Course.Where(r => r.CourseId == userCourses.CourseId).FirstOrDefault();
                        candidateDashBoard.courseName = course.Name;
                    }
                    else
                    {
                        candidateDashBoard.courseName = "";
                    }

                    candidateDashBoards.Add(candidateDashBoard);
                }
                catch (Exception ex)
                {

                }
            }

            return View(candidateDashBoards);
        }
    }
}

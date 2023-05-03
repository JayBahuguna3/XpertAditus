using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XpertAditusUI.Data;
using XpertAditusUI.DTO;
using XpertAditusUI.Models;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CandidateTrainingContentController : BaseController
    {
        private readonly XpertAditusDbContext _context;
        private readonly IMapper _mapper;
        private readonly TestService _testService;

        public CandidateTrainingContentController(XpertAditusDbContext context, TestService testService,
            ILogger<HomeController> logger, UserManager<IdentityUser> userManager, IMapper mapper)
            : base(logger, userManager)
        {
            _context = context;
            _testService = testService;
            _mapper = mapper;
        }
        [HttpGet("CandidateTrainingContent/{courseid}/{trainingcontentid}")]
        public async Task<IActionResult> CandidateTrainingContent(string courseid, string trainingcontentid)
        {
            var userId = this._userManager.GetUserId(this.User);
            var userProfile = this._testService.GetUserProfile(userId);
            var activeCourse = _context.Course.Where(e => e.CourseId.ToString() == courseid).FirstOrDefault();
            var userCourses = _context.UserCourses.Where(e => e.CourseId.ToString() == courseid
                && e.UserProfileId == userProfile.UserProfileId && e.IsActive == true).FirstOrDefault();

            var CandidateResult = _context.CandidateResult.Where(c => c.UserCoursesId == userCourses.UserCoursesId).FirstOrDefault();

            bool checkUserTestPassedOrTestAttempt = false;

            if (CandidateResult != null)
            {
                if ((bool)CandidateResult.IsCleared)
                {
                    checkUserTestPassedOrTestAttempt = true;
                }

                if (CandidateResult.TestAttempt == 3 && (bool)CandidateResult.IsCompleted)
                {
                    checkUserTestPassedOrTestAttempt = true;
                }
            }

            ViewBag.checkUserTestPassedOrTestAttempt = checkUserTestPassedOrTestAttempt;

            ViewBag.ActiveCourse = activeCourse;
            if (activeCourse != null)
            {

                var trainingContentList = this._context.TrainingContentsMaster
                        .Include(e => e.CandidateTrainingStatus.Where(e => e.UserCoursesId == userCourses.UserCoursesId))
                        .Where(e => e.CourseId.ToString() == courseid && e.IsActive == "True").ToList().OrderBy(e => e.CreatedDate);
                ViewBag.Course = activeCourse;
                ViewBag.CurrectContent = trainingContentList.FirstOrDefault();
                if(string.IsNullOrWhiteSpace(trainingcontentid))
                     ViewBag.CurrectContent = trainingContentList.FirstOrDefault();
                else
                {
                    ViewBag.CurrectContent = trainingContentList.Where(e => e.TrainingContentId.ToString() == trainingcontentid).FirstOrDefault();

                }
                return View(trainingContentList);
            }
            else
            {
                return View();

            }
        }
        [HttpGet("CandidateTrainingContent/{courseid}")]
        public async Task<IActionResult> CandidateTrainingContent(string courseid)
        {
            try
            {
                var userId = this._userManager.GetUserId(this.User);
                var userProfile = this._testService.GetUserProfile(userId);
                var activeCourse = _context.Course.Where(e => e.CourseId.ToString() == courseid).FirstOrDefault();
                var userCourses = _context.UserCourses.Where(e => e.CourseId.ToString() == courseid
                && e.UserProfileId == userProfile.UserProfileId && e.IsActive == true).FirstOrDefault();

                var CandidateResult = _context.CandidateResult.Where(c => c.UserCoursesId == userCourses.UserCoursesId).FirstOrDefault();

                bool checkUserTestPassedOrTestAttempt = false;
               
                if (CandidateResult != null)
                {
                    if((bool)CandidateResult.IsCleared)
                    {
                        checkUserTestPassedOrTestAttempt = true;
                    }

                    if(CandidateResult.TestAttempt == 3 && (bool)CandidateResult.IsCompleted)
                    {
                        checkUserTestPassedOrTestAttempt = true;
                    }
                }

                ViewBag.checkUserTestPassedOrTestAttempt = checkUserTestPassedOrTestAttempt;

                ViewBag.ActiveCourse = activeCourse;
                if (activeCourse != null)
                {
                    var trainingContentList = this._context.TrainingContentsMaster
                        .Include(e => e.CandidateTrainingStatus.Where(e => e.UserCoursesId == userCourses.UserCoursesId))
                        .Where(e => e.CourseId.ToString() == courseid && e.IsActive == "True").ToList().OrderBy(e => e.CreatedDate);
                    ViewBag.Course = activeCourse;
                    ViewBag.CurrectContent = trainingContentList.FirstOrDefault();
                    return View(trainingContentList);
                }
                else
                {
                    return View();

                }
            }
            catch(Exception ex)
            {
                ViewBag.ActiveCourse = null;
                return View();
            }
        }
        [HttpGet("MarkCandidateTrainingComplete/{courseid}/{trainingcontentid}")]
        public async Task<IActionResult> MarkCandidateTrainingComplete(string courseid, string trainingContentId)
        {
            try
            {
                var userId = this._userManager.GetUserId(this.User);
                var userProfile = this._testService.GetUserProfile(userId);
                //var activeCourse = _testService.GetActiveUserCourse(userProfile.UserProfileId);
                var userCourse = _context.UserCourses.Where(e => e.UserProfileId == userProfile.UserProfileId
                && e.IsActive == true && e.CourseId.ToString() == courseid).FirstOrDefault();
                var trainingContentList = this._context.TrainingContentsMaster
                    .Include(e => e.CandidateTrainingStatus.Where(e => e.UserCoursesId.ToString() == courseid))
                    .Where(e => e.CourseId.ToString() == courseid && e.IsActive == "True").ToList();
                var cList = _context.CandidateTrainingStatus.
                    Where(e => e.UserCoursesId == userCourse.UserCoursesId
                    && e.TrainingContentId.ToString() == trainingContentId).ToList();
                var alreadyExistCheck = cList.Count() > 0;
                if (!alreadyExistCheck)
                {
                    CandidateTrainingStatus candidateTrainingStatus = new CandidateTrainingStatus()
                    {
                        CandidateTrainingStatusId = Guid.NewGuid(),
                        CompletedDate = DateTime.Now,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now,
                        DownloadedDate = DateTime.Now,
                        TrainingContentId = new Guid(trainingContentId),
                        UserCoursesId = userCourse.UserCoursesId
                    };
                    _context.CandidateTrainingStatus.Add(candidateTrainingStatus);
                    _context.SaveChanges();
                }
                return Json(new ResponseResult() { Error = false, Message = "Marked complete successfully." });
            }
            catch(Exception ex)
            {
                return Json(new ResponseResult() { Error = true, Message = "Unable to mark complete. Try again." });

            }

        }


    }
}
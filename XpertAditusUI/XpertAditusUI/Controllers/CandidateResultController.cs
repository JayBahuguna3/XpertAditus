using AutoMapper.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.DTO;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    [Route("[controller]")]
    public class CandidateResultController : CandidateController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly XpertAditusDbContext _context;
        private readonly TestService _testService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private Microsoft.Extensions.Configuration.IConfiguration _configuration;


        public CandidateResultController(XpertAditusDbContext context,
            ILogger<HomeController> logger, UserManager<IdentityUser> userManager, IWebHostEnvironment environment,
            Microsoft.Extensions.Configuration.IConfiguration configuration, TestService testService, CandidateService candidateService) :
            base(logger, userManager, candidateService)

        {
            _testService = testService;
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _hostEnvironment = environment;
        }
        public string selectedCourseId;
        [HttpGet("CandidateResult")]
        public ActionResult CandidateResult()
        {
            var userProfile = this._userManager.GetUserProfile(this.User, this._context);
            ViewBag.UserCourse = this._context.UserCourses.Where(e => e.UserProfileId == userProfile.UserProfileId &&
                                   e.IsActive == true).FirstOrDefault();

            var userCourse = this._context.UserCourses.Where(e => e.UserProfileId == userProfile.UserProfileId &&
                           e.IsActive == true).FirstOrDefault();
            var PassingCriteria = _configuration["AppSettings:PassingCriteria"].ToString();
            if(userCourse != null)
            {
                var candidateResultList = this._context.CandidateResult.Where(e => e.UserCoursesId == userCourse.UserCoursesId).OrderByDescending(e => e.CreatedDate).ToList();
                //if (candidateResult.IsCleared == true)
                var IsPassed = candidateResultList.Where(e => e.IsCleared.Value == true).Count() > 0;
                ViewBag.IsPassed = IsPassed;
                ViewBag.CandidateResultList = candidateResultList;

                //if (candidateResult.Score <= decimal.Parse(PassingCriteria) || candidateResult.Score == null)
                //{
                //    if (candidateResult.TestAttempt == 0)
                //    {
                //        ViewBag.noActiveTest = candidateResult;
                //    }
                //    else
                //    {
                //        ViewBag.candidateResultStatus = candidateResult;
                //    }
                //}
                //else
                //{
                //    ViewBag.candidateResultStatus = candidateResult;
                //}
            }

            //ViewBag.UserCourse = this._context.UserCourses.Include(e => e.Course).
            //    Where(e => e.UserProfileId == userProfile.UserProfileId).ToList();
            //ViewBag.UserCourseDropdown = this._context.UserCourses.Include(e => e.Course).
            //    Where(e => e.UserProfileId == userProfile.UserProfileId).Select( e => new SelectListItem() { Text = e.Course.Name, Value=e.UserCoursesId.ToString() });


            return View();
        }

        [HttpGet("GetTestScore")]
        public async Task<IActionResult> GetTestScore()
        {
            var userProfile = this._userManager.GetUserProfile(this.User, this._context);
            var userCourse = this._context.UserCourses.Where(e => e.UserProfileId == userProfile.UserProfileId &&
                             e.IsActive == true).FirstOrDefault();
            //var userCourse = this._context.UserCourses.Where(e => e.UserCoursesId.ToString() == usercourseid).FirstOrDefault();
            ViewBag.UserCourse = userCourse;
            if (userCourse != null)
            {
                var candidateResult = this._context.CandidateResult.Where(e => e.UserCoursesId == userCourse.UserCoursesId && e.IsCleared == true).ToList();
                //if(candidateResult != null && candidateResult.IsCleared == true)
                //{
                  //  ViewBag.candidateResultStatus = candidateResult;
               // }
                //var candidateResult = this._context.CandidateResult.Include(e => e.UserCourses)
                //    .Where(e => e.UserCoursesId == userCourse.UserCoursesId && e.IsCompleted == true)
                //    .OrderByDescending(e => e.CreatedDate).FirstOrDefault();
                if (candidateResult.Count > 0)
                {
                    //var cResult = _context.CandidateResult.Include(e => e.UserCourses)
                    //    .Where(e => e.CandidateResultId.ToString() == candidateResult.CandidateResultId.ToString())
                    //    .FirstOrDefault();
                    // var PassingCriteria = _configuration["AppSettings:PassingCriteria"].ToString();
                    //var Score = _testService.GetPreAdmissionScore(candidateResult.CandidateResultId,
                    //    candidateResult.UserCourses.CourseId, PassingCriteria);
                    return Json(candidateResult);
                }
                else
                {
                    return Json("No Candidate Result");
                }
            }
            else
            {
                return Json("No Active Course");
            }
        }

        [HttpGet("DownloadCertificate/{usercourseid}")]
        public IActionResult DownloadCertificate(string usercourseid)
        {
            try
            {
                var userProfile = this._userManager.GetUserProfile(this.User, this._context);
                var userCourse = this._context.UserCourses.Include(e => e.Course).Where(e => e.UserCoursesId.ToString() == usercourseid&&
                e.IsActive == true).FirstOrDefault();
                if (userCourse != null)
                {

                    var candidateResult = this._context.CandidateResult.Where(e => e.UserCoursesId == userCourse.UserCoursesId
                    && e.IsActive == true && e.IsCompleted == true).FirstOrDefault();
                    if (candidateResult != null)
                    {
                        string filepath = Path.Combine(_hostEnvironment.ContentRootPath, "DocFormat\\CERTIFICATE.docx");
                        CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                        TextInfo textInfo = cultureInfo.TextInfo;
                        Document doc = new Document(filepath, FileFormat.Auto);
                        doc.Replace(@"[NAME]", userProfile.FirstName + " " + userProfile.MiddleName + " " + userProfile.LastName, true, true);
                        doc.Replace(@"[COURSE]", userCourse.Course.Name, true, true);
                        doc.Replace(@"[DATE]", ((DateTime)candidateResult.TestEnd).ToShortDateString(), true, true);


                        string savepath = Path.Combine(_hostEnvironment.ContentRootPath, "DocFormat\\" + userProfile.FirstName + "_" + DateTime.Now.ToString("dd-MMM-yyyy-hh-mm-ss") + "_CERTIFICATE.pdf");
                        doc.SaveToFile(savepath, FileFormat.PDF);
                        var stream = new FileStream(savepath, FileMode.Open);
                        return new FileStreamResult(stream, "application/pdf");
                    }
                    else
                    {
                        return Json("No Candidate Result");
                    }
                }
                else
                {
                    return Json("No Active Course");
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }        
      

    }


}


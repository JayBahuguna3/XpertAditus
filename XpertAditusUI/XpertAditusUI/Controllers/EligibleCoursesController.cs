using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.DTO;
using XpertAditusUI.Models;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    [Route("[controller]")]
    public class EligibleCoursesController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly XpertAditusDbContext _context;
        static List<CourseMaster> totalCoursesList;
        static List<CourseMaster> coursesList;
        static List<CourseMaster> newCoursesList = new List<CourseMaster>();
        static Guid CourseId;
        public EligibleCoursesController(XpertAditusDbContext db, UserProfileService userProfileService, TestService testService,
          ILogger<HomeController> logger, UserManager<IdentityUser> userManager, IMapper mapper)
          : base(logger, userManager)
        {
            _mapper = mapper;
            _context = db;
        }
        [HttpGet]
        public IActionResult EligibleCourses()
        {
            coursesList = new List<CourseMaster>();
            var userProfile = this._userManager.GetUserProfile(this.User, this._context);
            var userCourse = this._context.UserCourses.Where(e => e.UserProfileId == userProfile.UserProfileId && e.IsActive == true).FirstOrDefault();

            if (userCourse != null)
            {
                CandidateResult candidateResults = _context.CandidateResult
                        .Where(r => r.UserCoursesId == userCourse.UserCoursesId && r.IsCleared == true).FirstOrDefault();

                if (candidateResults == null)
                {
                    ViewBag.candidateResultStatus = null;
                    return View();
                }
                else
                {
                    ViewBag.candidateResultStatus = candidateResults;
                }
                Qualification qualification = _context.Qualification.Where(r => r.UserProfileId == userProfile.UserProfileId)
                    .FirstOrDefault();

                totalCoursesList = _context.CourseMaster.Where(e => e.IsActive == true).ToList();
                for (int i = 0; i < totalCoursesList.Count; i++)
                {
                    List<CollegeCourseSpecializationMapping> collegeCourseSpecializationMappings = _context.CollegeCourseSpecializationMapping.
                        Where(r => r.CourseId == totalCoursesList[i].CourseId && r.IsActive == true).ToList();

                    for (int j = 0; j < collegeCourseSpecializationMappings.Count; j++)
                    {
                        if (collegeCourseSpecializationMappings[j] != null)
                        {
                            if (collegeCourseSpecializationMappings[j].MinTestScore <= candidateResults.Score
                                && collegeCourseSpecializationMappings[j].MinAcademicPercentage <= qualification.Percentage)
                            {
                                if (!coursesList.Contains(totalCoursesList[i]))
                                    coursesList.Add(totalCoursesList[i]);
                            }
                        }
                    }
                }

                ViewBag.Courses = coursesList;

                return View();
            }
            else
            {
                ViewBag.candidateResultStatus = null;
                return View();
            }
        }

        [HttpPost]
        public IActionResult EligibleCourses(string Description, string Name, decimal Fee)
        {
            ViewBag.Courses = coursesList;

            var userProfile = this._userManager.GetUserProfile(this.User, this._context);
            var userCourse = this._context.UserCourses.Where(e => e.UserProfileId == userProfile.UserProfileId &&
                          e.IsActive == true).FirstOrDefault();
            List<CollegeProfile> collegeProfiles = new List<CollegeProfile>();
            CourseMaster course = _context.CourseMaster.Where(e => e.Name == Name).FirstOrDefault();
            List<CollegeCourseSpecializationMapping> collegeCourseSpecializationMappings
                = _context.CollegeCourseSpecializationMapping.Distinct().Where(e => e.CourseId == course.CourseId).ToList();
            CourseId = course.CourseId;
            CandidateResult candidateResults = _context.CandidateResult
                    .Where(r => r.UserCoursesId == userCourse.UserCoursesId && r.IsActive == true).FirstOrDefault();
            Qualification qualification = _context.Qualification.Where(r => r.UserProfileId == userProfile.UserProfileId)
            .FirstOrDefault();
            for (int i = 0; i < collegeCourseSpecializationMappings.Count; i++)
            {
                if (collegeCourseSpecializationMappings[i] != null)
                {
                    if (collegeCourseSpecializationMappings[i].MinTestScore <= candidateResults.Score
                        && collegeCourseSpecializationMappings[i].MinAcademicPercentage <= qualification.Percentage)
                    {
                        CollegeProfile collegeProfile =
                        _context.CollegeProfile.
                    Where(e => e.CollegeProfileId ==
                    collegeCourseSpecializationMappings[i].CollegeProfileId).FirstOrDefault();
                        if (!collegeProfiles.Contains(collegeProfile))
                        {
                            collegeProfiles.Add(collegeProfile);
                        }
                    }
                }
            }

            if (collegeProfiles.Count > 0)
            {
                ViewBag.SelectedCourse = true;
                ViewData["CollegeProfiles"] = collegeProfiles;
            }

            if (userCourse != null)
            {
                var candidateResult = this._context.CandidateResult.Where(e => e.UserCoursesId == userCourse.UserCoursesId && e.IsCleared == true).OrderByDescending(e => e.CreatedDate).FirstOrDefault();
                if (candidateResult.IsCleared == true)
                {
                    ViewBag.candidateResultStatus = candidateResult;
                }
            }

            return View();
        }

        [HttpGet("CollegeDetail/{id}")]
        public IActionResult CollegeDetail(string id)
        {
            var userId = this._userManager.GetUserId(this.User);
            CollegeProfile collegeProfile = _context.
                CollegeProfile.Where(r => r.CollegeProfileId == Guid.Parse(id)).FirstOrDefault();
            if (collegeProfile.DistrictId != null)
            {

                ViewBag.District = _context.DistrictMaster.Where(r => r.DistrictId ==
                Guid.Parse(collegeProfile.DistrictId)).FirstOrDefault().Name;
            }
            else
            {
                ViewBag.District = "";
            }
            if (collegeProfile.StateId != null)
            {

                ViewBag.State = _context.StateMaster.Where(r => r.StateId ==
                Guid.Parse(collegeProfile.StateId)).FirstOrDefault().Name;
            }
            else
            {
                ViewBag.State = "";
            }

            if (_context.CollegeStudentMapping.Where(r => r.CollegeProfileId == Guid.Parse(id)
              && r.LoginId == userId).FirstOrDefault() != null)
            {
                ViewBag.CollegeAppliedStatus = true;
            }
            return View(collegeProfile);
        }

        [HttpPost("AppliedCollege")]
        public IActionResult AppliedCollege()
        {
            var userId = this._userManager.GetUserId(this.User);
            string collegeProfileId = Request.Form["CollegeProfile_ID"].ToString();
            try
            {
                if (_context.CollegeStudentMapping.Where(r => r.CollegeProfileId == Guid.Parse(collegeProfileId)
             && r.LoginId == userId).FirstOrDefault() == null)
                {
                    CollegeStudentMapping collegeStudentMapping = new CollegeStudentMapping();
                    collegeStudentMapping.CollegeStudentMappingId = Guid.NewGuid();
                    collegeStudentMapping.CollegeProfileId = Guid.Parse(collegeProfileId);
                    collegeStudentMapping.LoginId = userId;
                    collegeStudentMapping.CreatedBy = userId;
                    collegeStudentMapping.CreatedDate = DateTime.Now;
                    collegeStudentMapping.CourseId = CourseId;
                    _context.Add(collegeStudentMapping);
                    _context.SaveChanges();
                    ViewBag.CollegeAppliedStatus = true;
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }
    }
}

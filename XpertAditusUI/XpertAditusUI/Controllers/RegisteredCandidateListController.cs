using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using System.Net.Http;
using XpertAditusUI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Dynamic;

namespace XpertAditusUI.Controllers
{
    public class RegisteredCandidateListController : BaseController
    {
        private readonly XpertAditusDbContext _context;
        private IActionResult ture;

        public RegisteredCandidateListController(XpertAditusDbContext context, ILogger<HomeController> logger, UserManager<IdentityUser> userManager) :  base(logger, userManager)
        {
            _context = context;
        }
        [HttpGet("RegisteredCandidateList")]
        [HttpPost("RegisteredCandidateList")]
        public IActionResult RegisteredCandidateList(Guid CollegeProfileId)
        {
            
            ViewBag.CollegeProfile = new SelectList(_context.CollegeProfile, "CollegeProfileId", "Name");
           
            if (Request.Method == "GET")
            {
                ViewBag.Alllist = _context.UserProfile.Where(c => c.UserProfileType == "Candidate").ToList();
            }
            else
            {
                var selectedId = Request.Form["dropdownid"];
                if (selectedId == "TestPassed")
                {
                    ViewBag.TestPassed = (from us in _context.UserProfile
                                       join uc in _context.UserCourses on us.UserProfileId equals uc.UserProfileId
                                       join cr in _context.CandidateResult on uc.UserCoursesId equals cr.UserCoursesId
                                       where cr.IsCleared == true && us.UserProfileType == "Candidate"
                                       select us
                                       ).ToList();

                }
                else if (selectedId == "EnrolledforTest")
                {
                    ViewBag.EnrolledforTest = (from us in _context.UserProfile
                                       join uc in _context.UserCourses on us.UserProfileId equals uc.UserProfileId
                                       join py in _context.PaymentHistory on uc.UserCoursesId equals py.UserCoursesId
                                       where us.UserProfileType == "Candidate"
                                       select us
                                      ).ToList();
                }

                else if (selectedId == "AllRegistered")
                {
                    ViewBag.Alllist = _context.UserProfile.Where(c => c.UserProfileType == "Candidate").ToList();
                }
                else if (selectedId == "Enrolledwithzerofee")
                {
                    ViewBag.Enrolledwithzerofee = (from us in _context.UserProfile
                                                   join uc in _context.UserCourses on us.UserProfileId equals uc.UserProfileId
                                                   join py in _context.PaymentHistory on uc.UserCoursesId equals py.UserCoursesId
                                                   where py.Amount == 0
                                                   where us.UserProfileType == "Candidate"
                                                   select us
                                                   ).ToList();
                }
                else if (selectedId == "AppliedforColleges")
                {

                    ViewBag.AppliedforColleges = (from us in _context.UserProfile
                                                  join cc in _context.CollegeStudentMapping on us.LoginId equals cc.LoginId
                                                  join col in _context.CollegeProfile on cc.CollegeProfileId equals col.CollegeProfileId
                                                  where us.UserProfileType == "Candidate"
                                                  select us)
                                                 .ToList();
                }
                else if (selectedId == "Appliedforspecificcollege")
                {
                    ViewBag.Appliedforspecificcollege = (from us in _context.UserProfile
                                                         join cc in _context.CollegeStudentMapping on us.LoginId equals cc.LoginId
                                                         join col in _context.CollegeProfile on cc.CollegeProfileId equals col.CollegeProfileId
                                                         where us.UserProfileType == "Candidate"
                                                         select us).ToList();
                }
                if(CollegeProfileId != Guid.Empty)
                {
                        ViewBag.CollegeName = (from us in _context.UserProfile
                                                               join cc in _context.CollegeStudentMapping on us.LoginId equals cc.LoginId 
                                                             where us.UserProfileType == "Candidate" &&  cc.CollegeProfileId == CollegeProfileId
                                                               select us).ToList();
                }
            }
        
            return View();
        }


        public async Task<IActionResult> Export_CandidateList()
        {

                var userProfile = _context.UserProfile.Where(c => c.UserProfileType == "Candidate").ToList();
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("UserProfileId,RegistrationNumber,FirstName,MiddleName,LastName,Email,MobileNumber,Dob,CreatedDate,UserProfileType,OfficeContactNumber,OfficeFaxNumber,BusinessType,CompanyName,ResumePath,PhotoPath,FacebookLink,LinkedinLink,TwitterLink,MaritalStatus,IsActive");
                foreach (var candilist in userProfile)
                {
                    stringBuilder.AppendLine(
                        $"{candilist.UserProfileId},{candilist.RegistrationNumber},{candilist.FirstName},{candilist.MiddleName},{candilist.LastName},{candilist.Email},{candilist.MobileNumber},{candilist.Dob},{candilist.CreatedDate},{candilist.UserProfileType},{candilist.OfficeContactNumber}" +
                        $"{candilist.OfficeFaxNumber},{candilist.BusinessType},{candilist.CompanyName},{candilist.ResumePath},{candilist.PhotoPath},{candilist.FacebookLink},{candilist.LinkedinLink},{candilist.TwitterLink},{candilist.MaritalStatus},{candilist.IsActive}"
                        );
                }


                return File(Encoding.UTF8.GetBytes(stringBuilder.ToString()), "text/csv", "RegisteredCandidateList_Report.csv");

        }
        public async Task<IActionResult> Export_TestPassedCandidateList()
        {

            var userProfile = (from us in _context.UserProfile
                               join uc in _context.UserCourses on us.UserProfileId equals uc.UserProfileId
                               join cr in _context.CandidateResult on uc.UserCoursesId equals cr.UserCoursesId
                               where cr.IsCleared == true && us.UserProfileType == "Candidate"
                               select us
                                       ).ToList();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("UserProfileId,RegistrationNumber,FirstName,MiddleName,LastName,Email,MobileNumber,Dob,CreatedDate,UserProfileType,OfficeContactNumber,OfficeFaxNumber,BusinessType,CompanyName,ResumePath,PhotoPath,FacebookLink,LinkedinLink,TwitterLink,MaritalStatus,IsActive");
            foreach (var candilist in userProfile)
            {
                stringBuilder.AppendLine(
                    $"{candilist.UserProfileId},{candilist.RegistrationNumber},{candilist.FirstName},{candilist.MiddleName},{candilist.LastName},{candilist.Email},{candilist.MobileNumber},{candilist.Dob},{candilist.CreatedDate},{candilist.UserProfileType},{candilist.OfficeContactNumber}" +
                    $"{candilist.OfficeFaxNumber},{candilist.BusinessType},{candilist.CompanyName},{candilist.ResumePath},{candilist.PhotoPath},{candilist.FacebookLink},{candilist.LinkedinLink},{candilist.TwitterLink},{candilist.MaritalStatus},{candilist.IsActive}"
                    );
            }


            return File(Encoding.UTF8.GetBytes(stringBuilder.ToString()), "text/csv", "RegisteredCandidateList_Report.csv");

        }
        public async Task<IActionResult> Export_EnrolledCandidateList()
        {

            var userProfile = (from us in _context.UserProfile
                               join uc in _context.UserCourses on us.UserProfileId equals uc.UserProfileId
                               join py in _context.PaymentHistory on uc.UserCoursesId equals py.UserCoursesId
                               where us.UserProfileType == "Candidate"
                               select us
                                      ).ToList();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("UserProfileId,RegistrationNumber,FirstName,MiddleName,LastName,Email,MobileNumber,Dob,CreatedDate,UserProfileType,OfficeContactNumber,OfficeFaxNumber,BusinessType,CompanyName,ResumePath,PhotoPath,FacebookLink,LinkedinLink,TwitterLink,MaritalStatus,IsActive");
            foreach (var candilist in userProfile)
            {
                stringBuilder.AppendLine(
                    $"{candilist.UserProfileId},{candilist.RegistrationNumber},{candilist.FirstName},{candilist.MiddleName},{candilist.LastName},{candilist.Email},{candilist.MobileNumber},{candilist.Dob},{candilist.CreatedDate},{candilist.UserProfileType},{candilist.OfficeContactNumber}" +
                    $"{candilist.OfficeFaxNumber},{candilist.BusinessType},{candilist.CompanyName},{candilist.ResumePath},{candilist.PhotoPath},{candilist.FacebookLink},{candilist.LinkedinLink},{candilist.TwitterLink},{candilist.MaritalStatus},{candilist.IsActive}"
                    );
            }


            return File(Encoding.UTF8.GetBytes(stringBuilder.ToString()), "text/csv", "RegisteredCandidateList_Report.csv");

        }
        public async Task<IActionResult> Export_EnrolledwithZerofeeCandidateList()
        {

            var userProfile = (from us in _context.UserProfile
                               join uc in _context.UserCourses on us.UserProfileId equals uc.UserProfileId
                               join py in _context.PaymentHistory on uc.UserCoursesId equals py.UserCoursesId
                               where py.Amount == 0
                               where us.UserProfileType == "Candidate"
                               select us
                                                   ).ToList();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("UserProfileId,RegistrationNumber,FirstName,MiddleName,LastName,Email,MobileNumber,Dob,CreatedDate,UserProfileType,OfficeContactNumber,OfficeFaxNumber,BusinessType,CompanyName,ResumePath,PhotoPath,FacebookLink,LinkedinLink,TwitterLink,MaritalStatus,IsActive");
            foreach (var candilist in userProfile)
            {
                stringBuilder.AppendLine(
                    $"{candilist.UserProfileId},{candilist.RegistrationNumber},{candilist.FirstName},{candilist.MiddleName},{candilist.LastName},{candilist.Email},{candilist.MobileNumber},{candilist.Dob},{candilist.CreatedDate},{candilist.UserProfileType},{candilist.OfficeContactNumber}" +
                    $"{candilist.OfficeFaxNumber},{candilist.BusinessType},{candilist.CompanyName},{candilist.ResumePath},{candilist.PhotoPath},{candilist.FacebookLink},{candilist.LinkedinLink},{candilist.TwitterLink},{candilist.MaritalStatus},{candilist.IsActive}"
                    );
            }

            return File(Encoding.UTF8.GetBytes(stringBuilder.ToString()), "text/csv", "RegisteredCandidateList_Report.csv");

        }
        public async Task<IActionResult> Export_AppliedforcollegeCandidateList()
        {

            var userProfile = (from us in _context.UserProfile
                               join cc in _context.CollegeStudentMapping on us.LoginId equals cc.LoginId
                               join col in _context.CollegeProfile on cc.CollegeProfileId equals col.CollegeProfileId
                               where us.UserProfileType == "Candidate"
                               select us).ToList();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("UserProfileId,RegistrationNumber,FirstName,MiddleName,LastName,Email,MobileNumber,Dob,CreatedDate,UserProfileType,OfficeContactNumber,OfficeFaxNumber,BusinessType,CompanyName,ResumePath,PhotoPath,FacebookLink,LinkedinLink,TwitterLink,MaritalStatus,IsActive");
            foreach (var candilist in userProfile)
            {
                stringBuilder.AppendLine(
                    $"{candilist.UserProfileId},{candilist.RegistrationNumber},{candilist.FirstName},{candilist.MiddleName},{candilist.LastName},{candilist.Email},{candilist.MobileNumber},{candilist.Dob},{candilist.CreatedDate},{candilist.UserProfileType},{candilist.OfficeContactNumber}" +
                    $"{candilist.OfficeFaxNumber},{candilist.BusinessType},{candilist.CompanyName},{candilist.ResumePath},{candilist.PhotoPath},{candilist.FacebookLink},{candilist.LinkedinLink},{candilist.TwitterLink},{candilist.MaritalStatus},{candilist.IsActive}"
                    );
            }


            return File(Encoding.UTF8.GetBytes(stringBuilder.ToString()), "text/csv", "RegisteredCandidateList_Report.csv");

        }
    

    }
}

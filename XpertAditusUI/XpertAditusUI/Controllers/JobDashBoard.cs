using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using XpertAditusUI.Models;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using Microsoft.Extensions.Configuration;
using XpertAditusUI.Service;
using XpertAditusUI.DTO;

namespace XpertAditusUI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class JobDashBoard : BaseController
    {

        private readonly XpertAditusDbContext _context;
        private readonly UserProfileService _userProfileService;
        private readonly IMapper _mapper;
        private readonly TestService _testService;
        private IConfiguration _configuration;

        public JobDashBoard(XpertAditusDbContext context, UserProfileService userProfileService, TestService testService, IConfiguration configurationManager,
          ILogger<HomeController> logger, UserManager<IdentityUser> userManager, IMapper mapper)
          : base(logger, userManager)
        {
            _context = context;
            _userProfileService = userProfileService;
            _mapper = mapper;
            _testService = testService;
            _configuration = configurationManager;
        }

        [HttpGet("JobDashBoards")]
        public IActionResult JobDashBoards(int sortBy = 0, string category = "")
        {

            var userId = this._userManager.GetUserId(this.User);
            var userProfile = this._testService.GetUserProfile(userId);
            ViewBag.AppliedJobsInfo = _userProfileService.GetAppliedJobs(userProfile.UserProfileId);
            ViewBag.JobDashBoardsInfo = _userProfileService.GetJobInfo();

            var activeCandidate = (from c in _context.UserCourses
                                   join p in _context.UserProfile.Where(p => p.LoginId == userId)
                                   on c.UserProfileId equals p.UserProfileId
                                   join r in _context.CandidateResult
                                   on c.UserCoursesId equals r.UserCoursesId
                                   where
                                   r.IsCleared == true &&
                                   p.UserProfileType == "Candidate" 
                                   select r).ToList();

            ViewBag.ActiveCandidateResult = activeCandidate.FirstOrDefault();

            ViewBag.Cities = _context.CityMaster.Select(c => c.Name).ToList();
          
            return View();
        }
      
        [HttpGet("JobInfo")]
        public IActionResult JobInfo(Guid jobId)
        {
            var userId = this._userManager.GetUserId(this.User);
            var userProfile = this._testService.GetUserProfile(userId);
            ViewBag.appliedjobsById = _userProfileService.GetAppliedJobsById(jobId, userProfile.UserProfileId);
            ViewBag.jobInfo = _userProfileService.GetJobInfoById(jobId);
            return View();
        }

        [HttpGet("JobDashBoardsOperation")]
        public IActionResult JobDashBoardsOperation(int sortBy = 0, string city = "", string category = "", string keyword = "", bool SalaryRange1 = false, bool SalaryRange2 = false, bool SalaryRange3 = false, bool SalaryRange4 = false, bool SalaryRange5 = false, int? valuetake = 10, int? valueskip = 0)
        {
            var result = _userProfileService.GetJobInfo(sortBy, city, category, keyword, SalaryRange1, SalaryRange2, SalaryRange3, SalaryRange4, SalaryRange5, valuetake, valueskip);
            return new JsonResult(result);
        }

        [HttpGet("CandidateDashBoardOperation")]
        public IActionResult CandidateDashBoardOperation(Guid location, int sortBy = 0, string category = "", string keyword = "", int valuetake = 10, int valueskip = 0)
        {
            var result = _userProfileService.GetCandidateInfo(location, sortBy, category, keyword, valuetake, valueskip);
            return new JsonResult(result);
        }

        [HttpGet("AppliedJobList")]
        public IActionResult AppliedJobList(Guid jobId)
        {

            var userId = this._userManager.GetUserId(this.User);
            var userProfile = this._testService.GetUserProfile(userId);
            ViewBag.applyinfo = _userProfileService.GetAppliedJobs(userProfile.UserProfileId);
            ViewBag.apply = _userProfileService.GetJobInfo();
            ViewBag.Appliedjoblist = _userProfileService.GetAppliedJobList(userProfile.UserProfileId);
            ViewBag.ApprovedJobs = _context.ApprovalJobs.Where(e => e.UserProfileId == userProfile.UserProfileId).ToList();
            return View();
        }



        [HttpGet("AppliedJobInfo")]
        public IActionResult AppliedJobInfo(Guid jobId)
        {
            var userId = this._userManager.GetUserId(this.User);
            var userProfile = this._testService.GetUserProfile(userId);
            ViewBag.applyId = _userProfileService.GetAppliedJobsById(jobId, userProfile.UserProfileId);
            ViewBag.appliedjoninfo = _userProfileService.GetJobInfoById(jobId);
            return View();
        }

        [HttpPost("Send_AppliedJobNotification")]
        public IActionResult Send_AppliedJobNotification(Guid jobId, string firstName)
        {
            var userId = this._userManager.GetUserId(this.User);
            var userProfile = this._testService.GetUserProfile(userId);
            var jobMaster = _context.JobMaster.Where(j => j.IsActive == "True" && j.JobId == jobId).FirstOrDefault();

            var empProfile = _context.UserProfile.Where(e => e.UserProfileId == jobMaster.UserProfileId).FirstOrDefault();

            if (empProfile != null)
            {
                MailMessage mailMessage = new MailMessage(_configuration["SMTPConfig:SenderAddress"].ToString(), empProfile.Email);
                mailMessage.Subject = "Candidate Applied Job!";
                mailMessage.Body =
                "Dear " + empProfile.FirstName + ",<br><br>" +
                "You have successfully applied for post of " + jobMaster.Description + "in" + jobMaster.CompanyName + "<br><br>";

                mailMessage.IsBodyHtml = Convert.ToBoolean(_configuration["SMTPConfig:IsBodyHTML"]);

                SmtpClient smtpClient = new SmtpClient(_configuration["SMTPConfig:host"].ToString(), int.Parse(_configuration["SMTPConfig:Port"]));
                smtpClient.Credentials = new System.Net.NetworkCredential()
                {
                    UserName = _configuration["SMTPConfig:UserName"].ToString(),
                    Password = _configuration["SMTPConfig:Password"].ToString()
                };
                smtpClient.EnableSsl = Convert.ToBoolean(_configuration["SMTPConfig:EnableSSL"]);
                smtpClient.Send(mailMessage);


                AppliedJobs appliedJobs = new AppliedJobs()
                {
                    AppliedJobId = Guid.NewGuid(),
                    UserProfileId = userProfile.UserProfileId,
                    JobId = jobId,
                    AppliedDate = DateTime.Now,
                    StatusUpdatedDate = DateTime.Now,
                    CreatedBy = userId,
                    CreatedDate = DateTime.Now,
                    Status = "Applied",
                    ModifiedBy = User.Claims.Select(x => x.Value).First(),
                    ModifiedDate = DateTime.Now

                };
                _context.AppliedJobs.Add(appliedJobs);
                _context.SaveChanges();
                appliedJobs.Status = "Applied";

            }
            else
            {
                NotFound();
            }

            return Ok(new ResponseResult()
            {
                Error = false,
                Message = "Congratulation! You have successfully applied to the job.",
            });

        }
    }
}

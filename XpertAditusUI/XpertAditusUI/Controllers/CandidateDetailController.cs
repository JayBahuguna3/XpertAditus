using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.Models;
using XpertAditusUI.Service;
using XpertAditusUI.DTO;
using System.Net.Mail;

namespace XpertAditusUI.Controllers
{
    [Route("[controller]")]
    public class CandidateDetailController : BaseController
    {
        private readonly XpertAditusDbContext _db;
        private readonly TestService _testService;
        private IConfiguration _configuration;
        private readonly UserProfileService _userProfileService;
        private readonly IMapper _mapper;

        public CandidateDetailController(XpertAditusDbContext db, UserProfileService userProfileService, TestService testService, IConfiguration configurationManager,
          ILogger<HomeController> logger, UserManager<IdentityUser> userManager, IMapper mapper)
          : base(logger, userManager)
        {
            _db = db;
            _userProfileService = userProfileService;
            _mapper = mapper;
            _testService = testService;
            _configuration = configurationManager;
        }
        public static string userProfileID = null;
        [HttpPost("CandidateDetail")]
        public IActionResult CandidateDetail()
        {

            userProfileID = Request.Form["UserProfile_ID"].ToString();
            return View();
        }

        [HttpGet("CandidateDetail")]
        public IActionResult CandidateDetail(int x)
        {
            CandidateDetail candidateDetail = null;

            if (userProfileID != null)
            {
                candidateDetail = new CandidateDetail();
                string s = userProfileID;
                UserProfile userProfile = _db.UserProfile.Where(r => r.UserProfileId == Guid.Parse(s)).FirstOrDefault();
                //Qualification qualification = _db.Qualification.Where(r => r.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
                //EducationMaster eduMaster = _db.EducationMaster.Where(r => r.EducationId.ToString() == qualification.Name).FirstOrDefault();
                //OrderByDescending(r => r.CreatedDate).FirstOrDefault();
                candidateDetail.UserProfileID = userProfile.UserProfileId;

                if (userProfile.FacebookLink != null)
                    candidateDetail.FacebookLink = userProfile.FacebookLink;
                else
                    candidateDetail.FacebookLink = "#";

                if (userProfile.LinkedinLink != null)
                    candidateDetail.LinkedinLink = userProfile.LinkedinLink;
                else
                    candidateDetail.LinkedinLink = "#";

                if (userProfile.PhotoPath != null)
                    candidateDetail.PhotoPath = "/" + userProfile.PhotoPath;

                if (userProfile.TwitterLink != null)
                    candidateDetail.TwitterLink = userProfile.TwitterLink;
                else
                    candidateDetail.TwitterLink = "#";

                //if (qualification != null)
                //    candidateDetail.Qualification = eduMaster.Name;
                //else
                //    candidateDetail.Qualification = "";

                if (userProfile.ResumePath != null)
                    candidateDetail.ResumePath = userProfile.ResumePath;
                else
                    candidateDetail.ResumePath = "#";

                candidateDetail.CandidateName = userProfile.FirstName + " " + userProfile.LastName;

                List<Experience> experiences = _db.Experience.Where(r => r.UserProfileId == Guid.Parse(s))
                    .OrderByDescending(r => r.ChronologicalOrder).ToList();
                if (experiences.Count > 0)
                    candidateDetail.Designation = experiences[0].DesignationName;
                else
                    candidateDetail.Designation = "";

                candidateDetail.Experiences = experiences;

                List<Qualification> qualifications = _db.Qualification.Where(r => r.UserProfileId == Guid.Parse(s))
                    .OrderByDescending(r => r.ChronologicalOrder).ToList();

                for(int i = 0; i < qualifications.Count; i++)
                {
                    EducationMaster eduMaster = _db.EducationMaster.Where(r => r.EducationId.ToString() == qualifications[i].Name).FirstOrDefault();
                    qualifications[i].Name = eduMaster.Name;
                }
                

                candidateDetail.Qualifications = qualifications;

                List<InterviewResult> interviewResults = _db.InterviewResult.Where(r => r.UserProfileId == Guid.Parse(s))
                    .ToList();

                candidateDetail.InterviewResults = interviewResults;

                var jobList = _db.JobMaster.Where(e => e.IsActive == "True" && e.CreatedBy == User.Claims.Select(x => x.Value).First()).ToList();
                var jobMasterDTOList = _mapper.Map<List<JobMasterDTO>>(jobList);
                ViewBag.Jobs = jobMasterDTOList;
            }

            return View(candidateDetail);
        }


        [HttpPost("Send_InterviewNotification")]
        public IActionResult Send_InterviewNotification(Guid userProfileID,Guid JobId)
        {
            var userId = this._userManager.GetUserId(this.User);
            var empProfile = this._testService.GetUserProfile(userId);
            var candidateProfile = _db.UserProfile.Where(c => c.UserProfileId == userProfileID).FirstOrDefault();
            AppliedJobs appliedJobs = _db.AppliedJobs.Where(a => a.UserProfileId == candidateProfile.UserProfileId).FirstOrDefault();
            JobMaster jobId = _db.JobMaster.Where(j => j.JobId == JobId).FirstOrDefault();

            if (jobId != null)
            {
                MailMessage mailMessage = new MailMessage(_configuration["SMTPConfig:SenderAddress"].ToString(), empProfile.Email);
                mailMessage.Subject = "Shortlisted for Interview";
                mailMessage.Body =
                "Dear " + candidateProfile.FirstName + ",<br><br>" +
                "Congratulations! You are selected for our further Interview Round.";

                mailMessage.IsBodyHtml = Convert.ToBoolean(_configuration["SMTPConfig:IsBodyHTML"]);

                SmtpClient smtpClient = new SmtpClient(_configuration["SMTPConfig:host"].ToString(), int.Parse(_configuration["SMTPConfig:Port"]));
                smtpClient.Credentials = new System.Net.NetworkCredential()
                {
                    UserName = _configuration["SMTPConfig:UserName"].ToString(),
                    Password = _configuration["SMTPConfig:Password"].ToString()
                };
                smtpClient.EnableSsl = Convert.ToBoolean(_configuration["SMTPConfig:EnableSSL"]);
                smtpClient.Send(mailMessage);

                if (appliedJobs == null)
                {
                    ShortlistedCandidates shortlistedCandidates = new ShortlistedCandidates()
                    {
                        Id = Guid.NewGuid(),
                        CandidateId = candidateProfile.UserProfileId,
                        EmployerId = empProfile.UserProfileId,
                        JobId = JobId,
                        NotificationDate = DateTime.Now,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = User.Claims.Select(x => x.Value).First(),
                        ModifiedDate = DateTime.Now

                    };
                    _db.ShortlistedCandidates.Add(shortlistedCandidates);
                    _db.SaveChanges();
                }
                else
                {
                    ShortlistedCandidates shortlistedCandidates = new ShortlistedCandidates()
                    {
                        Id = Guid.NewGuid(),
                        AppliedJobId = appliedJobs.AppliedJobId,
                        CandidateId = candidateProfile.UserProfileId,
                        EmployerId = empProfile.UserProfileId,
                        JobId = JobId,
                        NotificationDate = DateTime.Now,
                        //Status = "Send",
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now,
                        ModifiedBy = User.Claims.Select(x => x.Value).First(),
                        ModifiedDate = DateTime.Now

                    };
                    _db.ShortlistedCandidates.Add(shortlistedCandidates);
                    _db.SaveChanges();
                }

                return Ok(new ResponseResult()
                {
                    Error = false,
                    Message = "You have successfully applied to this candidate for Interview Round.",
                });

            } 
            else
            {
                return Ok(new ResponseResult()
                {
                    Error = true,
                    Message = "Please Select Job..."
                });
            }

        }

    }
}

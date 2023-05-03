using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.DTO;
using XpertAditusUI.Models;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class PACaseStudyController : Controller
    {
		private readonly UserProfileService _userProfileService;
		private readonly TestService _testService;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly CandidateService _candidateService;
		private readonly XpertAditusDbContext _context;
		private Microsoft.Extensions.Configuration.IConfiguration _configuration;
		private readonly IWebHostEnvironment _env;


		public PACaseStudyController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
		  UserProfileService userProfileService, TestService testService, CandidateService candidateService, XpertAditusDbContext context,
		  Microsoft.Extensions.Configuration.IConfiguration configuration, IWebHostEnvironment env)
		{
			_userProfileService = userProfileService;
			_testService = testService;
			_userManager = userManager;
			_candidateService = candidateService;
			_context = context;
			_configuration = configuration;
			_env = env;
		}
		[HttpGet("PACaseStudy")]
		public IActionResult PACaseStudy()
        {
            var userProfile = this._userManager.GetUserProfile(this.User, this._context);

            var admission = _context.Admission.Where(p => p.UserProfileId == userProfile.UserProfileId).FirstOrDefault();

			var PAMonthlyTest = _context.PamonthlyTest
				.Where(p => p.CourseId == admission.CourseId
				&& p.TestType == "CaseStudy"
				&& p.Month == DateTime.Now.Month
				&& p.Year == DateTime.Now.Year).FirstOrDefault();
			if (PAMonthlyTest != null)
			{
				ViewBag.MonthlyTestAvailable = true;
				var PACandidateResult = _context.PacandidateResult
			.Where(p => p.MonthlyTestId == PAMonthlyTest.MonthlyTestId && p.CreatedBy == userProfile.LoginId).FirstOrDefault();

				if (PACandidateResult == null)
				{
					var CandidateResult = new PacandidateResult();
					CandidateResult.PacandidateResultId = Guid.NewGuid();
					CandidateResult.TestDuration = 1;
					CandidateResult.TestAttempt = 1;
					CandidateResult.IsActive = true;
					CandidateResult.UserProfileId = userProfile.UserProfileId;
					CandidateResult.Status = "Pending";
					CandidateResult.MonthlyTestId = PAMonthlyTest.MonthlyTestId;
					CandidateResult.CreatedDate = DateTime.Now;
					CandidateResult.CreatedBy = userProfile.LoginId;
					_context.PacandidateResult.Add(CandidateResult);
					_context.SaveChanges();
					PAMonthlyTest.CandidateResultId = CandidateResult.PacandidateResultId;
					ViewBag.TestCompleted = false;
				}
				else if (PACandidateResult.Status == "Submitted")
				{
					ViewBag.TestCompleted = true;
				}
				else
				{
					PAMonthlyTest.CandidateResultId = PACandidateResult.PacandidateResultId;
					ViewBag.TestCompleted = false;
				}
			}
            else
            {
				ViewBag.MonthlyTestAvailable = false;
				ViewBag.TestCompleted = false;

			}

			return View(PAMonthlyTest);
        }

		[HttpPost("SaveCaseStudyDocuments")]
		public IActionResult SaveCaseStudyDocuments([FromForm] PamonthlyTest pamonthlyTest)
		{
			try
			{
				var userProfile = this._userManager.GetUserProfile(this.User, this._context);
				var paCandidateResult = _context.PacandidateResult.Where(p => 
				p.PacandidateResultId == pamonthlyTest.CandidateResultId).FirstOrDefault();
				paCandidateResult.UserProfileId = userProfile.UserProfileId;
				for (int i = 0; i < pamonthlyTest.Attachments.Count; i++)
				{
					var attachments = new PatestCaseAttachments();
					attachments.TestCaseAttachmentId = Guid.NewGuid();
					attachments.PaCandidateResultId = pamonthlyTest.CandidateResultId;

					string folder = "CaseStudyDocuments/";

					if (!Directory.Exists(Path.Combine(_env.WebRootPath, folder)))
					{
						Directory.CreateDirectory(Path.Combine(_env.WebRootPath, folder));
					}

					folder += "CaseStudy_" + userProfile.FirstName + "_" + userProfile.LastName + "_"
						+ Guid.NewGuid() + System.IO.Path.GetExtension(pamonthlyTest.Attachments[i].FileName);

					string serverFolder = Path.Combine(_env.WebRootPath, folder);
					using (FileStream resumefileStream = new FileStream(serverFolder, FileMode.Create))
					{
						pamonthlyTest.Attachments[i].CopyTo(resumefileStream);
					}

					attachments.AttachmentPath = folder.ToString();
					attachments.CreatedDate = DateTime.Now;
					attachments.CreatedBy = userProfile.LoginId;

					_context.PatestCaseAttachments.Add(attachments);
                  
                    paCandidateResult.Status = "Submitted";

                    _context.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.Message);
				ViewBag.TestCompleted = false;
				return View("/Views/PACaseStudy/PACaseStudy.cshtml");
			}
			ViewBag.MonthlyTestAvailable = false;
			ViewBag.TestCompleted = true;
			return View("/Views/PACaseStudy/PACaseStudy.cshtml");
		}

		//Get PATestCompleted Student List
		[HttpGet("PATestCompletedStudentList")]
		public IActionResult PATestCompletedStudentList()
		{

			var userid = this._userManager.GetUserId(this.User);
			var collegeProfileid = this._context.CollegeProfile.Where(u => u.LoginId == userid).FirstOrDefault();
            /*ViewBag.PATestCompletedStudentList = (from c in _context.PacandidateResult
                                                  join m in _context.PamonthlyTest on c.MonthlyTestId equals m.MonthlyTestId
                                                  join a in _context.Admission on m.CourseId equals a.CourseId
                                                  join cou in _context.CourseMaster on a.CourseId equals cou.CourseId
                                                  where c.Status == "Complete"
                                                  && m.TestType == "CaseStudy"
												  && a.CollegeProfileId == collegeProfileid.CollegeProfileId
                                                  select new PATestCompletedStudentListDTO()
                                                  {
                                                      UserProfileId = (Guid)a.UserProfileId,
                                                      FirstName = a.FirstName,
                                                      MiddleName = a.MiddleName,
                                                      LastName = a.LastName,
                                                      CourseName = cou.Name,
                                                      TestCourseName = m.Name,
                                                      Score = (decimal)c.Score,
                                                      Status = c.Status
                                                  }).ToList();*/
			//this is not in used

            return View();
		}
		[HttpGet("PACaseStudySubmittedStudentList")]
		public IActionResult PACaseStudySubmittedStudentList()
		{

			var userid = this._userManager.GetUserId(this.User);
			var collegeProfileid = this._context.CollegeProfile.Where(u => u.LoginId == userid).FirstOrDefault();
			//  var completedStudentList = (from c in _context.PacandidateResult
			//                              join m in _context.PamonthlyTest on c.MonthlyTestId equals m.MonthlyTestId
			//                              join a in _context.Admission on m.CourseId equals a.CourseId
			//join b in _context.UserProfile on c.UserProfileId equals b.UserProfileId                     
			//                              where c.Status == "Submitted"
			//                              && m.TestType == "CaseStudy"

			//                              && a.CollegeProfileId == collegeProfileid.CollegeProfileId
			//                              select new PATestCompletedStudentListDTO()
			//                              {
			//                                  UserProfileId = (Guid)a.UserProfileId,
			//                                  FirstName = a.UserProfile.FirstName,
			//                                  MiddleName = a.UserProfile.MiddleName,
			//                                  LastName = a.UserProfile.LastName,
			//                                  CourseName = c.MonthlyTest.Course.Name,
			//                                  TestCourseName = c.MonthlyTest.Name,
			//                                  Score = 0,
			//                                  Status = c.Status,
			//	Month = m.Month.Value.ToString(),
			//	Year = m.Year.Value.ToString(),
			//                                  PACandidateResultId = c.PacandidateResultId
			//                              }).ToList();

			

			var completedStudentList = _context.PacandidateResult.Include(e => e.MonthlyTest)

				 .ThenInclude(e => e.Course)
				 .Include(e => e.UserProfile)
				 .ThenInclude(e => e.Admission)
				 .Where(c => c.MonthlyTest.TestType == "CaseStudy" && c.Status == "Submitted" &&
				 c.UserProfile.Admission
				 .Where(a => a.UserProfileId == c.UserProfileId && a.CollegeProfileId == collegeProfileid.CollegeProfileId).Count() > 0
				 )
			.Select(e => new PATestCompletedStudentListDTO()
			{
				UserProfileId = (Guid)e.UserProfileId,
				FirstName = e.UserProfile.FirstName,
				MiddleName = e.UserProfile.MiddleName,
				LastName = e.UserProfile.LastName,
				CourseName = e.MonthlyTest.Course.Name,
				TestCourseName = e.MonthlyTest.Name,
				Score = 0,
				Status = e.Status,
				Month = e.MonthlyTest.Month.Value.ToString(),
				Year = e.MonthlyTest.Year.Value.ToString(),
				PACandidateResultId = e.PacandidateResultId
			}); ;
            //this is not working fine

            var list = completedStudentList.ToArray();

            for (int i = 0; i < completedStudentList.Count(); i++)
            {
				list[i].Score = this._context.PatestCaseAttachments.
                    Where(e => e.PaCandidateResultId == list[i].PACandidateResultId).Sum(e => (decimal)e.Score);

            }
            ViewBag.PATestCompletedStudentList = list;
			return View();
		}

		[HttpGet("TestCaseAttachment/{id}")]
		public IActionResult TestCaseAttachment(Guid id)
		{
			var t = _context.PatestCaseAttachments.Where(e => e.PaCandidateResultId == id).ToList();
			return View(t);
		}

		[HttpGet("SaveScore/{id}/{feedback}/{score}")]
		public IActionResult SaveScore(string id, string feedback, string score)
		{
			try
			{
				var t = _context.PatestCaseAttachments.Where(e => e.TestCaseAttachmentId == new Guid(id)).FirstOrDefault();
				if (t == null)
					return NotFound();
				else
				{
					t.Score = int.Parse(score);
					t.Feedback = feedback;

					_context.Update(t);
					_context.SaveChanges();
					return new JsonResult("Success");
				}

			}
			catch (Exception ex)
            {
				return new JsonResult(ex.Message);
            }
		}



	}
}

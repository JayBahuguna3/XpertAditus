using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.DTO;
using XpertAditusUI.Models;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    public class PADisclaimerAcceptedController : Controller
    {

		private readonly UserProfileService _userProfileService;
		private readonly TestService _testService;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly CandidateService _candidateService;
		private readonly XpertAditusDbContext _context;
		private Microsoft.Extensions.Configuration.IConfiguration _configuration;


		public PADisclaimerAcceptedController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
		  UserProfileService userProfileService, TestService testService, CandidateService candidateService, XpertAditusDbContext context,
		  Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
			_userProfileService = userProfileService;
			_testService = testService;
			_userManager = userManager;
			_candidateService = candidateService;
			_context = context;
			_configuration = configuration;
		}

		public IActionResult PADisclaimer()
        {
			var userProfile = this._userManager.GetUserProfile(this.User, this._context);
			var admission = _context.Admission.Where(p => p.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
			if (admission != null)
			{
				var paDisclaimer = _context.Padisclaimer.Where(p => p.CourseId == admission.CourseId).FirstOrDefault();
				ViewData["PassingCriteria"] = _configuration["AppSettings:PassingCriteria"].ToString();
				ViewData["CourseId"] = admission.CourseId.ToString();
				ViewData["Attempt"] = 0;
				ViewBag.Course = _context.CourseMaster.Where(c => c.CourseId == admission.CourseId).FirstOrDefault();
				var PAMonthlyTest = _context.PamonthlyTest
					.Where(p => p.CourseId == admission.CourseId
					&& p.TestType == "MCQ"
					&& p.Month == DateTime.Now.Month
					&& p.Year == DateTime.Now.Year).FirstOrDefault();
				if (PAMonthlyTest != null)
				{
					ViewBag.MonthlyTestAvailable = true;
					ViewBag.MonthlyTestID = PAMonthlyTest.MonthlyTestId;
					var PACandidateResult = _context.PacandidateResult
						.Where(p => p.MonthlyTestId == PAMonthlyTest.MonthlyTestId && p.CreatedBy == userProfile.LoginId).FirstOrDefault();

					if (PACandidateResult == null)
					{
						var CandidateResult = new PacandidateResult();
						CandidateResult.PacandidateResultId = Guid.NewGuid();
						CandidateResult.TestDuration = 1;
						CandidateResult.TestAttempt = 1;
						CandidateResult.IsActive = true;
						CandidateResult.Status = "Pending";
						CandidateResult.UserProfileId = userProfile.UserProfileId;
						CandidateResult.MonthlyTestId = PAMonthlyTest.MonthlyTestId;
						CandidateResult.CreatedDate = DateTime.Now;
						CandidateResult.CreatedBy = userProfile.LoginId;
						_context.PacandidateResult.Add(CandidateResult);
						_context.SaveChanges();
						ViewBag.TestCompleted = false;
					}
					else if (PACandidateResult.IsCleared != true)
					{
						ViewBag.TestCompleted = false;
					}
					else
					{
						ViewBag.TestCompleted = true;
					}
				}
				else
				{
					ViewBag.TestCompleted = false;
					ViewBag.MonthlyTestAvailable = false;
				}
				return View(paDisclaimer);
			}
            else
            {
				ViewBag.TestCompleted = false;
				ViewBag.MonthlyTestAvailable = false;
				return View();
			}

		}

		public async Task<ActionResult> PADisclaimerAccepted(string disclaimerId, string monthlytestid = "")
		{
			try
			{
				var userProfile = this._userProfileService.GetUserInfo(this._userManager.GetUserId(this.User));
				//var pendingTest = _testService.GetPendingUserCourseTest(userProfile.UserProfileId);

				//var Disclaimer = _userProfileService.GetDisclaimerById(disclaimerId);
				_userProfileService.SavePADisclaimerInfo( new Guid(disclaimerId), userProfile, monthlytestid);

			return Ok(new ResponseResult()
			{
					Error = false,
					Message = "Success"
				});
			}
			catch (Exception ex)
			{
				return Ok(new ResponseResult()
				{
					Error = true,
					Message = ex.Message
				});
			}
		}
	}
}

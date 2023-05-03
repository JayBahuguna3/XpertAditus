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
	[Route("[controller]")]
	[ApiController]
	public class DisclaimerAcceptedController : Controller
	{
		private readonly UserProfileService _userProfileService;
		private readonly TestService _testService;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly CandidateService _candidateService;
		private readonly XpertAditusDbContext _context;
		private Microsoft.Extensions.Configuration.IConfiguration _configuration;


		public DisclaimerAcceptedController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
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

		private void StartTestCourseFeeZero()
		{
			var userProfile = _candidateService.GetUserProfile(this.User);
			var coursesL = _context.Course.ToList();
			//var courseId = _context.Course.Select(c => c.CourseId).ToString();
			if (coursesL[0].Fee == 0 || coursesL[0].Fee == (decimal) 0.00)
			{
				var userCourses = _context.UserCourses.Where(e => e.UserProfileId == userProfile.UserProfileId
					&& e.IsActive == true).OrderByDescending(e => e.CreatedDate).ToList();

				if(userCourses == null || userCourses.Count==0)
				{

					try
					{
						string amount = "0";
						var TransactionId = _configuration["AppSettings:transactionId"].ToString();
						//string transactionId = "ABCD1234";
						//var userProfile = this._userManager.GetUserProfile(this.User, _context);
						if (userProfile != null )
						{
							PaymentHistory paymentHistory = new PaymentHistory();
							paymentHistory.PaymentHistoryId = Guid.NewGuid();
							paymentHistory.TransactionId = TransactionId;
							paymentHistory.Amount = Decimal.Parse(amount);
							paymentHistory.TransactionDate = DateTime.Now;
							paymentHistory.Description = "";
							paymentHistory.CreatedBy = this._userManager.GetUserId(this.User);
							//Course and amount paid check
							var course = this._context.Course.Where(e => e.CourseId.ToString() == coursesL[0].CourseId.ToString()).FirstOrDefault();
							bool paymentAmountCheck = false;
							var userCourse = new UserCourses();
							var candidateResult = new CandidateResult();

							if (course.Fee < decimal.Parse(amount))
							{
								paymentHistory.Status = "Paid";
							}
							else
							{
								paymentAmountCheck = true;
								paymentHistory.Status = "Paid";
								paymentHistory.Description = "Payment was not sufficient";

							}

							//Add usercourse
							if (paymentAmountCheck)
							{
								userCourse.UserCoursesId = Guid.NewGuid();
								userCourse.CourseId = course.CourseId;
								userCourse.UserProfileId = userProfile.UserProfileId;
								userCourse.IsActive = true;
								userCourse.CreatedBy = userProfile.LoginId;
								userCourse.CreatedDate = DateTime.Now;

								var oldUserCourse = _context.UserCourses.Where(e => e.CourseId.ToString() == coursesL[0].ToString()
								&& e.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
								if (oldUserCourse != null)
								{
									oldUserCourse.IsActive = false;
									_context.UserCourses.Update(oldUserCourse);
								}
								_context.UserCourses.Add(userCourse);
								_context.SaveChanges();
								paymentHistory.UserCoursesId = userCourse.UserCoursesId;
							}

							//Add Paymenthistory
							_context.PaymentHistory.Add(paymentHistory);
							_context.SaveChanges();

							//Save CandidateResult
							candidateResult.CandidateResultId = Guid.NewGuid();
							candidateResult.UserCoursesId = userCourse.UserCoursesId;
							candidateResult.TestDuration = course.TestDuration;
							candidateResult.IsCompleted = false;
							candidateResult.IsCleared = false;
							candidateResult.PaymentHistoryId = paymentHistory.PaymentHistoryId;
							//TODO Update TestAttempt 
							candidateResult.TestAttempt = 0;
							candidateResult.IsActive = true;
							candidateResult.Status = "Pending";

							_context.Add(candidateResult);

							//Save Changes
							_context.SaveChanges();

							var trainings = _context.TrainingContentsMaster.Where(t => t.CourseId == course.CourseId).ToList();

							foreach(TrainingContentsMaster trow in trainings)
							{
								CandidateTrainingStatus candidateTrainingStatus = new CandidateTrainingStatus()
								{
									CandidateTrainingStatusId = Guid.NewGuid(),
									CompletedDate = DateTime.Now,
									CreatedBy = userProfile.LoginId,
									CreatedDate = DateTime.Now,
									DownloadedDate = DateTime.Now,
									TrainingContentId =trow.TrainingContentId,
									// TrainingContentId = new Guid("7F15626E-E3E6-404D-9094-CCFD778C68C5"),
									UserCoursesId = userCourse.UserCoursesId
								};
								_context.CandidateTrainingStatus.Add(candidateTrainingStatus);
								_context.SaveChanges();

							}

							//var TrainingcontentId = _configuration["AppSettings:TrainingContentId"].ToString();
							//CandidateTrainingStatus candidateTrainingStatus = new CandidateTrainingStatus()
							//{
							//	CandidateTrainingStatusId = Guid.NewGuid(),
							//	CompletedDate = DateTime.Now,
							//	CreatedBy = userProfile.LoginId,
							//	CreatedDate = DateTime.Now,
							//	DownloadedDate = DateTime.Now,
							//	TrainingContentId = Guid.Parse(TrainingcontentId),
							//	// TrainingContentId = new Guid("7F15626E-E3E6-404D-9094-CCFD778C68C5"),
							//	UserCoursesId = userCourse.UserCoursesId
							//};
							//_context.CandidateTrainingStatus.Add(candidateTrainingStatus);
							//_context.SaveChanges();

							//return Ok(new ResponseResult()
							//{
							//	Error = false,
							//	Message = "Payment Successful!",
							//	Id = paymentHistory.PaymentHistoryId.ToString()
							//});
						}
						else
						{
							//return Ok(new ResponseResult()
							//{
							//	Error = true,
							//	Message = "Validation Failed!"
							//});
						}
					}
					catch (Exception ex)
					{
						//return Ok(new ResponseResult()
						//{
						//	Error = true,
						//	Message = ex.Message
						//});
					}
				}
			}

		}

		[HttpGet("DisclaimerAccepted")]
		public IActionResult DisclaimerAccepted()
		{
			var userProfile = _candidateService.GetUserProfile(this.User);
			var CourseLimitations = _configuration["Limitaions:CourseLimitations"].ToString();
			UserCourses userCourses = _context.UserCourses.Where(e =>  e.UserProfileId == userProfile.UserProfileId
									 && e.IsActive == true).FirstOrDefault();
			if (userCourses != null)
			{
				CandidateResult candidateResult = _context.CandidateResult.Where(c => c.UserCoursesId == userCourses.UserCoursesId
													 && c.IsActive == true ).FirstOrDefault();
				if (candidateResult != null)
				{
					if (candidateResult.TestAttempt == Int32.Parse(CourseLimitations) && candidateResult.IsCompleted==true)
					{
						ViewBag.AttempedAll = true;
						return View();
					}
				}
			}
			StartTestCourseFeeZero();


			ViewBag.NoDisclaimer = false;
			ViewData["PassingCriteria"] = _configuration["AppSettings:PassingCriteria"].ToString();
			var pendingTest = _testService.GetPendingUserCourseTest(userProfile.UserProfileId);

			//var userCourses = _context.UserCourses.Where(e => e.UserProfileId == userProfile.UserProfileId
			//    && e.IsActive == true).OrderByDescending(e => e.CreatedDate).ToList();
			if (pendingTest != null)
			{
				ViewBag.Course = _testService.GetCourseByUserCourseId(pendingTest.CourseId);

				//foreach (var item in userCourses)
				{
					//string CourseName = _context.Course.Where(c => c.CourseId == item.CourseId).Select(c => c.Name).FirstOrDefault();
					var CourseTrainings = _context.TrainingContentsMaster.Where(x => x.CourseId == pendingTest.CourseId).ToList();

					var CheckTraining = _context.CandidateTrainingStatus.Where(c => c.UserCoursesId == pendingTest.UserCoursesId).ToList();
					//var activeTest = this._context.CandidateResult.Where(e => e.UserCoursesId == item.UserCoursesId 
					// && e.IsActive == true && e.Status == "InProgress").FirstOrDefault();

					//if (CheckTraining.Count == CourseTrainings.Count)
					{
						if (ViewBag.Course != null)
						{
							var Disclaimer = _userProfileService.GetDisclaimerByCourseId(ViewBag.Course.CourseId);

							var userid = this._userManager.GetUserId(this.User);
							if (Disclaimer != null)
							{
								var result = _userProfileService.GetDisclaimerAcceptedById(userid, Disclaimer.DisclaimerId.ToString(), pendingTest.UserCoursesId.ToString());

								if (result != null && result.IsAccepted == true)
								{
									ViewBag.TestType = _candidateService.GetCourseInfo(ViewBag.Course.CourseId.ToString()).Type;

									ViewBag.TranningInfo = _candidateService.getTrainingContentsMaster(new Guid(ViewBag.Course.CourseId.ToString())).
									Select(a => new TrainingContentsMaster { Name = a.Name, TrainingContentId = a.TrainingContentId });

									return View("Views/Test/Test.cshtml");
								}
								else
								{
									ViewData["Attempt"] = _context.CandidateResult.Where(c => c.UserCoursesId == pendingTest.UserCoursesId).Count();
									ViewBag.Course.TestDuration = ViewBag.Course.TestDuration == null ? 1 : ViewBag.Course.TestDuration;
									return View(Disclaimer);
								}
							}
							else
							{
								ViewBag.NoDisclaimer = true;
								return View();
							}
						}
						else
						{
							return View();

						}
					}
					//else
					//{
					//	ViewBag.IsTrainingComplete = false;
					//	return View();
					//}
				}
			}
			else
			{
				return View();
			}
			return View();

			//ViewBag.ActiveUserCourses = _candidateService.GetActiveUserCourses(userProfile);
			//ViewBag.IsTrainingComplete = _candidateService.IsTrainingComplete(ViewBag.ActiveUserCourses);


			//if (ViewBag.Course != null)
			//{
			//    var Disclaimer = _userProfileService.GetDisclaimerByCourseId(ViewBag.Course.CourseId);

			//    var userid = this._userManager.GetUserId(this.User);

			//    var result = _userProfileService.GetDisclaimerAcceptedById(userid, Disclaimer.DisclaimerId.ToString());

			//    if (result != null && result.IsAccepted == true)
			//    {
			//        return View("Views/Test/Test.cshtml");
			//    }
			//    else
			//    {
			//        ViewBag.Course.TestDuration = ViewBag.Course.TestDuration == null ? 2 : ViewBag.Course.TestDuration;
			//        return View(Disclaimer);
			//    }
			//}
			//else
			//{
			//    return View();

			//}
		}

		[HttpPost("DisclaimerAccepted")]
		public async Task<ActionResult> DisclaimerAccepted(string disclaimerId)
		{
			try
			{
				var userProfile = this._userProfileService.GetUserInfo(this._userManager.GetUserId(this.User));
				var pendingTest = _testService.GetPendingUserCourseTest(userProfile.UserProfileId);

				var Disclaimer = _userProfileService.GetDisclaimerById(disclaimerId);
				_userProfileService.SaveDisclaimerInfo(Disclaimer.DisclaimerId, userProfile, pendingTest.UserCoursesId);

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

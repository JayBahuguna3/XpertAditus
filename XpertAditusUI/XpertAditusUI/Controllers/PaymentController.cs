using XpertAditusUI.Models;
using System;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using XpertAditusUI.Data;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using XpertAditusUI.DTO;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Globalization;
using System.Threading;
using Spire.Doc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Threading.Tasks;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly XpertAditusDbContext _context;
        private readonly TestService _testService;
        private readonly IMapper _mapper;
        private IConfiguration _configuration;
        private IWebHostEnvironment _hostEnvironment;

        public PaymentController(XpertAditusDbContext context, TestService testService, IConfiguration configurationManager,
          ILogger<HomeController> logger, UserManager<IdentityUser> userManager, IMapper mapper, IWebHostEnvironment environment)
          : base(logger, userManager)
        {
            _testService = testService;
            _context = context;
            _mapper = mapper;
            _hostEnvironment = environment;
            _configuration = configurationManager;
        }
        // GET: Payment
        [HttpGet]
        public ActionResult Payment()
        {
            var userProfile = this._userManager.GetUserProfile(this.User, _context);
            PaymentDTO payment = new PaymentDTO();
            if (userProfile != null)
            {
                payment.key = _configuration["AppSettings:KeyPayment"];
                payment.currency = _configuration["AppSettings:Currency"];
                payment.name = _configuration["AppSettings:ComName"];
                payment.description = _configuration["AppSettings:Description"];
                payment.image = _configuration["AppSettings:Image"];
                payment.theme_color = _configuration["AppSettings:ThemeColor"];

                payment.prefill_name = userProfile.FirstName + " " + userProfile.LastName;
                payment.prefill_email = userProfile.Email;
                payment.prefill_contact = userProfile.MobileNumber.ToString();

                payment.candidateId = userProfile.UserProfileId.ToString();
                payment.userName = this._userManager.GetUserName(this.User);
            }
            return View(payment);
        }

        [HttpGet("GetPaymentInfo")]
        public IActionResult GetPaymentInfo()
        {
            var userProfile = this._userManager.GetUserProfile(this.User, _context);
            PaymentDTO payment = new PaymentDTO();
            if (userProfile != null)
            {
                payment.key = _configuration["AppSettings:KeyPayment"];
                payment.currency = _configuration["AppSettings:Currency"];
                payment.name = _configuration["AppSettings:ComName"];
                payment.description = _configuration["AppSettings:Description"];
                payment.image = _configuration["AppSettings:Image"];
                payment.theme_color = _configuration["AppSettings:ThemeColor"];

                payment.prefill_name = userProfile.FirstName + " " + userProfile.LastName;
                payment.prefill_email = userProfile.Email;
                payment.prefill_contact = userProfile.MobileNumber.ToString();

                payment.candidateId = userProfile.UserProfileId.ToString();
                payment.userName = this._userManager.GetUserName(this.User);
            }
            return Json(payment);
        }

        [HttpPost("SavePaymentDetails")]
        public ActionResult SavePaymentDetails(string amount, string transactionId, string courseid)
        {
            try
            {
                var userProfile = this._userManager.GetUserProfile(this.User, _context);
                if (userProfile != null && decimal.Parse(amount) > 0)
                {
                    PaymentHistory paymentHistory = new PaymentHistory();
                    paymentHistory.PaymentHistoryId = Guid.NewGuid();
                    paymentHistory.TransactionId = transactionId;
                    paymentHistory.Amount = Decimal.Parse(amount);
                    paymentHistory.TransactionDate = DateTime.Now;
                    paymentHistory.Description = "";
                    paymentHistory.CreatedBy = this._userManager.GetUserId(this.User);
                    //Course and amount paid check
                    var course = this._context.Course.Where(e => e.CourseId.ToString() == courseid).FirstOrDefault();
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

                        var oldUserCourse = _context.UserCourses.Where(e => e.CourseId.ToString() == courseid
                        && e.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
                        if(oldUserCourse != null)
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

                    foreach (TrainingContentsMaster trow in trainings)
                    {
                        CandidateTrainingStatus candidateTrainingStatus = new CandidateTrainingStatus()
                        {
                            CandidateTrainingStatusId = Guid.NewGuid(),
                            CompletedDate = DateTime.Now,
                            CreatedBy = userProfile.LoginId,
                            CreatedDate = DateTime.Now,
                            DownloadedDate = DateTime.Now,
                            TrainingContentId = trow.TrainingContentId,
                            // TrainingContentId = new Guid("7F15626E-E3E6-404D-9094-CCFD778C68C5"),
                            UserCoursesId = userCourse.UserCoursesId
                        };
                        _context.CandidateTrainingStatus.Add(candidateTrainingStatus);
                        _context.SaveChanges();

                    }

                    //var TrainingcontentId = _configuration["AppSettings:TrainingContentId"].ToString();
                    //CandidateTrainingStatus candidateTrainingStatus = new CandidateTrainingStatus()
                    //{
                    //    CandidateTrainingStatusId = Guid.NewGuid(),
                    //    CompletedDate = DateTime.Now,
                    //    CreatedBy = userProfile.LoginId,
                    //    CreatedDate = DateTime.Now,
                    //    DownloadedDate = DateTime.Now,
                    //    TrainingContentId = Guid.Parse(TrainingcontentId),
                    //    // TrainingContentId = new Guid("7F15626E-E3E6-404D-9094-CCFD778C68C5"),
                    //    UserCoursesId = userCourse.UserCoursesId
                    //};
                    //_context.CandidateTrainingStatus.Add(candidateTrainingStatus);
                    //_context.SaveChanges();

                    return Ok(new ResponseResult()
                    {
                        Error = false,
                        Message = "Payment Successful!",
                        Id = paymentHistory.PaymentHistoryId.ToString()
                    });
                }
                else
                {
                    return Ok(new ResponseResult()
                    {
                        Error = true,
                        Message = "Validation Failed!"
                    });
                }
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

        [HttpGet("ValidateCourseAvailability")]
        public IActionResult ValidateCourseAvailability(string courseid)
        {
            var PassingCriteria = _configuration["AppSettings:PassingCriteria"].ToString();
            var CourseLimitations = _configuration["Limitaions:CourseLimitations"].ToString();

            var userProfile = this._userManager.GetUserProfile(this.User, _context);
            UserCourses userCourses = _context.UserCourses.Where(e => e.CourseId.ToString() == courseid
                                     && e.UserProfileId == userProfile.UserProfileId
                                     ).FirstOrDefault();

            if (userCourses != null)
            {
                CandidateResult candidateResult = _context.CandidateResult.Where(c => c.UserCoursesId == userCourses.UserCoursesId
                                                     && c.IsActive == true).FirstOrDefault();
                if (candidateResult != null)
                {
                    if (candidateResult.TestAttempt == Int32.Parse(CourseLimitations) && candidateResult.IsCompleted == true)
                    {
                        return Json(new
                        {
                            Error = true,
                            Message = "You have already completed all attempts, please contact to the administrator"
                        });
                    }
                }
            }

            var course = this._context.Course.Where(e => e.CourseId == new Guid(courseid)).FirstOrDefault();
            if (course != null && course.Fee == 0)
            {
                return Json(new
                {
                    Error = true,
                    Message = "Payment not required for the course where fee is zero and candidate can give test directly"
                    //Message = "You have remaining" + " " + decimal.Parse(CourseLimitations) + " "
                    //  + "attempts of Prevoius Selected Course i.e.," + CourseName + ", Please Complete your test attempts..."
                });
            }





			//selected same course
			if (userCourses != null)

			{

				CandidateResult candidateResult = _context.CandidateResult.Where(c => c.UserCoursesId == userCourses.UserCoursesId
											 && c.IsActive == true).FirstOrDefault();
				var cResultTestAttempt = _context.CandidateResult.Where(c => c.UserCoursesId == userCourses.UserCoursesId
											 && c.IsActive == false).FirstOrDefault();
				var userCourse = this._context.UserCourses.Where(e => e.UserProfileId == userProfile.UserProfileId &&
				e.IsActive == true).FirstOrDefault();
				string CourseName = _context.Course.Where(c => c.CourseId == userCourse.CourseId).Select(c => c.Name).FirstOrDefault();
				var testAttemptCount = _testService.GetTestAttempCount(userCourses.CourseId, userProfile.UserProfileId);

				if (candidateResult.Score <= decimal.Parse(PassingCriteria) || candidateResult.Score == null)
				{
					if (testAttemptCount >= int.Parse(CourseLimitations))
					{
						return Json(new { Error = false, Message = "" });
					}
					else
					{
						if (cResultTestAttempt == null)
						{
							return Json(new
							{
								Error = true,
                                Message = "Payment not required for the course where fee is zero and candidate can give test directly"
                                //Message = "You have remaining" + " " + decimal.Parse(CourseLimitations) + " "
                                //  + "attempts of Prevoius Selected Course i.e.," + CourseName + ", Please Complete your test attempts..."
                            });
						}
						else
						{
							return Json(new
							{
								Error = true,
								Message = "You have remaining" + " " + (decimal.Parse(CourseLimitations) - cResultTestAttempt.TestAttempt) + " "
								+ "Test attempts of Prevoius Selected Course i.e.," + CourseName + ", Please Complete your test attempts..."
							});
						}
					}
				}
				else
				{
					return Json(new { Error = true, Message = "Already enrolled!" });
				}
			}
			else
			{
				//selected another course
				var userCourse = this._context.UserCourses.Where(e => e.UserProfileId == userProfile.UserProfileId &&
				 e.IsActive == true).FirstOrDefault();
				ViewBag.UserCourse = userCourse;


				if (userCourse != null)
				{
					CandidateResult cResult = _context.CandidateResult.Where(c => c.UserCoursesId == userCourse.UserCoursesId
												  && c.IsActive == true).FirstOrDefault();

					if (cResult.Score >= decimal.Parse(PassingCriteria) || cResult.Score != null)
					{
						return Json(new { Error = false, Message = "" });
					}
					else
					{
						if (cResult.TestAttempt == (decimal.Parse(CourseLimitations)))
						{
							return Json(new { Error = false, Message = "" });
						}
						else
						{
							string CourseName = _context.Course.Where(c => c.CourseId == userCourse.CourseId).Select(c => c.Name).FirstOrDefault();
							//return Json(new { Error = true, Message = "You have remaining" + " " + (decimal.Parse(CourseLimitations) - cResult.TestAttempt) + " " + "attempts of Prevoius Selected Course i.e.," + CourseName + ", Please Complete your test attempts..." });
							return Json(new { Error = true, Message = "You have already enrolled for Course i.e.," + CourseName + ", Please Complete it first..." });
						}
					}
				}
			}

			return Json(new { Error = false, Message = "" });


            //var userProfile = this._userManager.GetUserProfile(this.User, _context);
            //if (_context.UserCourses.Where(e => e.CourseId.ToString() == courseid
            //&& e.UserProfileId == userProfile.UserProfileId
            //&& e.IsActive == true).Count() > 0)
            //{
            //    return Json(new { Error = true, Message = "Already enrolled!" });
            //}
            //else
            //{
            //    return Json(new { Error = false, Message = "" });
            //}

        }

		[HttpGet("checkCompleteTrainingAndTest")]
		public IActionResult checkCompleteTrainingAndTest()
		{
			var userProfile = this._userManager.GetUserProfile(this.User, _context);

			var courseResult = _context.UserCourses.Where(e => e.UserProfileId == userProfile.UserProfileId
			 && e.IsActive == true).ToList();
			var CourseLimitations = _configuration["Limitaions:CourseLimitations"].ToString();
			var PassingCriteria = _configuration["AppSettings:PassingCriteria"].ToString();

			if (courseResult != null)
			{
				foreach (var item in courseResult)
				{
					string CourseName = _context.Course.Where(c => c.CourseId == item.CourseId).Select(c => c.Name).FirstOrDefault();
					var CourseTrainings = _context.TrainingContentsMaster.Where(x => x.CourseId == item.CourseId).ToList();

					var CheckTraining = _context.CandidateTrainingStatus.Where(c => c.UserCoursesId == item.UserCoursesId).ToList();

					if (CheckTraining.Count == CourseTrainings.Count)
					{
						//var testStatus = _context.CandidateResult.Where(n => n.UserCoursesId == item.UserCoursesId && n.IsActive == true && n.Status == "Complete").FirstOrDefault();

						var testAttemptCount = _testService.GetTestAttempCount(item.CourseId, userProfile.UserProfileId);
						var candidateResult = this._context.CandidateResult.Include(e => e.UserCourses)
							.Where(e => e.UserCoursesId == item.UserCoursesId &&
							e.IsCleared == true).FirstOrDefault();
						//var CheckTestStatus = _context.CandidateResult.Where(c => c.UserCoursesId == item.UserCoursesId).FirstOrDefault();

						//if (CheckTestStatus.TestAttempt == 1)
						if (testAttemptCount < int.Parse(CourseLimitations) && candidateResult == null)
						{
							return Json(new { Error = true, Message = "Dear Candidate, Please complete your Test of previous selected course i.e.," + CourseName });
						}
					}
					else
					{
						return Json(new { Error = true, Message = "Dear Candidate, Please complete your training content and test of previous selected course i.e.," + CourseName });
					}
				}
			}
			else
			{
				return Json(new { Error = false, Message = "" });
			}

			return Json(new { Error = false, Message = "" });
		}

		[HttpGet("DownloadPaymentInvoice")]
        public FileStreamResult DownloadPaymentInvoice(string paymentid)
        {
            try
            {
                var paymentHistory = _context.PaymentHistory.Include(e => e.UserCourses).Where(e => e.PaymentHistoryId.ToString() == paymentid).FirstOrDefault();
                var course = _context.Course.Where(e => e.CourseId == paymentHistory.UserCourses.CourseId).FirstOrDefault();
                var userProfile = _userManager.GetUserProfile(this.User, _context);
                string filepath = Path.Combine(_hostEnvironment.ContentRootPath, "DocFormat\\Invoice.doc");
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;
                Document doc = new Document();
                doc.LoadFromFile(filepath);
                doc.Replace("[DATE]", DateTime.Today.ToLongDateString(), true, true);
                doc.Replace("[CANDIDATENAME]", textInfo.ToTitleCase(userProfile.FirstName + "  " + userProfile.MiddleName + "  " + userProfile.LastName), true, true);
                doc.Replace("[NO]", paymentid, true, true);
                doc.Replace("[AMOUNT]", course.Fee.ToString(), true, true);
                //doc.Replace("[SALARY]", string.Format("{0:n}", 1564865), true, true);
                doc.Watermark = null;
                string savepath = Path.Combine(_hostEnvironment.ContentRootPath, "DocFormat\\" + userProfile.LoginId + DateTime.Now.ToString("dd-MMM-yyyy-hh-mm-ss") + "invoice.pdf");
                doc.SaveToFile(savepath, FileFormat.PDF);
                Stream stream = System.IO.File.OpenRead(savepath);
                SendPaymentInvoice_Mail();
                return File(stream, "application/pdf", "Invoice.pdf");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost("SendPaymentInvoice_Mail")]
        public IActionResult SendPaymentInvoice_Mail()
        {
            var userProfile = this._userManager.GetUserProfile(this.User, this._context);
            var userCourses = this._context.UserCourses.Where(e =>
                              e.UserProfileId == userProfile.UserProfileId && e.IsActive == true).FirstOrDefault();
            var activePaymentHistory = this._context.PaymentHistory.Where(e =>
                e.UserCoursesId == userCourses.UserCoursesId
                && e.Status == "Paid").FirstOrDefault();
            var address = this._context.Address.Where(a => a.UserProfileId == userProfile.UserProfileId).FirstOrDefault();

            var course = this._context.Course.Where(j => j.CourseId == userCourses.CourseId).FirstOrDefault();
            string filePath = Path.Combine(_hostEnvironment.ContentRootPath,
                           "InvoicePaymentEmail_Format\\PaymentInvoice_Mail.html");
            StreamReader str = new StreamReader(filePath);
            string MailText = str.ReadToEnd();
            str.Close();

            MailMessage mailMessage = new MailMessage(_configuration["SMTPConfig:SenderAddress"].ToString(), userProfile.Email);
            mailMessage.Subject = "Payment has been done successfully!";
            mailMessage.Body = MailText
            .Replace("{OrderId}", activePaymentHistory.OrderId.ToString())
            .Replace("{CourseName}", course.Name)
            .Replace("{Date}", DateTime.Today.ToShortDateString())
            .Replace("{Fee}", course.Fee.ToString())
            .Replace("{SubTotal}", course.Fee.ToString())
            .Replace("{TotalAmount}", activePaymentHistory.Amount.ToString())
            .Replace("{Email}", userProfile.Email)
            .Replace("{CandidateName}", (userProfile.FirstName + " " + userProfile.MiddleName + " " + userProfile.LastName));

            if (address != null)
            {
                mailMessage.Body = mailMessage.Body.Replace("{Address}", address.AddressType)
            .Replace("{Address-Line1}", address.Line1)
            .Replace("{Address-Line2}", address.Line2)
            .Replace("{Address-Line3}", address.Line3);
            }

            mailMessage.IsBodyHtml = Convert.ToBoolean(_configuration["SMTPConfig:IsBodyHTML"]);

            SmtpClient smtpClient = new SmtpClient(_configuration["SMTPConfig:host"].ToString(), int.Parse(_configuration["SMTPConfig:Port"]));
            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = _configuration["SMTPConfig:UserName"].ToString(),
                Password = _configuration["SMTPConfig:Password"].ToString()
            };
            smtpClient.EnableSsl = Convert.ToBoolean(_configuration["SMTPConfig:EnableSSL"]);
            smtpClient.Send(mailMessage);
            return RedirectToPage("./SelectCourse");
        }

    }
}

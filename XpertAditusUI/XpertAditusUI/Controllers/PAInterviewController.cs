using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.DTO;
using XpertAditusUI.Models;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class PAInterviewController : Controller
    {
        private readonly UserProfileService _userProfileService;
        private readonly XpertAditusDbContext _context;
        private IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public PAInterviewController(UserManager<IdentityUser> userManager,
            XpertAditusDbContext context, IConfiguration configurationManager,
            UserProfileService userProfileService, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _context = context;
            _userProfileService = userProfileService;
            _configuration = configurationManager; 
            _env = env;

        }


        //public IActionResult PAInterview()
        //{
        //    return View();
        //}
        [HttpGet]
        public IActionResult PAInterview()
        {
            var userProfile = this._userManager.GetUserProfile(this.User, this._context);
            var Admission = _context.Admission.Where(p => p.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
            var PAMonthlyTest = _context.PamonthlyTest
                .Where(p => p.CourseId == Admission.CourseId
                && p.TestType == "Video"
                && p.Month == DateTime.Now.Month
                && p.Year == DateTime.Now.Year).FirstOrDefault();
            if (PAMonthlyTest != null)
            {
                ViewBag.MonthlyTestAvailable = true;
                var PACandidateResult = _context.PacandidateResult
            .Where(p => p.MonthlyTestId == PAMonthlyTest.MonthlyTestId && p.CreatedBy == userProfile.LoginId).FirstOrDefault();
                if (PACandidateResult == null)
                {
                    PACandidateResult = new PacandidateResult();
                    PACandidateResult.PacandidateResultId = Guid.NewGuid();
                    PACandidateResult.TestDuration = 1;
                    PACandidateResult.TestAttempt = 1;
                    PACandidateResult.IsActive = true;
                    PACandidateResult.Status = "Pending";
                    PACandidateResult.UserProfileId = userProfile.UserProfileId;
                    PACandidateResult.MonthlyTestId = PAMonthlyTest.MonthlyTestId;
                    PACandidateResult.CreatedDate = DateTime.Now;
                    PACandidateResult.CreatedBy = userProfile.LoginId;
                    _context.PacandidateResult.Add(PACandidateResult);
                    _context.SaveChanges();
                    //PAMonthlyTest.CandidateResultId = PACandidateResult.PacandidateResultId;
                    ViewBag.TestCompleted = false;
                }

                ViewBag.CandidateResultId = PACandidateResult.PacandidateResultId;

                if (PAMonthlyTest.CourseId != null)
                {
                    List<Paquestionnaire> questionnaires = _context.Paquestionnaire.
                        Where(r => r.QuestionnaireType == "Video").
                        Where(r => r.IsActive == true).
                        Where(r => r.CourseId == PAMonthlyTest.CourseId).ToList();

                    List<PavideoQuestionResult> interviewResults = _context.PavideoQuestionResult.
                       Where(r => r.CandidateResultId == PACandidateResult.PacandidateResultId).ToList();

                    for (int i = 0; i < questionnaires.Count; i++)
                    {
                        try
                        {
                            if (interviewResults.Where(e =>
                                    e.PaquestionnaireId == questionnaires[i].PaquestionnaireId).Count() > 0)
                            {

                            }
                            else
                            {
                                questionnaires[i].TotalQuestion = questionnaires.Count.ToString();
                                questionnaires[i].AttemptedQuestion = interviewResults.Count.ToString();
                                return View(questionnaires[i]);
                            }

                        }
                        catch (Exception ex)
                        {
                            questionnaires[i].TotalQuestion = questionnaires.Count.ToString();
                            questionnaires[i].AttemptedQuestion = interviewResults.Count.ToString();
                            return View(questionnaires[i]);
                        }
                    }

                    if (questionnaires.Count == 0)
                    {
                        ViewBag.IsPAInterviewAvailables = false;
                    }

                    if (questionnaires.Count == interviewResults.Count)
                    {
                        ViewBag.IsPAInterviewAvailable = false;
                    }
                    return View(questionnaires.FirstOrDefault());
                }
                else
                {
                    ViewBag.MonthlyTestAvailable = false;
                    return View();
                }
            }
            else
            {
                ViewBag.MonthlyTestAvailable = false;
                //return View();

            }
            return View(PAMonthlyTest);
        }

        [HttpPost("PAUploadVideo/{candidateResultId}")]
        public ActionResult PleaseUploadFile(string candidateResultId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cResult = _context.PacandidateResult.Where
                (e => e.PacandidateResultId.ToString() == candidateResultId
                && e.IsActive == true).FirstOrDefault();
            IFormFileCollection files = Request.Form.Files;
            for (int i = 0; i < files.Count; i++)
            {
                IFormFile file = files[i];
                try
                {
                    string questionaireId = Request.Form["Question_Id"].ToString();
                    string fileName = this.User.Identity.Name + "_" + questionaireId + ".mp4";
                    string videopath = _configuration["AppSettings:PaInterviewVideoPath"];
                    string path = Path.Combine(_env.WebRootPath, videopath);
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        file.CopyTo(stream);
                        UserProfile userProfile = _context.UserProfile.Where(r => r.LoginId == userId).First();
                        //List<InterviewResult> interviews = _db.InterviewResult.Where(r => r.CreatedBy == loginID).ToList();

                        if (_context.PavideoQuestionResult.Where(e =>
                         e.PaquestionnaireId == new Guid(questionaireId)
                         && e.CandidateResultId == new Guid(candidateResultId)).Count() > 0)
                        {
                            PavideoQuestionResult interviewResult = _context.PavideoQuestionResult.Where(e =>
                                    e.PaquestionnaireId == new Guid(questionaireId)
                                    && e.CandidateResultId == new Guid(candidateResultId)).FirstOrDefault();
                            //interviewResult.VideoQuestionResultId = Guid.NewGuid();
                            interviewResult.UserProfileId = userProfile.UserProfileId;
                            interviewResult.VideoAbsolutePath = videopath + fileName;
                            interviewResult.VideoName = fileName;
                            interviewResult.PaquestionnaireId = Guid.Parse(questionaireId);
                            interviewResult.QuestionOrder = 1;
                            interviewResult.CreatedBy = userId;
                            interviewResult.CandidateResultId = new Guid(candidateResultId);
                            _context.PavideoQuestionResult.Update(interviewResult);
                        }
                        else
                        {
                            PavideoQuestionResult interviewResult = new PavideoQuestionResult();
                            interviewResult.VideoQuestionResultId = Guid.NewGuid();
                            interviewResult.UserProfileId = userProfile.UserProfileId;
                            interviewResult.VideoAbsolutePath = videopath + fileName;
                            interviewResult.VideoName = fileName;
                            interviewResult.PaquestionnaireId = Guid.Parse(questionaireId);
                            interviewResult.QuestionOrder = 1;
                            interviewResult.CreatedBy = userId;
                            interviewResult.CandidateResultId = new Guid(candidateResultId);
                            _context.PavideoQuestionResult.Add(interviewResult);
                        }
                        _context.SaveChanges();
                        //saveDataToDb(userId, fileName, path, questionaireId);
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return new JsonResult("success");
        }

        [HttpGet("PAVideoTestSubmittedStudentList")]
        public IActionResult PAVideoTestSubmittedStudentList()
        {

            var userid = this._userManager.GetUserId(this.User);
            var collegeProfileid = this._context.CollegeProfile.Where(u => u.LoginId == userid).FirstOrDefault();
            var completedStudentList =
                 _context.PacandidateResult.Include(e => e.MonthlyTest)

                 .ThenInclude(e => e.Course)
                 .Include(e => e.UserProfile)
                 .ThenInclude( e => e.Admission)
                 .Where(c => c.MonthlyTest.TestType == "Video" && 
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
                     Status = e.Status,
                     Score = 0,
                     PACandidateResultId = e.PacandidateResultId
                 });

            //var completedStudentList = (from c in _context.PacandidateResult
            //                            join m in _context.PamonthlyTest on c.MonthlyTestId equals m.MonthlyTestId
            //                            join a in _context.Admission on m.CourseId equals a.CourseId
            //                            join b in _context.PavideoQuestionResult on a.UserProfileId equals b.UserProfileId

            //                            where m.TestType == "Video"
            //                            && a.CollegeProfileId == collegeProfileid.CollegeProfileId

            //                            select new PATestCompletedStudentListDTO()
            //                            {
            //                                UserProfileId = (Guid)a.UserProfileId,
            //                                FirstName = a.UserProfile.FirstName,
            //                                MiddleName = a.UserProfile.MiddleName,
            //                                LastName = a.UserProfile.LastName,
            //                                CourseName = c.MonthlyTest.Course.Name,
            //                                TestCourseName = c.MonthlyTest.Name,
            //                                Status = c.Status,
            //                                Score = 0,
            //                                PACandidateResultId = c.PacandidateResultId
            //                            }).ToList();


            var list = completedStudentList.ToArray();

            for (int i = 0; i < completedStudentList.Count(); i++)
            {
                list[i].Score = this._context.PavideoQuestionResult.
                    Where(e => e.CandidateResultId == list[i]
                    .PACandidateResultId).Sum(e => (decimal)e.Score);

                //if (!string.IsNullOrWhiteSpace(pavideoQuestionResult.Score.ToString()))
                //{
                //    list[i].Score = int.Parse(pavideoQuestionResult.Score.ToString());
                //}
                //else
                //{
                //    list[i].Score = 0;

                //}

            }


            ViewBag.PATestCompletedStudentList = list;

            return View();
        }
        [HttpGet("VideTestEvaluation/{id}")]
        public IActionResult VideTestEvaluation(Guid id)
        {
            var t = _context.PavideoQuestionResult.Where(e => e.CandidateResultId == id).ToList();
            return View(t);
        }


        [HttpGet("SaveScore/{id}/{feedback}/{score}")]
        public IActionResult SaveScore(string id, string feedback, string score)
        {
            try
            {
                var t = _context.PavideoQuestionResult
                    .Where(e => e.VideoQuestionResultId == new Guid(id)).FirstOrDefault();
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

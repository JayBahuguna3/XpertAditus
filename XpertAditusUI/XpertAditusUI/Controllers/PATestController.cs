using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class PATestController : Controller
    {
        private readonly UserProfileService _userProfileService;
        private readonly TestService _testService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CandidateService _candidateService;
        private readonly XpertAditusDbContext _context;
        private Microsoft.Extensions.Configuration.IConfiguration _configuration;
        protected readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;

        public PATestController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
          UserProfileService userProfileService, TestService testService, CandidateService candidateService, XpertAditusDbContext context,
          Microsoft.Extensions.Configuration.IConfiguration configuration, IMapper mapper)
        {
            _userProfileService = userProfileService;
            _testService = testService;
            _userManager = userManager;
            _candidateService = candidateService;
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
        }

        public IActionResult PATest(string monthlyTestID = "")
        {
            var userProfile = _userProfileService.GetUserProfile(this.User);

            ViewBag.Course = _context.Admission.Where(p => p.UserProfileId == userProfile.UserProfileId).FirstOrDefault().CourseId;
            ViewBag.MonthlyTestId = monthlyTestID;
            ViewBag.TranningInfo = _candidateService.getPATrainingContentsMaster(new Guid(ViewBag.Course.ToString())).
                        Where(e => e.PatestScenario.Where(p => p.NoOfQuestions > 0).Count() > 0).
                        Select(a => new PatrainingContentMaster { Name = a.Name, TrainingContentId = a.TrainingContentId });

            ViewBag.TestType = "Multiple";

            return View();
        }

        public IActionResult StartPATest(string monthlyTestID = "")
        {
            try
            {
                var userProfile = this._userManager.GetUserProfile(this.User, this._context);

                var activeTest = this._context.PacandidateResult
                    .Where(e => e.MonthlyTestId == new Guid(monthlyTestID)
                    && e.UserProfileId == userProfile.UserProfileId && e.IsActive == true)
                    .FirstOrDefault();

                if (activeTest != null && activeTest.Status == "Pending")
                {
                    activeTest.Status = "InProgress";
                    activeTest.TestStarted = DateTime.Now;
                    activeTest.TestDuration = 1;
                    activeTest.TestEnd = activeTest.TestStarted.Value.AddHours((double)activeTest.TestDuration);
                    activeTest.RemainingTime = activeTest.TestStarted;
                    _context.PacandidateResult.Update(activeTest);
                    _context.SaveChanges();

                    var result = _mapper.Map<PacandidateResultDTO>(activeTest);
                    return Ok(new ResponseResult()
                    {
                        Error = false,
                        Model = result
                    });
                }
                else
                {
                    return Ok(new ResponseResult()
                    {
                        Error = true,
                        Message = "Test already started"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                _logger.LogDebug(ex.StackTrace);
                return Ok(ex.Message);
            }
        }

        public IActionResult GetActiveTestInfo(string Courseid = "", string TrainingContentId = "", string monthytestid = "")
        {
            try
            {
                var userProfile = _userProfileService.GetUserProfile(this.User);
                var monthlyTest = _context.PamonthlyTest.Include(e => e.Course).Where(e => e.MonthlyTestId == new Guid(monthytestid)).FirstOrDefault();
                if (Courseid == "" || Courseid == "undefined")
                {
                    //var course = monthlyTest.Course;// _context.CourseMaster.Where(e => e.CourseId == new Guid(Courseid)).FirstOrDefault();
                    //if (course != null)
                    //{
                    //    Courseid = course.CourseId.ToString();
                    //}
                    //_testService.GetCourseByUserCourseId(new Guid(Courseid)).ToString();
                }
                //else
                //{
                //    var Course = _testService.GetPendingUserCourseTest(userProfile.UserProfileId).Course;
                //    courseid = Course.CourseId.ToString();
                //}

                //var userCourses = this._context.UserCourses.Include(e => e.Course)
                //    .Where(e => e.UserProfileId == userProfile.UserProfileId
                //    && e.CourseId.ToString() == courseid
                //    && e.IsActive == true).FirstOrDefault();

                var activeTestt = this._context.PacandidateResult.Where(e =>
                e.MonthlyTestId == monthlyTest.MonthlyTestId
                && e.UserProfileId == userProfile.UserProfileId
                && e.Status == "InProgress" && e.IsActive == true).FirstOrDefault();
                var activeTest = _mapper.Map<PacandidateResultDTO>(activeTestt);
                if (activeTest != null)
                {
                    var totalQuestionCount = _testService.GetPACourseQuestiosCount(new Guid(Courseid), new Guid(TrainingContentId));
                    if (activeTest != null)
                    {
                        var CheckAnswerProvided = _context.PaquestionnaireResult.
                          Where(q => q.CandidateResultId == activeTest.PacandidateResultId && q.TrainingContentId == new Guid(TrainingContentId))
                          .Select(e => new PaquestionnaireResult()
                          {
                              AnswerProvided = e.AnswerProvided,
                              QuestionOrder = e.QuestionOrder
                          }).ToList();
                        //var result = _mapper.Map<PacandidateResultDTO>(activeTest);
                        return Ok(new ResponseResult()
                        {
                            Error = false,
                            Model = activeTest,
                            MaxCount = totalQuestionCount,
                            CheckAnswerProvided = CheckAnswerProvided
                        });
                    }
                    else
                    {
                        return Ok(new ResponseResult()
                        {
                            Error = true,
                            Message = "No active test found.",
                            Id = "1"
                        });
                    }
                }
                else
                {
                    var cRes = this._context.PacandidateResult.Where(e =>
                 e.MonthlyTestId == new Guid(monthytestid) && e.UserProfileId == userProfile.UserProfileId
                 && e.Status == "Complete" && e.IsCleared == true).Count();
                    if (cRes > 0)
                    {
                        return Ok(new ResponseResult()
                        {
                            Error = true,
                            Message = "You already cleared the test for " + monthlyTest.Course.Name + ". Please select a different course",
                            Id = "1"
                        });
                    }


                   // var testAttemptCount = _testService.GetPATestAttempCount(monthlyTest.CourseId, userProfile.UserProfileId);
                    //var CourseLimitations = _configuration["Limitaions:CourseLimitations"].ToString();
                    //if (testAttemptCount < 3)
                    //{
                    //    var cResult = this._context.PacandidateResult.Where(e =>
                    //            e.UserProfileId == userProfile.UserProfileId)
                    //        .OrderByDescending(e => e.CreatedBy).ToList().LastOrDefault();
                    //    CreateNewCandidateResultForNextAttempt(cResult, userProfile);
                    //    return RedirectToPage("/PADisclaimerAccepted/PADisclaimerAccepted");
                    //    ;
                    //}
                    //else
                    //{
                        return Ok(new ResponseResult()
                        {
                            Error = true,
                            Message = "All Test attempts are complete. Please check the score in Dashboard.",
                            Id = "1"
                        });
                    //}

                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                _logger.LogDebug(ex.StackTrace);
                return Ok(ex.Message);
            }
         }

        public IActionResult UpdateRemainingTime(Guid candidateResultId)
        {
            var cResult = _context.PacandidateResult.Where(c => c.PacandidateResultId == candidateResultId).FirstOrDefault();
            if (cResult != null)
            {
                DateTime d1 = cResult.RemainingTime == null ? (DateTime)cResult.TestStarted : (DateTime)cResult.RemainingTime;
                cResult.RemainingTime = d1.AddSeconds(5);
                _context.PacandidateResult.Update(cResult);
                _context.SaveChanges();
                TimeSpan ts = (TimeSpan)(cResult.TestEnd - cResult.RemainingTime);
                if (ts == TimeSpan.Zero)
                {
                    return Ok(new ResponseResult()
                    {
                        Error = false,
                        Message = "Success",
                        Id = "101"
                    });
                }
                else
                {
                    return Ok(new ResponseResult()
                    {
                        Error = false,
                        Message = "Success",
                        Id = "102"
                    });
                }
            }
            else
            {
                return Ok(new ResponseResult()
                {
                    Error = true,
                    Message = "Something Wrong!"
                });
            }
        }

        public IActionResult getPAListOfActiveTestQuestions(string candidateResultId, Guid TrainingContentId)
        {
            var questionList = _context.PaquestionnaireResult.Include(e => e.Paquestionnaire).Where(e =>
             e.CandidateResultId.ToString() == candidateResultId && e.Paquestionnaire.PatrainingContentId == TrainingContentId).Select(e => new
             {
                 QuestionnaireId = e.PaquestionnaireId,
                 QuestionnaireResultId = e.PaquestionnaireResultId,
                 QuestionText = e.Paquestionnaire.QuestionText,
                 Option1 = e.Paquestionnaire.Option1,
                 Option2 = e.Paquestionnaire.Option2,
                 Option3 = e.Paquestionnaire.Option3,
                 Option4 = e.Paquestionnaire.Option4,
                 AnswerProvided = e.AnswerProvided,
                 OptionSelected = e.OptionSelected,
                 QuestionOrder = e.QuestionOrder
             }).OrderBy(e => e.QuestionOrder).ToList();
            return Ok(questionList);
        }

        public async Task<IActionResult> GetPAActiveTestQuestion(Guid TrainingContentId, int questionOrder = -1, string candidateResultId = "", int driectQuestionNo = 0)
        {
            try
            {
                var userId = this._userManager.GetUserId(this.User);
                var userProfile = this._testService.GetUserProfile(userId);
                if (userId == null)
                {
                    return Ok("User is not Logged-in");
                }
                var cResult = _testService.GetPACandidateResult(candidateResultId);
                if (cResult == null)
                {
                    return Ok("Course is not selected!");
                }
                if (cResult.Status == "Pending")
                {
                    return Ok("Test is not started!");
                }
                var Admission = _context.Admission.
                Where(p => p.UserProfileId == userProfile.UserProfileId).FirstOrDefault();

                var activeCourse = _testService.GetPACourseByUserCourseId(Admission.CourseId);

                if (activeCourse == null)
                {
                    return Ok("No Active course found!");
                }
                var randQuestion = _testService.GetPARandomQuestion(activeCourse.CourseId, TrainingContentId);
                if (questionOrder < 0 && driectQuestionNo == 0)
                {
                    //randQuestion = GetRandomQuestion(activeCourse.CourseId);

                    questionOrder = _testService.GetPAQuestionOrder(cResult.PacandidateResultId, TrainingContentId) + 1;
                    var questionCount = _testService.GetPACourseQuestiosCount(activeCourse.CourseId, TrainingContentId);
                    var validationCheck = true;
                    if (questionCount - (questionOrder - 1) <= 0)
                    {
                        return Ok(new ResponseResult()
                        {
                            Error = true,
                            Message = "No more questions!",
                            Id = "101"
                        });
                    }
                    while (validationCheck)
                    {
                        randQuestion = _testService.GetPARandomQuestion(activeCourse.CourseId, TrainingContentId);
                        validationCheck = _testService.ValidatePADuplicateQuestion(randQuestion.PaquestionnaireId, cResult.PacandidateResultId);
                    }

                    PaquestionnaireResult questionnaireResult = new PaquestionnaireResult()
                    {
                        PaquestionnaireResultId = Guid.NewGuid(),
                        CandidateResultId = cResult.PacandidateResultId,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now,
                        PaquestionnaireId = randQuestion.PaquestionnaireId,
                        TrainingContentId = TrainingContentId,
                        //IsCorrectAnswer = Convert.ToBoolean(randQuestion.CorrectAnswer),
                        QuestionOrder = questionOrder
                    };
                    _context.PaquestionnaireResult.Add(questionnaireResult);
                    _context.SaveChanges();
                    return new JsonResult(new
                    {
                        QuestionnaireId = randQuestion.PaquestionnaireId,
                        QuestionnaireResultId = questionnaireResult.PaquestionnaireResultId,
                        QuestionText = randQuestion.QuestionText,
                        Option1 = randQuestion.Option1,
                        Option2 = randQuestion.Option2,
                        Option3 = randQuestion.Option3,
                        Option4 = randQuestion.Option4,
                        AnswerProvided = randQuestion.AnswerProvided,
                        OptionSelected = questionnaireResult.OptionSelected,
                        QuestionOrder = questionnaireResult.QuestionOrder
                        //CourseId = j.CourseId
                    });
                }
                else if (questionOrder < 0 && driectQuestionNo != 0)
                {
                    questionOrder = driectQuestionNo;
                    var questionCount = _testService.GetPACourseQuestiosCount(activeCourse.CourseId, TrainingContentId);
                    var validationCheck = true;
                    if (questionCount - (questionOrder - 1) <= 0)
                    {
                        return Ok(new ResponseResult()
                        {
                            Error = true,
                            Message = "No more questions!",
                            Id = "101"
                        });
                    }
                    while (validationCheck)
                    {
                        randQuestion = _testService.GetPARandomQuestion(activeCourse.CourseId, TrainingContentId);
                        validationCheck = _testService.ValidatePADuplicateQuestion(randQuestion.PaquestionnaireId, cResult.PacandidateResultId);
                    }

                    PaquestionnaireResult questionnaireResult = new PaquestionnaireResult()
                    {
                        PaquestionnaireResultId = Guid.NewGuid(),
                        CandidateResultId = cResult.PacandidateResultId,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now,
                        PaquestionnaireId = randQuestion.PaquestionnaireId,
                        TrainingContentId = TrainingContentId,
                        QuestionOrder = questionOrder
                    };
                    _context.PaquestionnaireResult.Add(questionnaireResult);
                    _context.SaveChanges();
                    return new JsonResult(new
                    {
                        QuestionnaireId = randQuestion.PaquestionnaireId,
                        QuestionnaireResultId = questionnaireResult.PaquestionnaireResultId,
                        QuestionText = randQuestion.QuestionText,
                        Option1 = randQuestion.Option1,
                        Option2 = randQuestion.Option2,
                        Option3 = randQuestion.Option3,
                        Option4 = randQuestion.Option4,
                        AnswerProvided = randQuestion.AnswerProvided,
                        OptionSelected = questionnaireResult.OptionSelected,
                        QuestionOrder = questionnaireResult.QuestionOrder
                    });
                }
                else
                {
                    var qResult = _context.PaquestionnaireResult.Where(e => e.CandidateResultId == cResult.PacandidateResultId
                    && e.QuestionOrder == questionOrder && e.TrainingContentId == TrainingContentId).FirstOrDefault();
                    randQuestion = _context.Paquestionnaire.Where(e => e.PaquestionnaireId == qResult.PaquestionnaireId).FirstOrDefault();
                    randQuestion.AnswerProvided = qResult.AnswerProvided.ToString();
                    return new JsonResult(new
                    {
                        QuestionnaireId = randQuestion.PaquestionnaireId,
                        QuestionnaireResultId = qResult.PaquestionnaireId,
                        QuestionText = randQuestion.QuestionText,
                        Option1 = randQuestion.Option1,
                        Option2 = randQuestion.Option2,
                        Option3 = randQuestion.Option3,
                        Option4 = randQuestion.Option4,
                        AnswerProvided = randQuestion.AnswerProvided,
                        OptionSelected = qResult.OptionSelected,
                        //CorrectAnswer = randQuestion.CorrectAnswer,
                        QuestionOrder = questionOrder
                        //CourseId = j.CourseId
                    });
                }

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        public IActionResult UpdatePAQuestionaireResult(string questionaireResultId, string selectedOption)
        {
            var qResult = _context.PaquestionnaireResult.Include(e => e.Paquestionnaire).Where(e => e.PaquestionnaireResultId.ToString() == questionaireResultId).FirstOrDefault();
            if (qResult != null)
            {
                qResult.OptionSelected = int.Parse(selectedOption);
                qResult.IsCorrectAnswer = qResult.Paquestionnaire.CorrectAnswer == selectedOption;
                qResult.AnswerProvided = true;
                _context.PaquestionnaireResult.Update(qResult);
                _context.SaveChanges();
                return Ok(new ResponseResult()
                {
                    Error = false,
                    Message = "Success"
                });

            }
            else
            {
                return Ok(new ResponseResult()
                {
                    Error = true,
                    Message = "Question not found!"
                });
            }
        }

        public async Task<IActionResult> CompletePATest(string candidateResultId)
        {
            try
            {
                var userProfile = this._userManager.GetUserProfile(this.User, _context);
                var courseId = _context.Admission.Where(p => p.UserProfileId == userProfile.UserProfileId).FirstOrDefault().CourseId;
                var cResult = _context.PacandidateResult.Where(e => e.PacandidateResultId.ToString() == candidateResultId && e.IsActive == true).FirstOrDefault();
                var PassingCriteria = _configuration["AppSettings:PassingCriteria"].ToString();
                var CourseLimitations = _configuration["Limitaions:CourseLimitations"].ToString();
                var score = _testService.GetPAScore(cResult.PacandidateResultId, courseId, PassingCriteria);

                //var totalQuestion = _context.QuestionnaireResult.Where(e => e.CandidateResultId.ToString() == candidateResultId).Count();
                // var correctQ = _context.QuestionnaireResult.Where(e => e.CandidateResultId.ToString() == candidateResultId && e.IsCorrectAnswer == true).Count();
                cResult.Score = score.Score;

                cResult.Status = "Complete";
                cResult.IsActive = true;
                cResult.IsCompleted = true;
                cResult.TestEnd = DateTime.Now;
                if (cResult.TestAttempt == 0)
                    cResult.TestAttempt = 1;

                if (cResult.Score >= decimal.Parse(PassingCriteria))
                {
                    //cResult.UserCourses.IsActive = false;
                    cResult.UserProfileId = userProfile.UserProfileId;
                    cResult.IsCleared = true;
                    _context.PacandidateResult.Update(cResult);
                    _context.SaveChanges();
                    return Ok(new ResponseResult()
                    {
                        Error = false,
                        Message = "Congratulation you cleared the Test!"
                    });
                }
                else
                {
                    //if (cResult.TestAttempt != (int.Parse(CourseLimitations)))
                    //{
                    //    // Deactivate previous Test
                    //    //CreateNewCandidateResultForNextAttempt(cResult, userProfile);
                    //    return Ok(new ResponseResult()
                    //    {
                    //        Error = false,
                    //        Message = "You have failed the Test! "// + (int.Parse(CourseLimitations) - (int)cResult.TestAttempt) + " attempt remaining."
                    //    });
                    //}
                    //else
                    //{
                        //cResult.UserCourses.IsActive = false;
                        _context.PacandidateResult.Update(cResult);
                        _context.SaveChanges();
                        return Ok(new ResponseResult()
                        {
                            Error = false,
                            Message = "You have failed the Test!"// And no more attempts are remaining, Please do the payment again for reattempt."
                        });
                    //}
                }
            }
            catch (Exception ex)
            {
                return Ok(new ResponseResult()
                {
                    Error = false,
                    Message = ex.Message
                });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public void CreateNewCandidateResultForNextAttempt(PacandidateResult cResult, UserProfile userProfile)
        {

            cResult.IsActive = false;
            cResult.Status = "Complete";
            _context.PacandidateResult.Update(cResult);

            //Deactivate previous DisclaimerAccepted
            var disclaimerAccetped = _context.PadisclaimerAccetped.Where(d => d.UserProfileId == userProfile.UserProfileId
               && d.IsAccepted == true).FirstOrDefault();

            disclaimerAccetped.IsAccepted = false;
            _context.PadisclaimerAccetped.Update(disclaimerAccetped);

            PacandidateResult candidateResult = new PacandidateResult()
            {
                PacandidateResultId = Guid.NewGuid(),
                UserProfileId = userProfile.UserProfileId,
                TestDuration = cResult.TestDuration,
                IsCompleted = false,
                IsCleared = false,
                TestAttempt = cResult.TestAttempt + 1,
                IsActive = true,
                Status = "Pending",

            };
            _context.PacandidateResult.Add(candidateResult);
            _context.SaveChanges();
        }


    }
}

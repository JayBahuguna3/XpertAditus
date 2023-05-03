using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.DTO;
using XpertAditusUI.Models;
using XpertAditusUI.Service;
using Microsoft.Extensions.Configuration;

namespace XpertAditusUI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TestController : BaseController
    {
        private readonly XpertAditusDbContext _context;
        private readonly TestService _testService;
        private readonly CandidateService _candidateService;
        private readonly UserProfileService _userProfileService;
        private readonly IMapper _mapper;
        private IConfiguration _configuration;

        public TestController(XpertAditusDbContext context, TestService testService, IConfiguration configurationManager,
            ILogger<HomeController> logger, UserProfileService userProfileService, UserManager<IdentityUser> userManager, IMapper mapper, CandidateService candidateService)
            : base(logger, userManager)
        {
            _context = context;
            _userProfileService = userProfileService;
            _testService = testService;
            _mapper = mapper;
            _configuration = configurationManager;
            _candidateService = candidateService;
        }
        [HttpGet("Test")]
        public IActionResult Test()
        {
            var userProfile = _userProfileService.GetUserProfile(this.User);

            ViewBag.Course = _testService.GetPendingUserCourseTest(userProfile.UserProfileId).Course;
            //ViewBag.Course =
            //   _testService.GetActiveUserCourse(_userProfileService.GetUserInfo(this._userManager.GetUserId(this.User)).UserProfileId);

            ViewBag.TestType = _candidateService.GetCourseInfo(ViewBag.Course.CourseId.ToString()).Type;

            ViewBag.TranningInfo = _candidateService.getTrainingContentsMaster(new Guid(ViewBag.Course.CourseId.ToString())).
            Select(a => new TrainingContentsMaster { Name = a.Name, TrainingContentId = a.TrainingContentId });

            return View();
        }
        [HttpGet("StartTest/{courseid}")]
        public IActionResult StartTest(string courseid)
        {
            try
            {
                var userProfile = this._userManager.GetUserProfile(this.User, this._context);
                var userCourses = _testService.GetPendingUserCourseTest(userProfile.UserProfileId);

                //var userCourses = this._context.UserCourses.Where(e => e.UserProfileId == userProfile.UserProfileId 
                //&& e.CourseId.ToString() == courseid
                //&& e.IsActive == true).FirstOrDefault();
                var activeTest = this._context.CandidateResult.Where(e => e.UserCoursesId == userCourses.UserCoursesId && e.IsActive == true).FirstOrDefault();

                if (activeTest.Status == "Pending")
                {
                    activeTest.Status = "InProgress";
                    activeTest.TestStarted = DateTime.Now;
                    activeTest.TestDuration = 1;
                    activeTest.TestEnd = activeTest.TestStarted.Value.AddHours((double)activeTest.TestDuration);
                    activeTest.RemainingTime = activeTest.TestStarted;
                    _context.CandidateResult.Update(activeTest);
                    _context.SaveChanges();

                    var result = _mapper.Map<CandidateResultDTO>(activeTest);
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

        [HttpGet("GetActiveTestInfo/{courseid}/{TrainingContentId}")]
        public IActionResult GetActiveTestInfo(string courseid="", string TrainingContentId = "")
        {
            try
            {
                var userProfile = _userProfileService.GetUserProfile(this.User);
                if (courseid == "" || courseid == "undefined")
                {
                    courseid =
                        _testService.GetCourseByUserCourseId(new Guid(courseid)).ToString();
                }
                else
                {
                    var Course = _testService.GetPendingUserCourseTest(userProfile.UserProfileId).Course;
                    courseid = Course.CourseId.ToString();
                }

                var userCourses = this._context.UserCourses.Include(e =>e.Course)
                    .Where(e => e.UserProfileId == userProfile.UserProfileId 
                    && e.CourseId.ToString() == courseid
                    && e.IsActive == true).FirstOrDefault();

                var activeTest = this._context.CandidateResult.Where(e =>
                e.UserCoursesId == userCourses.UserCoursesId
                && e.Status == "InProgress" && e.IsActive == true).FirstOrDefault();

                if (activeTest != null)
                {
                    var totalQuestionCount = _testService.GetCourseQuestiosCount(userCourses.CourseId, new Guid(TrainingContentId));
                    if (activeTest != null)
                    {
                        var CheckAnswerProvided = _context.QuestionnaireResult.
                      Where(q => q.CandidateResultId == activeTest.CandidateResultId && q.TrainingContentId ==  new Guid (TrainingContentId))
                      .Select(e => new QuestionnaireResult()
                      {
                          AnswerProvided = e.AnswerProvided,
                          QuestionOrder = e.QuestionOrder
                      }).ToList();

                        var result = _mapper.Map<CandidateResultDTO>(activeTest);


                        return Ok(new ResponseResult()
                        {
                            Error = false,
                            Model = result,
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
                  var cRes =  this._context.CandidateResult.Where(e =>
                e.UserCoursesId == userCourses.UserCoursesId
                && e.Status == "Complete" && e.IsCleared == true).Count(); 
                    if(cRes > 0)
                    {
                        return Ok(new ResponseResult()
                        {
                            Error = true,
                            Message = "You already cleared the test for "+ userCourses .Course.Name + ". Please select a different course",
                            Id = "1"
                        });
                    }


                    var testAttemptCount = _testService.GetTestAttempCount(userCourses.CourseId, userProfile.UserProfileId);
                    var CourseLimitations = _configuration["Limitaions:CourseLimitations"].ToString();
                    if (testAttemptCount < int.Parse(CourseLimitations))
                    {
                        var cResult = this._context.CandidateResult.Where(e =>
                                e.UserCoursesId == userCourses.UserCoursesId)
                            .OrderByDescending(e => e.CreatedBy).ToList().LastOrDefault();
                        CreateNewCandidateResultForNextAttempt(cResult, userProfile);
                        return RedirectToPage("/DisclaimerAccepted/DisclaimerAccepted");
                        ;
                    }
                    else
                    {
                        return Ok(new ResponseResult()
                        {
                            Error = true,
                            Message = "All Test attempts are complete. Please Select the course again and do the payment.",
                            Id = "1"
                        });
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                _logger.LogDebug(ex.StackTrace);
                return Ok(ex.Message);
            }
        }

        [HttpGet("GetListOfActiveTestQuestions")]

        public IActionResult GetListOfActiveTestQuestions(string candidateResultId, Guid TrainingContentId)
            {
            var questionList = _context.QuestionnaireResult.Include(e => e.Questionnaire).Where(e =>
             e.CandidateResultId.ToString() == candidateResultId && e.Questionnaire.TrainingContentId == TrainingContentId).Select(e => new
             {
                 QuestionnaireId = e.QuestionnaireId,
                 QuestionnaireResultId = e.QuestionnaireResultId,
                 QuestionText = e.Questionnaire.QuestionText,
                 Option1 = e.Questionnaire.Option1,
                 Option2 = e.Questionnaire.Option2,
                 Option3 = e.Questionnaire.Option3,
                 Option4 = e.Questionnaire.Option4,
                 AnswerProvided = e.AnswerProvided,
                 OptionSelected = e.OptionSelected,
                 QuestionOrder = e.QuestionOrder
             }).OrderBy(e => e.QuestionOrder).ToList();
            return Ok(questionList);
        }

        [HttpPost("UpdateQuestionaireResult")]
        public IActionResult UpdateQuestionaireResult(string questionaireResultId, string selectedOption)
        {
            var qResult = _context.QuestionnaireResult.Include(e => e.Questionnaire).Where(e => e.QuestionnaireResultId.ToString() == questionaireResultId).FirstOrDefault();
            if (qResult != null)
            {
                qResult.OptionSelected = int.Parse(selectedOption);
                qResult.IsCorrectAnswer = qResult.Questionnaire.CorrectAnswer == selectedOption;
                qResult.AnswerProvided = true;
                _context.QuestionnaireResult.Update(qResult);
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

        [HttpPost("UpdateRemainingTime")]
        public IActionResult UpdateRemainingTime(Guid candidateResultId)
        {
            var cResult = _context.CandidateResult.Where(c => c.CandidateResultId == candidateResultId).FirstOrDefault();
            if (cResult != null)
            {
                DateTime d1 = cResult.RemainingTime == null ? (DateTime)cResult.TestStarted : (DateTime)cResult.RemainingTime;
                cResult.RemainingTime = d1.AddSeconds(5);
                _context.CandidateResult.Update(cResult);
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

        [HttpGet("GetActiveTestQuestion")]
        public async Task<IActionResult> GetActiveTestQuestion(Guid TrainingContentId, int questionOrder = -1, string candidateResultId = "", int driectQuestionNo = 0)
        {
            try
            {
                var userId = this._userManager.GetUserId(this.User);
                var userProfile = this._testService.GetUserProfile(userId);
                if (userId == null)
                {
                    return Ok("User is not Logged-in");
                }
                var cResult = _testService.GetCandidateResult(candidateResultId);
                if (cResult == null)
                {
                    return Ok("Course is not selected!");
                }
                if (cResult.Status == "Pending")
                {
                    return Ok("Test is not started!");
                }
                var activeCourse = _testService.GetCourseByUserCourseId(cResult.UserCourses.CourseId);
                if (activeCourse == null)
                {
                    return Ok("No Active course found!");
                }
                var randQuestion = _testService.GetRandomQuestion(activeCourse.CourseId, TrainingContentId);
                if (questionOrder < 0 && driectQuestionNo == 0)
                {
                    //randQuestion = GetRandomQuestion(activeCourse.CourseId);

                    questionOrder = _testService.GetQuestionOrder(cResult.CandidateResultId , TrainingContentId) + 1;
                    var questionCount = _testService.GetCourseQuestiosCount(activeCourse.CourseId, TrainingContentId);
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
                        randQuestion = _testService.GetRandomQuestion(activeCourse.CourseId, TrainingContentId);
                        validationCheck = _testService.ValidateDuplicateQuestion(randQuestion.QuestionnaireId, cResult.CandidateResultId);
                    }
                    QuestionnaireResult questionnaireResult = new QuestionnaireResult()
                    {
                        //QuestionnaireResultId = new Guid(),
                        QuestionnaireResultId = Guid.NewGuid(),
                        CandidateResultId = cResult.CandidateResultId,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now,
                        QuestionnaireId = randQuestion.QuestionnaireId,
                        TrainingContentId = TrainingContentId,
                        //IsCorrectAnswer = Convert.ToBoolean(randQuestion.CorrectAnswer),
                        QuestionOrder = questionOrder
                    };
                    _context.QuestionnaireResult.Add(questionnaireResult);
                    _context.SaveChanges();
                    return new JsonResult(new
                    {
                        QuestionnaireId = randQuestion.QuestionnaireId,
                        QuestionnaireResultId = questionnaireResult.QuestionnaireResultId,
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
                else if(questionOrder < 0 && driectQuestionNo != 0)
                {
                    questionOrder = driectQuestionNo;
                    var questionCount = _testService.GetCourseQuestiosCount(activeCourse.CourseId, TrainingContentId);
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
                        randQuestion = _testService.GetRandomQuestion(activeCourse.CourseId, TrainingContentId);
                        validationCheck = _testService.ValidateDuplicateQuestion(randQuestion.QuestionnaireId, cResult.CandidateResultId);
                    }
                    QuestionnaireResult questionnaireResult = new QuestionnaireResult()
                    {
                        QuestionnaireResultId = Guid.NewGuid(),
                        CandidateResultId = cResult.CandidateResultId,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now,
                        QuestionnaireId = randQuestion.QuestionnaireId,
                        TrainingContentId = TrainingContentId,
                        QuestionOrder = questionOrder
                    };
                    _context.QuestionnaireResult.Add(questionnaireResult);
                    _context.SaveChanges();
                    return new JsonResult(new
                    {
                        QuestionnaireId = randQuestion.QuestionnaireId,
                        QuestionnaireResultId = questionnaireResult.QuestionnaireResultId,
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
                    var qResult = _context.QuestionnaireResult.Where(e => e.CandidateResultId == cResult.CandidateResultId
                    && e.QuestionOrder == questionOrder && e.TrainingContentId == TrainingContentId).FirstOrDefault();
                    randQuestion = _context.Questionnaire.Where(e => e.QuestionnaireId == qResult.QuestionnaireId).FirstOrDefault();
                    randQuestion.AnswerProvided = qResult.AnswerProvided.ToString();
                    return new JsonResult(new
                    {
                        QuestionnaireId = randQuestion.QuestionnaireId,
                        QuestionnaireResultId = qResult.QuestionnaireResultId,
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

        [HttpGet("CompleteTest")]
        public async Task<IActionResult> CompleteTest(string candidateResultId)
        {
            try
            {
                var userProfile = this._userManager.GetUserProfile(this.User, _context);
                var cResult = _context.CandidateResult.Include(e => e.UserCourses).Where(e => e.CandidateResultId.ToString() == candidateResultId && e.IsActive == true).FirstOrDefault();
                var PassingCriteria = _configuration["AppSettings:PassingCriteria"].ToString();
                var CourseLimitations = _configuration["Limitaions:CourseLimitations"].ToString();
                var score = _testService.GetPreAdmissionScore(cResult.CandidateResultId, cResult.UserCourses.CourseId, PassingCriteria);

                //var totalQuestion = _context.QuestionnaireResult.Where(e => e.CandidateResultId.ToString() == candidateResultId).Count();
               // var correctQ = _context.QuestionnaireResult.Where(e => e.CandidateResultId.ToString() == candidateResultId && e.IsCorrectAnswer == true).Count();
                cResult.Score = score.Score;

                cResult.Status = "Complete";
                cResult.IsActive = true;
                cResult.IsCompleted = true;
                cResult.TestEnd = DateTime.Now;
                if(cResult.TestAttempt == 0)
                  cResult.TestAttempt = 1;

                if (cResult.Score >= decimal.Parse(PassingCriteria))
                {
                    //cResult.UserCourses.IsActive = false;
                    cResult.IsCleared = true;
                    _context.CandidateResult.Update(cResult);
                    _context.SaveChanges();
                    return Ok(new ResponseResult()
                    {
                        Error = false,
                        Message = "Congratulation you cleared the Test!"
                    });
                }
                else
                {
                    if (cResult.TestAttempt != (int.Parse(CourseLimitations)))
                    {
                        // Deactivate previous Test
                        CreateNewCandidateResultForNextAttempt(cResult, userProfile);
                        return Ok(new ResponseResult()
                        {
                            Error = false,
                            Message = "You have failed the Test! Please Try again. " + (int.Parse(CourseLimitations) - (int)cResult.TestAttempt) + " attempt remaining."
                        });
                    }
                    else
                    {
                        //cResult.UserCourses.IsActive = false;
                        _context.CandidateResult.Update(cResult);
                        _context.SaveChanges();
                        return Ok(new ResponseResult()
                        {
                            Error = false,
                            Message = "You have failed the Test! And no more attempts are remaining, Please do the payment again for reattempt."
                        });
                    }
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
        public void CreateNewCandidateResultForNextAttempt(CandidateResult cResult, UserProfile userProfile)
        {
            cResult.IsActive = false;
            cResult.Status = "Complete";
            _context.CandidateResult.Update(cResult);

            //Deactivate previous DisclaimerAccepted
            var disclaimerAccetped = _context.DisclaimerAccetped.Where(d => d.UserProfileId == userProfile.UserProfileId
               && d.IsAccepted == true).FirstOrDefault();

            disclaimerAccetped.IsAccepted = false;
            _context.DisclaimerAccetped.Update(disclaimerAccetped);

            CandidateResult candidateResult = new CandidateResult()
            {
                CandidateResultId = Guid.NewGuid(),
                UserCoursesId = cResult.UserCoursesId,
                TestDuration = cResult.TestDuration,
                IsCompleted = false,
                IsCleared = false,
                PaymentHistoryId = cResult.PaymentHistoryId,
                TestAttempt = cResult.TestAttempt + 1,
                IsActive = true,
                Status = "Pending"

            };
            _context.CandidateResult.Add(candidateResult);
            _context.SaveChanges();
        }

        [HttpGet("GetTestScore")]
        public async Task<IActionResult> GetTestScore(string candidateResultId)
        {
            try
            {
                var cResult = _context.CandidateResult.Where(e => e.CandidateResultId.ToString() == candidateResultId).FirstOrDefault();

                return Ok(new ResponseResult()
                {
                    Error = false,
                    Model = cResult
                });

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

    }
}

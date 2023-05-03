using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.Models;

namespace XpertAditusUI.Service
{
    public class TestService
    {
        private IConfiguration _configuration;
        private XpertAditusDbContext _XpertAditusDbContext;
        public TestService(IConfiguration Configuration, XpertAditusDbContext XpertAditusDbContext)
        {
            _configuration = Configuration;
            _XpertAditusDbContext = XpertAditusDbContext;
        }

        public UserProfile GetUserProfile(string userid)
        {
            return _XpertAditusDbContext.UserProfile.Where(u => u.LoginId == userid).FirstOrDefault();
        }
        public Course GetCourseByUserCourseId(Guid? courseid)
        {
            return _XpertAditusDbContext.Course.Where(e => e.CourseId == courseid).FirstOrDefault();
        }
        public UserCourses GetPendingUserCourseTest(Guid? UserProfileID)
        {
            var PendingCourse = _XpertAditusDbContext.UserCourses.Include(e => e.Course)
                .Include(e => e.CandidateResult)
                .Where(e => e.UserProfileId == UserProfileID && e.IsActive == true
                && (e.CandidateResult.Count() == 0 || 
                (e.CandidateResult.Where(d => d.IsCleared == true).Count() == 0 
                    && e.CandidateResult.Where(d => d.IsActive == true && d.IsCleared == false).Count() > 0)))
                             .FirstOrDefault();
            return PendingCourse;
        }
        public UserCourses GetUserCourses(Guid? UserProfileID)
        {
            return _XpertAditusDbContext.UserCourses.Where(e => e.UserProfileId == UserProfileID && e.IsActive == true)
                             .FirstOrDefault();

        }
        public CandidateResult GetCandidateResult(string candidateResultId)
        {
            return _XpertAditusDbContext.CandidateResult.Include(e => e.UserCourses)
                .Where(e => e.CandidateResultId.ToString() == candidateResultId).FirstOrDefault();

        }
        public Questionnaire GetRandomQuestion(Guid? courseguid, Guid TrainingContentId)
        {
            Random rand = new Random();
            //int toSkip = rand.Next(0, _context.Questionnaire.Where(e => e.CourseId == courseguid).Count());
            int toSkip = rand.Next(0, _XpertAditusDbContext.Questionnaire.Where(e => e.CourseId == courseguid && e.TrainingContentId == TrainingContentId).Count());
            return _XpertAditusDbContext.Questionnaire.Where(e => e.CourseId == courseguid && e.TrainingContentId == TrainingContentId).Skip(toSkip).FirstOrDefault();
        }
        public dynamic GetScore(Guid candidateResultId, Guid? courseId, string PassingCriteria)
        {
            int totalQuestion = 0;
            List<TestScenario> testScenarios = _XpertAditusDbContext.TestScenario.Where(e => e.CourseId == courseId).ToList();
            for(int i = 0; i < testScenarios.Count; i++)
            {
                totalQuestion = totalQuestion + _XpertAditusDbContext.Questionnaire.Where(r => r.TrainingContentId == testScenarios[i].TrainingContentsId).Count();
            }
            var correctQ = _XpertAditusDbContext.QuestionnaireResult.Where(e => e.CandidateResultId == candidateResultId && e.IsCorrectAnswer == true).Count();
            return new
            {
                Score = correctQ > 0 ? Math.Round((decimal)((decimal)((double)correctQ / totalQuestion) * 100),2) : 0,
                CorrectQuestion = correctQ,
                IncorrectQuestion = totalQuestion - correctQ,
                TotalQuestion = totalQuestion,
                Pass = correctQ > 0 ? ((decimal)((double)correctQ / totalQuestion) * 100) > decimal.Parse(PassingCriteria) : false
            };
        }
        public dynamic GetPreAdmissionScore(Guid candidateResultId, Guid? courseId, string PassingCriteria)
        {
            int totalQuestion = 0;
            List<TestScenario> testScenarios = _XpertAditusDbContext.TestScenario.Where(e => e.CourseId == courseId).ToList();
            for (int i = 0; i < testScenarios.Count; i++)
            {
                totalQuestion = totalQuestion + testScenarios[i].NoOfQuestions.Value;
            }
            //totalQuestion = testScenarios.Count;
            var correctQ = _XpertAditusDbContext.QuestionnaireResult.Where(e => e.CandidateResultId == candidateResultId && e.IsCorrectAnswer == true).Count();
            return new
            {
                Score = correctQ > 0 ? Math.Round((decimal)((decimal)((double)correctQ / totalQuestion) * 100), 2) : 0,
                CorrectQuestion = correctQ,
                IncorrectQuestion = totalQuestion - correctQ,
                TotalQuestion = totalQuestion,
                Pass = correctQ > 0 ? ((decimal)((double)correctQ / totalQuestion) * 100) > decimal.Parse(PassingCriteria) : false
            };
        }
        public dynamic GetPAScore(Guid candidateResultId, Guid? courseId, string PassingCriteria)
        {
            int totalQuestion = 0;
            List<PatestScenario> testScenarios = _XpertAditusDbContext.PatestScenario
                .Where(e => e.CourseId == courseId).ToList();
            for (int i = 0; i < testScenarios.Count; i++)
            {
                totalQuestion = totalQuestion + testScenarios[i].NoOfQuestions.Value;
                //totalQuestion = totalQuestion + _XpertAditusDbContext
                //    .Paquestionnaire.Where(r => r.PatrainingContentId == 
                //    testScenarios[i].TrainingContentsId).Count();
            }
            var correctQ = _XpertAditusDbContext.
                PaquestionnaireResult.Where(e => e.CandidateResultId == candidateResultId 
                && e.IsCorrectAnswer == true).Count();
            return new
            {
                Score = correctQ > 0 ? Math.Round((decimal)((decimal)((double)correctQ / totalQuestion) * 100), 2) : 0,
                CorrectQuestion = correctQ,
                IncorrectQuestion = totalQuestion - correctQ,
                TotalQuestion = totalQuestion,
                Pass = correctQ > 0 ? ((decimal)((double)correctQ / totalQuestion) * 100) > decimal.Parse(PassingCriteria) : false
            };
        }

        public bool ValidateDuplicateQuestion(Guid QuestionnaireId, Guid candidateResultId)
        {
            return
            _XpertAditusDbContext.QuestionnaireResult.Where(e => e.QuestionnaireId == QuestionnaireId
            && e.CandidateResultId == candidateResultId).Count() > 0;
        }
        public int GetQuestionOrder(Guid? guid, Guid TrainingContentId)
        {
            return _XpertAditusDbContext.QuestionnaireResult.Where(e => e.CandidateResultId == guid && e.TrainingContentId == TrainingContentId).Count();
        }
        public int GetCourseQuestiosCount(Guid? guid, Guid TrainingContentId)
        {
            //return _XpertAditusDbContext.Questionnaire.Where(e => e.CourseId == guid).Count();
            return (int)_XpertAditusDbContext.TestScenario.
                Where(e => e.CourseId == guid && e.TrainingContentsId == TrainingContentId)
                .Select(e => e.NoOfQuestions).FirstOrDefault();
        }

        public int GetPACourseQuestiosCount(Guid? guid, Guid TrainingContentId)
        {
            return (int)_XpertAditusDbContext.PatestScenario.
                Where(e => e.CourseId == guid && e.TrainingContentsId == TrainingContentId)
                .Select(e => e.NoOfQuestions).FirstOrDefault();
        }

        public int GetTestAttempCount(Guid? courseId, Guid? userProfileId)
        {
            return _XpertAditusDbContext.CandidateResult.Include(e => e.UserCourses)
                .Where(e => e.UserCourses.CourseId == courseId && e.UserCourses.UserProfileId == userProfileId
                && e.IsCompleted == true)
                .Count();
        }

        public int GetPATestAttempCount(Guid? courseId, Guid? userProfileId)
        {
            return _XpertAditusDbContext.PacandidateResult.Include(e => e.MonthlyTest)
               .Where(e => e.UserProfileId == userProfileId && e.MonthlyTest.CourseId == courseId  && e.IsCompleted == true)
                .Count();
        }

        private bool QuestionnaireExists(Guid id)
        {
            return _XpertAditusDbContext.Questionnaire.Any(e => e.QuestionnaireId == id);
        }

        public PacandidateResult GetPACandidateResult(string candidateResultId)
        {
            return _XpertAditusDbContext.PacandidateResult
                .Where(e => e.PacandidateResultId.ToString() == candidateResultId).FirstOrDefault();

        }

        public CourseMaster GetPACourseByUserCourseId(Guid? courseid)
        {
            return _XpertAditusDbContext.CourseMaster.Where(e => e.CourseId == courseid).FirstOrDefault();
        }

        public Paquestionnaire GetPARandomQuestion(Guid? courseguid, Guid TrainingContentId)
        {
            Random rand = new Random();
            //int toSkip = rand.Next(0, _context.Questionnaire.Where(e => e.CourseId == courseguid).Count());
            int toSkip = rand.Next(0, _XpertAditusDbContext.Paquestionnaire
                .Where(e => e.CourseId == courseguid && e.QuestionnaireType != "Video"
                && e.PatrainingContentId == TrainingContentId).Count());
            return _XpertAditusDbContext.Paquestionnaire.Where(e => e.CourseId == courseguid
            && e.QuestionnaireType != "Video"
            && e.PatrainingContentId == TrainingContentId).Skip(toSkip).FirstOrDefault();
        }

        public bool ValidatePADuplicateQuestion(Guid QuestionnaireId, Guid candidateResultId)
        {
            return
            _XpertAditusDbContext.PaquestionnaireResult.Where(e => e.PaquestionnaireId == QuestionnaireId
            && e.CandidateResultId == candidateResultId).Count() > 0;
        }

        public int GetPAQuestionOrder(Guid? guid, Guid TrainingContentId)
        {
            return _XpertAditusDbContext.PaquestionnaireResult.
                Where(e => e.CandidateResultId == guid && e.TrainingContentId == TrainingContentId).Count();
        }

        

    }
}

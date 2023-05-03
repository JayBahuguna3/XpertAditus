using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using XpertAditusUI.Data;
using XpertAditusUI.Models;

namespace XpertAditusUI.Service
{
    public class CandidateService
    {
        private XpertAditusDbContext _XpertAditusDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        public CandidateService(UserManager<IdentityUser> userManager, XpertAditusDbContext XpertAditusDbContext)
        {
            _XpertAditusDbContext = XpertAditusDbContext;
            _userManager = userManager;
        }
        public UserProfile GetUserProfile(ClaimsPrincipal user)
        {
            return _XpertAditusDbContext.UserProfile.
                Where(r => r.LoginId == this._userManager.GetUserId(user).ToString()).First();
        }
        public UserCourses GetActiveUserCourses(UserProfile userProfile)
        {
            return _XpertAditusDbContext.UserCourses.Where(r => r.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
        }
        public List<Course> GetAllUserCourses(UserProfile userProfile)
        {
            return _XpertAditusDbContext.Course.Include(e => e.UserCourses).
                Where(r => r.UserCourses.Where(e => e.UserProfileId == userProfile.UserProfileId).Count() > 0)
                .ToList();
        }
        public List<MyCourseModel> GetAllUserCoursesWithTrainingStatus(UserProfile userProfile)
        {
            var CourseList = _XpertAditusDbContext.Course.Include(e => e.UserCourses).
                Where(r => r.UserCourses.Where(e => e.UserProfileId == userProfile.UserProfileId
                && e.IsActive == true).Count() > 0)
                .ToList();
            List<MyCourseModel> result = new List<MyCourseModel>();
            foreach (var item in CourseList)
            {
                var UserCourse = _XpertAditusDbContext.UserCourses.Where(x => x.CourseId == item.CourseId
                    && x.UserProfileId == userProfile.UserProfileId && x.IsActive == true).FirstOrDefault();
                var CourseTrainings = _XpertAditusDbContext.TrainingContentsMaster
                    .Where(x => x.CourseId == item.CourseId).Count();
                var CheckTraining = 0;
                if (UserCourse != null)
                {
                    CheckTraining = _XpertAditusDbContext.CandidateTrainingStatus
                        .Where(c => c.UserCoursesId == UserCourse.UserCoursesId).Count();
                }
                result.Add(new MyCourseModel()
                {
                    Course = item,
                    IsTrainingComplete = CourseTrainings == CheckTraining
                });
            }
            return result;
        }

        public Course GetActiveUserCourse(UserCourses userCourse)
        {
            if (userCourse != null)
                return _XpertAditusDbContext.Course.Where(e => e.CourseId == userCourse.CourseId).FirstOrDefault();
            else
                return null;

        }

        public bool IsTrainingComplete(UserCourses userCourse)
        {
            if (userCourse != null)
            {
                var count = _XpertAditusDbContext.TrainingContentsMaster
                     .Include(e => e.CandidateTrainingStatus.Where(e => e.UserCoursesId == userCourse.UserCoursesId))
                         .Where(e => e.CourseId == userCourse.CourseId && e.IsActive == "True" && e.CandidateTrainingStatus != null).Count();
                //TODO
                return true;
            }
            else
            {
                return false;
            }

        }

        public Course GetCourseInfo(string CourseId)
        {
            return _XpertAditusDbContext.Course.Where(c => c.CourseId ==  new Guid(CourseId)).FirstOrDefault();
        }

        public List<Course> GetCourses()
        {
            return _XpertAditusDbContext.Course.Where(a => a.IsActive == "True").ToList();
        }

        public bool CheckNoOfQuestion(int NoOfQuestion, Guid CourseId, Guid TrainingContentId)
        {
            bool Result = false;

            int QuestionCount = _XpertAditusDbContext.Questionnaire.
                Where(a => a.CourseId == CourseId && a.TrainingContentId == TrainingContentId && a.IsActive == "True").Count();

            if (QuestionCount < NoOfQuestion)
            {
                Result = false;
            }
            else
            {
                Result = true;
            }

            return Result;
        }

        public List<TrainingContentsMaster> getTrainingContentsMaster(Guid Course)
        {
            return _XpertAditusDbContext.TrainingContentsMaster.Where(a => a.CourseId == Course && a.IsActive == "True").ToList();
        }

        public List<PatrainingContentMaster> getPATrainingContentsMaster(Guid Course)
        {
            return _XpertAditusDbContext.PatrainingContentMaster.Include(e => e.PatestScenario).Where(a => a.CourseId == Course && a.IsActive == true).ToList();
        }

        public void SaveTrainingContents(int NoOfQuestion, Guid CourseId, Guid TrainingContentId, string id)
        {
            var result = _XpertAditusDbContext.TestScenario.Where(a => a.IsActive == true && a.CourseId == CourseId && a.TrainingContentsId == TrainingContentId).FirstOrDefault();

            if (result == null)
            {
                TestScenario testScenario = new TestScenario();
                testScenario.Id = Guid.NewGuid();
                testScenario.CourseId = CourseId;
                testScenario.NoOfQuestions = NoOfQuestion;
                testScenario.TrainingContentsId = TrainingContentId;
                testScenario.IsActive = true;
                testScenario.CreatedDate = DateTime.Now;
                testScenario.CreatedBy = id;
                _XpertAditusDbContext.Add(testScenario);
                _XpertAditusDbContext.SaveChanges();
            }
            else
            {
                result.NoOfQuestions = NoOfQuestion;
                result.UpdatedDate = DateTime.Now;
                result.UpdatedBy = id;
                _XpertAditusDbContext.SaveChanges();
            }
        }
    }
}

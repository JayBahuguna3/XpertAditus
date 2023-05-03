using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.Models;
using XpertAditusUI.Service;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace XpertAditusUI.Controllers
{
    [Route("[controller]")]
    public class InterviewController : CandidateController
    {
        private readonly XpertAditusDbContext _db;
        private readonly IWebHostEnvironment _env;
        private IConfiguration _configuration;
        public InterviewController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
            XpertAditusDbContext db, IConfiguration configurationManager, IWebHostEnvironment env,
            CandidateService candidateService) :
            base(logger, userManager, candidateService)
        {
            _db = db;
            _configuration = configurationManager;
            _env = env;


        }
        [HttpGet("Interview")]
        public ActionResult Interview()
        {
            this.InitializeViewBag();
            UserCourses userCourses = ViewBag.ActiveUserCourses;
            if (userCourses != null)
            {
                List<Questionnaire> questionnaires = _db.Questionnaire.
                    Where(r => r.QuestionnaireType == "Video").
                    Where(r => r.IsActive == "True").
                    Where(r => r.CourseId == userCourses.CourseId).ToList();

                List<InterviewResult> interviewResults = _db.InterviewResult.
                    Where(r => r.CreatedBy == User.FindFirstValue(ClaimTypes.NameIdentifier)).
                    Where(r => r.UserProfileId == userCourses.UserProfileId).ToList();

                for (int i = 0; i < questionnaires.Count; i++)
                {
                    try
                    {
                        if (interviewResults.Where(e =>
                                e.QuestionnaireId == questionnaires[i].QuestionnaireId).Count() > 0)
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
                    ViewBag.IsInterviewAvailables = false;
                }

                if (questionnaires.Count == interviewResults.Count)
                {
                    ViewBag.IsInterviewAvailable = false;
                }

                return View(questionnaires.FirstOrDefault());
            }
            else
            {
                return View();
            }
        }

        [HttpGet("DeleteInterview")]
        public ActionResult DeleteInterview()
        {
            this.InitializeViewBag();
            List<DeleteInterviewModel> deleteInterviewModels = new List<DeleteInterviewModel>();
            DeleteInterviewModel deleteInterviewModel = null;

            //UserProfile userProfile = _db.UserProfile.
            //    Where(r => r.LoginId == User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

            UserCourses userCourses = ViewBag.ActiveUserCourses;
            if (userCourses != null)
            {
                List<InterviewResult> interviewResults = _db.InterviewResult.
                    Where(r => r.CreatedBy == User.FindFirstValue(ClaimTypes.NameIdentifier)).
                    Where(r => r.UserProfileId == userCourses.UserProfileId).ToList();
                if (interviewResults.Count > 0)
                {
                    for (int i = 0; i < interviewResults.Count; i++)
                    {
                        deleteInterviewModel = new DeleteInterviewModel();
                        Questionnaire questionnaire = _db.Questionnaire.
                            Where(r => r.QuestionnaireId == interviewResults[i].QuestionnaireId).
                            Where(r => r.IsActive == "True").FirstOrDefault();

                        deleteInterviewModel.QuestionText = questionnaire.QuestionText;
                        deleteInterviewModel.VideoAbsolutePath = interviewResults[i].VideoAbsolutePath;
                        deleteInterviewModel.QuestionnaireId = (Guid)interviewResults[i].QuestionnaireId;
                        deleteInterviewModels.Add(deleteInterviewModel);
                    }

                    ViewBag.IsInterviewAvailable = true;

                }
                else
                {
                    ViewBag.IsInterviewAvailable = false;
                }

                return View(deleteInterviewModels);
            }
            else
            {
                return View();

            }
        }

        [HttpPost("UploadVideo")]
        public ActionResult PleaseUploadFile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IFormFileCollection files = Request.Form.Files;
            for (int i = 0; i < files.Count; i++)
            {
                IFormFile file = files[i];
                try
                {
                    string questionaireId = Request.Form["Question_Id"].ToString();
                    string fileName = this.User.Identity.Name + "_" + questionaireId + ".mp4";
                    string videopath = _configuration["AppSettings:FolderPath"];
                    string path = Path.Combine(_env.WebRootPath, videopath);

                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);
                    using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        file.CopyTo(stream);

                        UserProfile userProfile = _db.UserProfile.Where(r => r.LoginId == userId).First();
                        //List<InterviewResult> interviews = _db.InterviewResult.Where(r => r.CreatedBy == loginID).ToList();
                        InterviewResult interviewResult = new InterviewResult();
                        interviewResult.InterviewResultId = Guid.NewGuid();
                        interviewResult.UserProfileId = userProfile.UserProfileId;
                        interviewResult.VideoAbsolutePath = videopath + fileName;
                        interviewResult.VideoName = fileName;
                        interviewResult.QuestionnaireId = Guid.Parse(questionaireId);
                        interviewResult.QuestionOrder = 1;
                        interviewResult.CreatedBy = userId;
                        _db.InterviewResult.Add(interviewResult);
                        _db.SaveChanges();

                        //saveDataToDb(userId, fileName, path, questionaireId);

                    }
                }
                catch (Exception ex)
                {

                }

            }

            return RedirectToAction("Interview"); ;
        }

        [HttpPost("Delete")]

        public ActionResult Delete()
        {
            string questionaireId = Request.Form["Question_Id"].ToString();
            InterviewResult interviewResult = _db.InterviewResult.
                Where(r => r.CreatedBy == User.FindFirstValue(ClaimTypes.NameIdentifier)).
                Where(r => r.QuestionnaireId == Guid.Parse(questionaireId)).FirstOrDefault();
            string path = Path.Combine(_env.WebRootPath, interviewResult.VideoAbsolutePath);

            if (ExtensionMethods.DeleteFileFromFolder(path))
            {
                _db.InterviewResult.Remove(interviewResult);
                _db.SaveChanges();
            }
            return RedirectToAction("Interview"); ;
        }


    }
}



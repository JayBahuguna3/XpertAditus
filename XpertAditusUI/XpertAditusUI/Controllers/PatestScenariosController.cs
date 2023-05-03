using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using XpertAditusUI.Data;
using XpertAditusUI.Models;

namespace XpertAditusUI.Controllers
{
    public class PatestScenariosController : Controller
    {
        private readonly XpertAditusDbContext _context;

        public PatestScenariosController(XpertAditusDbContext context)
        {
            _context = context;
        }

        public IActionResult PATestScenario()
        {
            ViewData["AllCourses"] = _context.CourseMaster.Select(e => new SelectListItem(e.Name, e.CourseId.ToString())).ToList();
            return View();
        }

        public IActionResult GetTrainingContents(Guid Course)
        {
            var result = _context.PatrainingContentMaster.Include(e => 
                    e.PatestScenario.Where(p => p.TrainingContentsId == e.TrainingContentId))
                .Where(a => a.CourseId == Course && a.IsActive == true)
                .Select(e => new { 
                e.Name,
                e.CourseId,
                e.TrainingContentId,
                    NoOfQuestions = e.PatestScenario.Count() > 0 ? e.PatestScenario.FirstOrDefault().NoOfQuestions : 0 
                }).ToList();
            return new JsonResult(result);
        }

        public IActionResult SaveTrainingContents(int NoOfQuestion, Guid CourseId, Guid TrainingContentId)
        {
            bool AvalibleNoOfQuestion = CheckNoOfQuestion(NoOfQuestion, CourseId, TrainingContentId);
            if (!AvalibleNoOfQuestion)
            {
                return new JsonResult(NoOfQuestion + " Questions Not Avaliable");
            }

            string id = User.Claims.Select(x => x.Value).First();

            var result = _context.PatestScenario.Where(a => a.IsActive == true && 
                a.CourseId == CourseId && a.TrainingContentsId == TrainingContentId).FirstOrDefault();

            if (result == null)
            {
                PatestScenario testScenario = new PatestScenario();
                testScenario.PatestScenarioId = Guid.NewGuid();
                testScenario.CourseId = CourseId;
                testScenario.NoOfQuestions = NoOfQuestion;
                testScenario.TrainingContentsId = TrainingContentId;
                testScenario.IsActive = true;
                testScenario.CreatedDate = DateTime.Now;
                testScenario.CreatedBy = id;
                _context.Add(testScenario);
                _context.SaveChanges();
            }
            else
            {
                result.NoOfQuestions = NoOfQuestion;
                result.UpdatedDate = DateTime.Now;
                result.UpdatedBy = id;
                _context.SaveChanges();
            }

            return new JsonResult(true);
        }
        private bool CheckNoOfQuestion(int NoOfQuestion, Guid CourseId, Guid TrainingContentId)
        {
            bool Result = false;

            int QuestionCount = _context.Paquestionnaire.
                Where(a => a.CourseId == CourseId && 
                a.PatrainingContentId == TrainingContentId && a.IsActive == true).Count();

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
    }
}

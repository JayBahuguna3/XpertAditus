using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    public class TestScenarioController : Controller
    {

        private CandidateService _CandidateService;

        public TestScenarioController(CandidateService candidateService)
        {
            _CandidateService  = candidateService;
        }

        public IActionResult TestScenario()
        {
            ViewData["AllCourses"] = _CandidateService.GetCourses().Select(e => new SelectListItem(e.Name, e.CourseId.ToString())).ToList();
            return View();
        }

        public IActionResult GetTrainingContents(Guid Course)
        {
            var result = _CandidateService.getTrainingContentsMaster(Course);

            return new JsonResult(result);
        }

        public IActionResult SaveTrainingContents(int NoOfQuestion, Guid CourseId, Guid TrainingContentId)
        {
            bool AvalibleNoOfQuestion = _CandidateService.CheckNoOfQuestion(NoOfQuestion, CourseId, TrainingContentId);
            if (!AvalibleNoOfQuestion)
            {
                return new JsonResult(NoOfQuestion + " Questions Not Avaliable");
            }

            string id = User.Claims.Select(x => x.Value).First();
            _CandidateService.SaveTrainingContents(NoOfQuestion, CourseId, TrainingContentId, id);

            return new JsonResult(true);
        }
    }
}

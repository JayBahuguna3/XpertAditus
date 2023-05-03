using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionnaireController : BaseController
    {
        private readonly UserProfileService _userProfileService;

        public QuestionnaireController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
           UserProfileService userProfileService) : base(logger, userManager)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var questionnaire = _userProfileService.GetQuestionnaire();
            return new JsonResult(questionnaire);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] QuestionInfo questionInfo)
        {
            return new JsonResult("Ok");
        }
    }

    public class QuestionInfo
    {
        public int CurrentQuestionNo { get; set; }
        public int CurrentQuestionAns { get; set; }
        public int NextQuestionNo { get; set; }
    }
}

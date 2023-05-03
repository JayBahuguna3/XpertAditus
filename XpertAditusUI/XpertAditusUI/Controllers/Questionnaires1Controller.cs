using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using XpertAditusUI.Data;
using XpertAditusUI.Models;

namespace XpertAditusUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Questionnaires1Controller : ControllerBase
    {
        private readonly XpertAditusDbContext _context;
        UserManager<IdentityUser> _userManager;

        public Questionnaires1Controller(XpertAditusDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

       

        // GET: api/Questionnaires1
        //[HttpGet]
        //public async Task<IActionResult> GetQuestionnaire(int questionOrder = -1)
        //{
        //    try
        //    {
        //        var userId = this._userManager.GetUserId(this.User);
        //        if (userId == null)
        //        {
        //            return Ok("User is not Logged-in");
        //        }
        //        var cResult = GetCandidateResult();
        //        if (cResult == null)
        //        {
        //            return Ok("Course is not selected!");
        //        }
        //        var activeCourse = GetActiveUserCourse();
        //        if (activeCourse == null)
        //        {
        //            return Ok("No Active course found!");
        //        }
        //        var randQuestion = GetRandomQuestion(activeCourse.CourseId);
        //        if (questionOrder < 0)
        //        {
        //            //randQuestion = GetRandomQuestion(activeCourse.CourseId);

        //            questionOrder = GetQuestionOrder(cResult.CandidateResultId) + 1;
        //            while (ValidateDuplicateQuestion(randQuestion))
        //            {
        //                randQuestion = GetRandomQuestion(activeCourse.CourseId);
        //            }
        //            QuestionnaireResult questionnaireResult = new QuestionnaireResult()
        //            {
        //                //QuestionnaireResultId = new Guid(),
        //                QuestionnaireResultId = Guid.NewGuid(),
        //                CandidateResultId = cResult.CandidateResultId,
        //                CreatedBy = userId,
        //                CreatedDate = DateTime.Now,
        //                QuestionnaireId = randQuestion.QuestionnaireId,
        //                //IsCorrectAnswer = Convert.ToBoolean(randQuestion.CorrectAnswer),
        //                QuestionOrder = questionOrder
        //            };
        //            _context.QuestionnaireResult.Add(questionnaireResult);
        //            _context.SaveChanges();
        //        }
        //        else
        //        {
        //            var qResult = _context.QuestionnaireResult.Where(e => e.CandidateResultId == cResult.CandidateResultId
        //            && questionOrder == questionOrder).FirstOrDefault();
        //            randQuestion = _context.Questionnaire.Where(e => e.QuestionnaireId == qResult.QuestionnaireId).FirstOrDefault();
        //            randQuestion.AnswerProvided = qResult.AnswerProvided.ToString();

        //        }
        //        randQuestion.QuestionOrder = questionOrder;
        //        return new JsonResult(new
        //        {
        //            QuestionnaireId = randQuestion.QuestionnaireId,
        //            QuestionText = randQuestion.QuestionText,
        //            Option1 = randQuestion.Option1,
        //            Option2 = randQuestion.Option2,
        //            Option3 = randQuestion.Option3,
        //            Option4 = randQuestion.Option4,
        //            AnswerProvided = randQuestion.AnswerProvided,
        //            CorrectAnswer = randQuestion.CorrectAnswer,
        //            QuestionOrder = randQuestion.QuestionOrder
        //            //CourseId = j.CourseId
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ex.Message);
        //    }
        //}

        // GET: api/Questionnaires1/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Questionnaire>> GetQuestionnaire(Guid id)
        //{
        //    var questionnaire = await _context.Questionnaire.FindAsync(id);

        //    if (questionnaire == null)
        //    {
        //        return NotFound();
        //    }

        //    return questionnaire;
        //}

        // PUT: api/Questionnaires1/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutQuestionnaire(Guid id, Questionnaire questionnaire)
        //{
        //    if (id != questionnaire.QuestionnaireId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(questionnaire).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!QuestionnaireExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Questionnaires1
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Questionnaire>> PostQuestionnaire(Questionnaire questionnaire)
        //{
        //    _context.Questionnaire.Add(questionnaire);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetQuestionnaire", new { id = questionnaire.QuestionnaireId }, questionnaire);
        //}

        //// DELETE: api/Questionnaires1/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteQuestionnaire(Guid id)
        //{
        //    var questionnaire = await _context.Questionnaire.FindAsync(id);
        //    if (questionnaire == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Questionnaire.Remove(questionnaire);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        
    }
}

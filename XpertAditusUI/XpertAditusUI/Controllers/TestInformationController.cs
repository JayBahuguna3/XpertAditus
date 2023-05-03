using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XpertAditusUI.Data;
using XpertAditusUI.Models;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestInformationController : BaseController
    {
        private readonly XpertAditusDbContext _context;


        public TestInformationController(XpertAditusDbContext context, ILogger<HomeController> logger,
            UserManager<IdentityUser> userManager
            ) : base(logger, userManager)
        {
            _context = context;
        }

        // GET: api/TestInformation
        [HttpGet]
        public IActionResult GetCandidateResult()
        {

            try
            {
                var userid = _userManager.GetUserId(this.User); //loginid
                if (userid == null)
                {
                    return Ok("LoginId not found");
                }
                var userprofileid = _context.UserProfile.Where(e => e.LoginId == userid).Select(e => e.UserProfileId).FirstOrDefault(); //userprofileid
                if (String.IsNullOrEmpty(userprofileid.ToString()))
                {
                    return Ok("user profile not created");
                }
                var usercourseid = _context.UserCourses.Where(e => e.UserProfileId == userprofileid && e.IsActive == true).FirstOrDefault().UserCoursesId;//usercourseid
                if (String.IsNullOrEmpty(usercourseid.ToString()))
                {
                    return Ok("course not selected");
                }

                var result = _context.CandidateResult.Where(e => e.UserCoursesId == usercourseid && e.IsActive == true).FirstOrDefault();
                if (result == null)
                {
                    return Ok("Candidate result not found.");
                }
                else
                {
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }


        }
       /* [HttpGet]
        [Route("api/TestInformation/CompleteResult")]
        public IActionResult GCompleteResult()
        {
            return Json(_context.CandidateResult.FirstOrDefault());
        }*/

        /*  var userid = this._userManager.GetUserId(this.User);
         UserProfile userProfile = _userProfileService.GetUserInfo(userid);
         return await _context.CandidateResult.Where(u => u.LoginId == Id).Select(u => u.UserProfileId).ToListAsync();*/
        /*  return await _context.CandidateResult.Select(r => new
         {
            candidateResultID=r.CandidateResultId,
            UsercourseId =r.UserCoursesId,
             testDuration = r.TestDuration,
             isComplete = r.IsCompleted,
             isCleared = r.IsCleared,
             testStarted = r.TestStarted,
             remainingTime = r.RemainingTime,
             testEnd = r.TestEnd,
             PaymentHistoryId = r.PaymentHistoryId,
             isActive = r.IsActive,
             testAttempt = r.TestDuration,
             score = r.Score,
             status = r.Status
           createdDate = r.CreatedDate,
             createdby = r.CreatedBy
         }).ToListAsync();*/

        /*      public IActionResult CompleteTest(CandidateResult candidateResult)
              {
                  return Json(candidateResult);
              }*/
       

        // GET: api/TestInformation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CandidateResult>> GetCandidateResult(Guid id)
        {
            var candidateResult = await _context.CandidateResult.FindAsync(id);

            if (candidateResult == null)
            {
                return NotFound();
            }

            return candidateResult;
        }

        // PUT: api/TestInformation/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCandidateResult(Guid id, CandidateResult candidateResult)
        {
            if (id != candidateResult.CandidateResultId)
            {
                return BadRequest();
            }

            _context.Entry(candidateResult).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CandidateResultExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TestInformation
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<CandidateResult>> PostCandidateResult(CandidateResult candidateResult)
        //{
        //    _context.CandidateResult.Add(candidateResult);
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (CandidateResultExists(candidateResult.CandidateResultId))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtAction("GetCandidateResult", new { id = candidateResult.CandidateResultId }, candidateResult);
        //}

        // DELETE: api/TestInformation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCandidateResult(Guid id)
        {
            var candidateResult = await _context.CandidateResult.FindAsync(id);
            if (candidateResult == null)
            {
                return NotFound();
            }

            _context.CandidateResult.Remove(candidateResult);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CandidateResultExists(Guid id)
        {
            return _context.CandidateResult.Any(e => e.CandidateResultId == id);
        }
    }
}

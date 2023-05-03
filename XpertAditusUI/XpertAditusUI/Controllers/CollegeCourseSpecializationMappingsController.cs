using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XpertAditusUI.Data;
using XpertAditusUI.Models;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    public class CollegeCourseSpecializationMappingsController : BaseController
    {
        private readonly XpertAditusDbContext _context;
        private readonly UserProfileService _userProfileService;

        public CollegeCourseSpecializationMappingsController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
         UserProfileService userProfileService, XpertAditusDbContext context) : base(logger, userManager)
        {
            _context = context;
            _userProfileService = userProfileService;
        }

        // GET: CollegeCourseSpecializationMappings
        public async Task<IActionResult> Index()
        {
            var userid = this._userManager.GetUserId(this.User);
            var colgProfile = _context.CollegeProfile.Where(e => e.LoginId == userid).FirstOrDefault();
            var xpertAditusDbContext = _context.CollegeCourseSpecializationMapping
                .Include(c => c.CollegeProfile)
                .Include(c => c.Course)
                .Include(c => c.Education)
                .Include(c => c.CreatedByNavigation)
                .Include(c => c.Specialzation)
                .Include(c => c.UpdatedByNavigation).Where(c => c.CollegeProfileId == colgProfile.CollegeProfileId);
            return View(await xpertAditusDbContext.ToListAsync());
        }

        // GET: CollegeCourseSpecializationMappings/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collegeCourseSpecializationMapping = await _context.CollegeCourseSpecializationMapping
                .Include(c => c.CollegeProfile)
                .Include(c => c.Course)
                .Include(c => c.Education)
                .Include(c => c.CreatedByNavigation)
                .Include(c => c.Specialzation)
                .Include(c => c.UpdatedByNavigation)
                .FirstOrDefaultAsync(m => m.CollegeCourseSpecializationId == id);
            if (collegeCourseSpecializationMapping == null)
            {
                return NotFound();
            }

            return View(collegeCourseSpecializationMapping);
        }

        // GET: CollegeCourseSpecializationMappings/Create
        public IActionResult Create()
        {

            var userid = this._userManager.GetUserId(this.User);
            var collegeProfile = this._context.CollegeProfile.Where(u => u.LoginId == userid).FirstOrDefault();
            //  CollegeProfile collegeProfile = _userProfileService.GetCollegeProfileInfo(userid);

            ViewData["CollegeProfileId"] = this._context.CollegeProfile.Where(e => e.CollegeProfileId == collegeProfile.CollegeProfileId)
                .Select(e => new SelectListItem() { Text = e.Name, Value = e.CollegeProfileId.ToString() });

            //  ViewData["CollegeProfileId"] = new SelectList(_context.CollegeProfile, "CollegeProfileId", "Name");
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name");
            ViewBag.Course = _context.CountryMaster.ToList();
            ViewData["EducationId"] = new SelectList(_context.EducationMaster, "EducationId", "Name");
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["SpecialzationId"] = new SelectList(_context.SpecializationMaster, "SpecializationId", "SpecializationId");
            ViewData["UpdatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }
        public JsonResult LoadQualification(Guid CourseId)
        {
            string type = _context.CourseMaster.Where(c => c.CourseId == CourseId).Select(c => c.Type).FirstOrDefault();
            var educationInfo = _context.EducationMaster.Where(e => e.Type == type).ToList();
            return Json(educationInfo);
        }

        // POST: CollegeCourseSpecializationMappings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CollegeCourseSpecializationId,CollegeProfileId,CourseId,EducationId,SpecialzationId,IsActive,HigherSecondary,Graduate,MinQualification,MinTestScore,MinAcademicPercentage,CreatedDate,UpdatedDate,CreatedBy,UpdatedBy")] CollegeCourseSpecializationMapping collegeCourseSpecializationMapping)
        {
            var userid = this._userManager.GetUserId(this.User);
            var collegeProfile = this._context.CollegeProfile.Where(u => u.LoginId == userid).FirstOrDefault();
            if (ModelState.IsValid)
            {
                if (_context.CollegeCourseSpecializationMapping.Where(e => e.CollegeProfileId == collegeProfile.CollegeProfileId && e.CourseId == collegeCourseSpecializationMapping.CourseId).Count() > 0)
                {
                    ModelState.AddModelError(nameof(collegeCourseSpecializationMapping.CourseId), "Already Exist");
                }
                else
                {
                    collegeCourseSpecializationMapping.CollegeCourseSpecializationId = Guid.NewGuid();
                    collegeCourseSpecializationMapping.CreatedDate = DateTime.Now;
                    collegeCourseSpecializationMapping.UpdatedDate = DateTime.Now;
                    collegeCourseSpecializationMapping.CreatedBy = User.Claims.Select(x => x.Value).First();
                    collegeCourseSpecializationMapping.UpdatedBy = User.Claims.Select(x => x.Value).First();
                    collegeCourseSpecializationMapping.CollegeProfileId = collegeProfile.CollegeProfileId;
                    _context.Add(collegeCourseSpecializationMapping);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            // ViewData["CollegeProfileId"] = new SelectList(_context.CollegeProfile, "CollegeProfileId", "Name", collegeCourseSpecializationMapping.CollegeProfileId);
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name", collegeCourseSpecializationMapping.CourseId);
            ViewData["EducationId"] = new SelectList(_context.EducationMaster, "EducationId", "Name");
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", collegeCourseSpecializationMapping.CreatedBy);
            ViewData["SpecialzationId"] = new SelectList(_context.SpecializationMaster, "SpecializationId", "SpecializationId", collegeCourseSpecializationMapping.SpecialzationId);
            ViewData["UpdatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", collegeCourseSpecializationMapping.UpdatedBy);
            return View(collegeCourseSpecializationMapping);
        }

        // GET: CollegeCourseSpecializationMappings/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collegeCourseSpecializationMapping = await _context.CollegeCourseSpecializationMapping.FindAsync(id);
            if (collegeCourseSpecializationMapping == null)
            {
                return NotFound();
            }
            var userid = this._userManager.GetUserId(this.User);
            var collegeProfile = this._context.CollegeProfile.Where(u => u.LoginId == userid).FirstOrDefault();
            ViewData["CollegeProfileId"] = this._context.CollegeProfile.Where(e => e.CollegeProfileId == collegeProfile.CollegeProfileId)
                .Select(e => new SelectListItem() { Text = e.Name, Value = e.CollegeProfileId.ToString() });

            // ViewData["CollegeProfileId"] = new SelectList(_context.CollegeProfile, "CollegeProfileId", "Name", collegeCourseSpecializationMapping.CollegeProfileId);
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name", collegeCourseSpecializationMapping.CourseId);
            ViewData["EducationId"] = new SelectList(_context.EducationMaster, "EducationId", "Name");
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", collegeCourseSpecializationMapping.CreatedBy);
            ViewData["SpecialzationId"] = new SelectList(_context.SpecializationMaster, "SpecializationId", "SpecializationId", collegeCourseSpecializationMapping.SpecialzationId);
            ViewData["UpdatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", collegeCourseSpecializationMapping.UpdatedBy);
            return View(collegeCourseSpecializationMapping);
        }

        // POST: CollegeCourseSpecializationMappings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CollegeCourseSpecializationId,CollegeProfileId,CourseId,EducationId,SpecialzationId,IsActive,HigherSecondary,Graduate,MinQualification,MinTestScore,MinAcademicPercentage,CreatedDate,UpdatedDate,CreatedBy,UpdatedBy")] CollegeCourseSpecializationMapping collegeCourseSpecializationMapping)
        {

            var userid = this._userManager.GetUserId(this.User);
            var collegeProfile = this._context.CollegeProfile.Where(u => u.LoginId == userid).FirstOrDefault();
            if (id != collegeCourseSpecializationMapping.CollegeCourseSpecializationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    collegeCourseSpecializationMapping.CreatedDate = DateTime.Now;
                    collegeCourseSpecializationMapping.UpdatedDate = DateTime.Now;
                    collegeCourseSpecializationMapping.CreatedBy = User.Claims.Select(x => x.Value).First();
                    collegeCourseSpecializationMapping.UpdatedBy = User.Claims.Select(x => x.Value).First();
                    collegeCourseSpecializationMapping.CollegeProfileId = collegeProfile.CollegeProfileId;

                    _context.Update(collegeCourseSpecializationMapping);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollegeCourseSpecializationMappingExists(collegeCourseSpecializationMapping.CollegeCourseSpecializationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            //  ViewData["CollegeProfileId"] = new SelectList(_context.CollegeProfile, "CollegeProfileId", "Name", collegeCourseSpecializationMapping.CollegeProfileId);
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name", collegeCourseSpecializationMapping.CourseId);
            ViewData["EducationId"] = new SelectList(_context.EducationMaster, "EducationId", "Name");
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", collegeCourseSpecializationMapping.CreatedBy);
            ViewData["SpecialzationId"] = new SelectList(_context.SpecializationMaster, "SpecializationId", "SpecializationId", collegeCourseSpecializationMapping.SpecialzationId);
            ViewData["UpdatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", collegeCourseSpecializationMapping.UpdatedBy);
            return View(collegeCourseSpecializationMapping);
        }

        // GET: CollegeCourseSpecializationMappings/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collegeCourseSpecializationMapping = await _context.CollegeCourseSpecializationMapping
                .Include(c => c.CollegeProfile)
                .Include(c => c.Course)
                .Include(c => c.Education)
                .Include(c => c.CreatedByNavigation)
                .Include(c => c.Specialzation)
                .Include(c => c.UpdatedByNavigation)
                .FirstOrDefaultAsync(m => m.CollegeCourseSpecializationId == id);
            if (collegeCourseSpecializationMapping == null)
            {
                return NotFound();
            }

            return View(collegeCourseSpecializationMapping);
        }

        // POST: CollegeCourseSpecializationMappings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var collegeCourseSpecializationMapping = await _context.CollegeCourseSpecializationMapping.FindAsync(id);
            _context.CollegeCourseSpecializationMapping.Remove(collegeCourseSpecializationMapping);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CollegeCourseSpecializationMappingExists(Guid id)
        {
            return _context.CollegeCourseSpecializationMapping.Any(e => e.CollegeCourseSpecializationId == id);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.Models;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    public class PADisclaimerController : BaseController
    {
        private readonly XpertAditusDbContext _context;

        private readonly UserProfileService _userProfileService;

        public PADisclaimerController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
          UserProfileService userProfileService, XpertAditusDbContext context) : base(logger, userManager)
        {
            _context = context;
            _userProfileService = userProfileService;
        }

        // GET: Disclaimers
        public async Task<IActionResult> Index()
        {
            var XpertAditusDbContext = _context.Padisclaimer
                .Include(d => d.Course)
                .Include(d => d.CreatedByNavigation)
                .Include(d => d.ModifiedByNavigation);
            return View(await XpertAditusDbContext.ToListAsync());
        }

        // GET: Disclaimers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disclaimer = await _context.Padisclaimer
                .Include(d => d.Course)
                .Include(d => d.CreatedByNavigation)
                .Include(d => d.ModifiedByNavigation)
                .FirstOrDefaultAsync(m => m.PadisclaimerId == id);
            if (disclaimer == null)
            {
                return NotFound();
            }

            return View(disclaimer);
        }

        // GET: Disclaimers/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name");
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: Disclaimers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DisclaimerId,Instruction,CourseId,IsActive,ModifiedBy,ModifiedDate,CreatedDate,CreatedBy")] Padisclaimer disclaimer)
        {
            if (ModelState.IsValid)
            {
                disclaimer.PadisclaimerId = Guid.NewGuid();
                disclaimer.CreatedDate = DateTime.Now;
                disclaimer.ModifiedDate = DateTime.Now;
                disclaimer.CreatedBy = User.Claims.Select(x => x.Value).First();
                disclaimer.ModifiedBy = User.Claims.Select(x => x.Value).First();
                _context.Add(disclaimer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //   ViewData["CourseId"] = new SelectList(_context.Course, "CourseId", "Name", disclaimer.CourseId);
            //  ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", disclaimer.CreatedBy);
            //  ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", disclaimer.ModifiedBy);
            return View(disclaimer);
        }

        // GET: Disclaimers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disclaimer = await _context.Padisclaimer.FindAsync(id);
            if (disclaimer == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name", disclaimer.CourseId);
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", disclaimer.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", disclaimer.ModifiedBy);
            return View(disclaimer);
        }

        // POST: Disclaimers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PadisclaimerId,Instruction,CourseId,IsActive,ModifiedBy,ModifiedDate,CreatedDate,CreatedBy")] Padisclaimer disclaimer)
        {
            if (id != disclaimer.PadisclaimerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    disclaimer.CreatedDate = DateTime.Now;
                    disclaimer.ModifiedDate = DateTime.Now;
                    disclaimer.CreatedBy = User.Claims.Select(x => x.Value).First();
                    disclaimer.ModifiedBy = User.Claims.Select(x => x.Value).First();
                    _context.Update(disclaimer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisclaimerExists(disclaimer.PadisclaimerId))
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
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name", disclaimer.CourseId);
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", disclaimer.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", disclaimer.ModifiedBy);
            return View(disclaimer);
        }

        // GET: Disclaimers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disclaimer = await _context.Padisclaimer
                .FirstOrDefaultAsync(m => m.PadisclaimerId == id);
            if (disclaimer == null)
            {
                return NotFound();
            }

            return View(disclaimer);
        }

        // POST: Disclaimers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var disclaimer = await _context.Padisclaimer.FindAsync(id);
            _context.Padisclaimer.Remove(disclaimer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DisclaimerExists(Guid id)
        {
            return _context.Padisclaimer.Any(e => e.PadisclaimerId == id);
        }
    }
}

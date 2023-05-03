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
    public class DisclaimerController : BaseController
    {
        private readonly XpertAditusDbContext _context;

        private readonly UserProfileService _userProfileService;

        public DisclaimerController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
          UserProfileService userProfileService, XpertAditusDbContext context) : base(logger, userManager)
        {
            _context = context;
            _userProfileService = userProfileService;
        }
        
        // GET: Disclaimers
        public async Task<IActionResult> Index()
        {
            var XpertAditusDbContext = _context.Disclaimer
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

            var disclaimer = await _context.Disclaimer
                .Include(d => d.Course)
                .Include(d => d.CreatedByNavigation)
                .Include(d => d.ModifiedByNavigation)
                .FirstOrDefaultAsync(m => m.DisclaimerId == id);
            if (disclaimer == null)
            {
                return NotFound();
            }

            return View(disclaimer);
        }

        // GET: Disclaimers/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Course, "CourseId", "Name");
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: Disclaimers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DisclaimerId,Instruction,CourseId,IsActive,ModifiedBy,ModifiedDate,CreatedDate,CreatedBy")] Disclaimer disclaimer)
        {
            if (ModelState.IsValid)
            {
                disclaimer.DisclaimerId = Guid.NewGuid();
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

            var disclaimer = await _context.Disclaimer.FindAsync(id);
            if (disclaimer == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "CourseId", "Name", disclaimer.CourseId);
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", disclaimer.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", disclaimer.ModifiedBy);
            return View(disclaimer);
        }

        // POST: Disclaimers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("DisclaimerId,Instruction,CourseId,IsActive,ModifiedBy,ModifiedDate,CreatedDate,CreatedBy")] Disclaimer disclaimer)
        {
            if (id != disclaimer.DisclaimerId)
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
                    if (!DisclaimerExists(disclaimer.DisclaimerId))
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
            ViewData["CourseId"] = new SelectList(_context.Course, "CourseId", "Name", disclaimer.CourseId);
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

            var disclaimer = await _context.Disclaimer
                .Include(d => d.Course)
                .Include(d => d.CreatedByNavigation)
                .Include(d => d.ModifiedByNavigation)
                .FirstOrDefaultAsync(m => m.DisclaimerId == id);
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
            var disclaimer = await _context.Disclaimer.FindAsync(id);
            _context.Disclaimer.Remove(disclaimer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DisclaimerExists(Guid id)
        {
            return _context.Disclaimer.Any(e => e.DisclaimerId == id);
        }
    }
}

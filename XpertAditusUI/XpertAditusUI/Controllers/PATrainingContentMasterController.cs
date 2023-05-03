using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using XpertAditusUI.Data;
using XpertAditusUI.Models;

namespace XpertAditusUI.Controllers
{
    public class PATrainingContentMasterController : Controller
    {
        private readonly XpertAditusDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PATrainingContentMasterController(XpertAditusDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _hostEnvironment = environment;
        }

        // GET: TrainingContentsMasters
        public async Task<IActionResult> Index()
        {
            var XpertAditusDbContext = _context.PatrainingContentMaster.Include(t => t.Course).Include(t => t.CreatedByNavigation).Include(t => t.ModifiedByNavigation);
            return View(await XpertAditusDbContext.ToListAsync());
        }

        // GET: TrainingContentsMasters/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patrainingContent = await _context.PatrainingContentMaster
                .Include(t => t.Course)
                .Include(t => t.CreatedByNavigation)
                .Include(t => t.ModifiedByNavigation)
                .FirstOrDefaultAsync(m => m.TrainingContentId == id);
            if (patrainingContent == null)
            {
                return NotFound();
            }

            return View(patrainingContent);
        }

        // GET: TrainingContentsMasters/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name");
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: TrainingContentsMasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TrainingContentId,Name,Path,ContentType,CourseId,IsActive,ModifiedBy,ModifiedDate,CreatedDate,CreatedBy")] PatrainingContentMaster patrainingContent)
        {
            if (ModelState.IsValid)
            {
                string uploads = Path.Combine(_hostEnvironment.WebRootPath, "fileContent\\content");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);
                //string paths = uploads;
                foreach (IFormFile file in this.Request.Form.Files)
                {
                    if (file.Length > 0)
                    {
                        string filePath = Path.Combine(uploads, patrainingContent.Name + "." + file.FileName.Split(".")[1]);
                        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        patrainingContent.Path = Path.Combine(@"fileContent\content", patrainingContent.Name + "." + file.FileName.Split(".")[1]);
                    }
                }




                patrainingContent.TrainingContentId = Guid.NewGuid();
                _context.Add(patrainingContent);
                patrainingContent.CreatedDate = DateTime.Now;
                patrainingContent.ModifiedDate = DateTime.Now;
                patrainingContent.CreatedBy = User.Claims.Select(x => x.Value).First();
                patrainingContent.ModifiedBy = User.Claims.Select(x => x.Value).First();
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name", patrainingContent.CourseId);
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", patrainingContent.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", patrainingContent.ModifiedBy);
            return View(patrainingContent);
        }

        // GET: TrainingContentsMasters/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patrainingContent = await _context.PatrainingContentMaster.FindAsync(id);
            if (patrainingContent == null)
            {
                return NotFound();
            }

            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name", patrainingContent.CourseId);
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", patrainingContent.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", patrainingContent.ModifiedBy);
            return View(patrainingContent);
        }

        // POST: TrainingContentsMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("TrainingContentId,Name,Path,ContentType,CourseId,IsActive,ModifiedBy,ModifiedDate,CreatedDate,CreatedBy")] PatrainingContentMaster patrainingContent)
        {
            if (id != patrainingContent.TrainingContentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    patrainingContent.CreatedDate = DateTime.Now;
                    patrainingContent.ModifiedDate = DateTime.Now;
                    patrainingContent.CreatedBy = User.Claims.Select(x => x.Value).First();
                    patrainingContent.ModifiedBy = User.Claims.Select(x => x.Value).First();

                    string uploads = Path.Combine(_hostEnvironment.WebRootPath, "fileContent\\content");
                    if (!Directory.Exists(uploads))
                        Directory.CreateDirectory(uploads);
                    //string paths = uploads;
                    foreach (IFormFile file in this.Request.Form.Files)
                    {
                        if (file.Length > 0)
                        {
                            string filePath = Path.Combine(uploads, patrainingContent.Name + "." + file.FileName.Split(".")[1]);
                            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            patrainingContent.Path = Path.Combine(@"fileContent\content", patrainingContent.Name + "." + file.FileName.Split(".")[1]);
                        }
                    }

                    _context.Update(patrainingContent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingContentsMasterExists(patrainingContent.TrainingContentId))
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
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name", patrainingContent.CourseId);
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", patrainingContent.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", patrainingContent.ModifiedBy);
            return View(patrainingContent);
        }

        // GET: TrainingContentsMasters/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patrainingContent = await _context.PatrainingContentMaster
                .Include(t => t.Course)
                .Include(t => t.CreatedByNavigation)
                .Include(t => t.ModifiedByNavigation)
                .FirstOrDefaultAsync(m => m.TrainingContentId == id);
            if (patrainingContent == null)
            {
                return NotFound();
            }

            return View(patrainingContent);
        }

        // POST: TrainingContentsMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var patrainingContent = await _context.PatrainingContentMaster.FindAsync(id);
            _context.PatrainingContentMaster.Remove(patrainingContent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingContentsMasterExists(Guid id)
        {
            return _context.PatrainingContentMaster.Any(e => e.TrainingContentId == id);
        }
    }
}

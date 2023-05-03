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
    public class TrainingContentsMastersController : Controller
    {
        private readonly XpertAditusDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public TrainingContentsMastersController(XpertAditusDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _hostEnvironment = environment;
        }

        // GET: TrainingContentsMasters
        public async Task<IActionResult> Index()
        {
            var XpertAditusDbContext = _context.TrainingContentsMaster.Include(t => t.Course).Include(t => t.CreatedByNavigation).Include(t => t.ModifiedByNavigation);
            return View(await XpertAditusDbContext.ToListAsync());
        }

        // GET: TrainingContentsMasters/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainingContentsMaster = await _context.TrainingContentsMaster
                .Include(t => t.Course)
                .Include(t => t.CreatedByNavigation)
                .Include(t => t.ModifiedByNavigation)
                .FirstOrDefaultAsync(m => m.TrainingContentId == id);
            if (trainingContentsMaster == null)
            {
                return NotFound();
            }

            return View(trainingContentsMaster);
        }

        // GET: TrainingContentsMasters/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Course, "CourseId", "Name");
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: TrainingContentsMasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TrainingContentId,Name,Path,ContentType,CourseId,IsActive,ModifiedBy,ModifiedDate,CreatedDate,CreatedBy")] TrainingContentsMaster trainingContentsMaster)
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
                        string filePath = Path.Combine(uploads, trainingContentsMaster.Name + "." + file.FileName.Split(".")[1]);
                        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        trainingContentsMaster.Path = Path.Combine(@"fileContent\content", trainingContentsMaster.Name + "." + file.FileName.Split(".")[1]);
                    }
                }




                trainingContentsMaster.TrainingContentId = Guid.NewGuid();
                _context.Add(trainingContentsMaster);
                trainingContentsMaster.CreatedDate = DateTime.Now;
                trainingContentsMaster.ModifiedDate = DateTime.Now;
                trainingContentsMaster.CreatedBy = User.Claims.Select(x => x.Value).First();
                trainingContentsMaster.ModifiedBy = User.Claims.Select(x => x.Value).First();
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "CourseId", "Name", trainingContentsMaster.CourseId);
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", trainingContentsMaster.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", trainingContentsMaster.ModifiedBy);
            return View(trainingContentsMaster);
        }

        // GET: TrainingContentsMasters/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainingContentsMaster = await _context.TrainingContentsMaster.FindAsync(id);
            if (trainingContentsMaster == null)
            {
                return NotFound();
            }

            ViewData["CourseId"] = new SelectList(_context.Course, "CourseId", "Name", trainingContentsMaster.CourseId);
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", trainingContentsMaster.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", trainingContentsMaster.ModifiedBy);
            return View(trainingContentsMaster);
        }

        // POST: TrainingContentsMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("TrainingContentId,Name,Path,ContentType,CourseId,IsActive,ModifiedBy,ModifiedDate,CreatedDate,CreatedBy")] TrainingContentsMaster trainingContentsMaster)
        {
            if (id != trainingContentsMaster.TrainingContentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    trainingContentsMaster.CreatedDate = DateTime.Now;
                    trainingContentsMaster.ModifiedDate = DateTime.Now;
                    trainingContentsMaster.CreatedBy = User.Claims.Select(x => x.Value).First();
                    trainingContentsMaster.ModifiedBy = User.Claims.Select(x => x.Value).First();

                    string uploads = Path.Combine(_hostEnvironment.WebRootPath, "fileContent\\content");
                    if (!Directory.Exists(uploads))
                        Directory.CreateDirectory(uploads);
                    //string paths = uploads;
                    foreach (IFormFile file in this.Request.Form.Files)
                    {
                        if (file.Length > 0)
                        {
                            string filePath = Path.Combine(uploads, trainingContentsMaster.Name + "." + file.FileName.Split(".")[1]);
                            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            trainingContentsMaster.Path = Path.Combine(@"fileContent\content", trainingContentsMaster.Name + "." + file.FileName.Split(".")[1]);
                        }
                    }

                    _context.Update(trainingContentsMaster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingContentsMasterExists(trainingContentsMaster.TrainingContentId))
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
            ViewData["CourseId"] = new SelectList(_context.Course, "CourseId", "Name", trainingContentsMaster.CourseId);
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", trainingContentsMaster.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", trainingContentsMaster.ModifiedBy);
            return View(trainingContentsMaster);
        }

        // GET: TrainingContentsMasters/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainingContentsMaster = await _context.TrainingContentsMaster
                .Include(t => t.Course)
                .Include(t => t.CreatedByNavigation)
                .Include(t => t.ModifiedByNavigation)
                .FirstOrDefaultAsync(m => m.TrainingContentId == id);
            if (trainingContentsMaster == null)
            {
                return NotFound();
            }

            return View(trainingContentsMaster);
        }

        // POST: TrainingContentsMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var trainingContentsMaster = await _context.TrainingContentsMaster.FindAsync(id);
            _context.TrainingContentsMaster.Remove(trainingContentsMaster);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingContentsMasterExists(Guid id)
        {
            return _context.TrainingContentsMaster.Any(e => e.TrainingContentId == id);
        }
    }
}

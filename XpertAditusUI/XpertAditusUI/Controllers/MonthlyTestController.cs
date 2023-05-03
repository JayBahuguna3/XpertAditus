using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using XpertAditusUI.Data;
using XpertAditusUI.Models;

namespace XpertAditusUI.Controllers
{
    public class MonthlyTestController : Controller
    {
        private readonly XpertAditusDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IHostingEnvironment _hostingEnvironment;
        private string message;

        public MonthlyTestController(XpertAditusDbContext context, IWebHostEnvironment environment, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostEnvironment = environment;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult ImportQuestion()
        {
            return View();
        }
        // GET: /ExportExcel/
        public FileStreamResult ExportCourseContent()
        {
            var result = this._context.TrainingContentsMaster.Include(e => e.Course).Select(e =>
                new
                {
                    CourseId = e.CourseId,
                    CourseName = e.Course.Name,
                    TrainingContentId = e.TrainingContentId,
                    TrainingContentName = e.Name
                }).ToList();
            string filepath = Path.Combine(_hostEnvironment.ContentRootPath, "DocFormat\\CTExport.csv");

            using (var writer = new StreamWriter(filepath, false, System.Text.Encoding.UTF8))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {

                csv.WriteRecords(result); // where values implements IEnumerable
            }



            Stream stream = System.IO.File.OpenRead(filepath);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", "DocFormat\\CTExport.csv");

        }
        // GET: Questionnaires
        public async Task<IActionResult> Index()
        {
            var XpertAditusDbContext = _context.PamonthlyTest.Include(q => q.Course)
                .Include(q => q.CreatedByNavigation).Include(q => q.UpdatedByNavigation)
                .OrderByDescending(e => e.UpdatedDate);
            return View(await XpertAditusDbContext.ToListAsync());
        }

        // GET: Questionnaires/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pamonthlyTest = await _context.PamonthlyTest
                .Include(q => q.Course)
                .Include(q => q.CreatedByNavigation)
                .Include(q => q.UpdatedByNavigation)
                .FirstOrDefaultAsync(m => m.MonthlyTestId == id);
            if (pamonthlyTest == null)
            {
                return NotFound();
            }

            return View(pamonthlyTest);
        }

        // GET: Questionnaires/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name");
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");

            return View();
        }

        // POST: Questionnaires/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MonthlyTestId,CourseId,Month,Year,TestType,Name,Description,IsActive,UpdatedBy,UpdatedDate,CreatedDate,CreatedBy")] PamonthlyTest pamonthlyTest)
        {
            var monthlyTestExist = _context.PamonthlyTest.Where(e => e.Month == pamonthlyTest.Month 
                && e.Year == pamonthlyTest.Year 
                && e.TestType == pamonthlyTest.TestType 
                && e.IsActive == true
                && e.CourseId == pamonthlyTest.CourseId ).Count();
            if (monthlyTestExist == 0)
            {
                if (ModelState.IsValid)
                {
                    pamonthlyTest.MonthlyTestId = Guid.NewGuid();
                    pamonthlyTest.CreatedDate = DateTime.Now;
                    pamonthlyTest.UpdatedDate = DateTime.Now;
                    pamonthlyTest.CreatedBy = User.Claims.Select(x => x.Value).First();
                    pamonthlyTest.UpdatedBy = User.Claims.Select(x => x.Value).First();
                    _context.Add(pamonthlyTest);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["Error"] = "";
            }
            else
            {
                ViewData["Error"] = "Monthly Test Already Exist!";
            }
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name", pamonthlyTest.CourseId);
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", pamonthlyTest.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", pamonthlyTest.UpdatedBy);
            return View(pamonthlyTest);
        }

        // GET: Questionnaires/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pamonthlyTest = await _context.PamonthlyTest.FindAsync(id);
            if (pamonthlyTest == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name", pamonthlyTest.CourseId);
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", pamonthlyTest.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", pamonthlyTest.UpdatedBy);
            return View(pamonthlyTest);
        }

        // POST: Questionnaires/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("MonthlyTestId,CourseId,Month,Year,TestType,Name,Description,IsActive,UpdatedBy,UpdatedDate,CreatedDate,CreatedBy")] PamonthlyTest pamonthlyTest)
        {
            if (id != pamonthlyTest.MonthlyTestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    pamonthlyTest.CreatedDate = DateTime.Now;
                    pamonthlyTest.UpdatedDate = DateTime.Now;
                    pamonthlyTest.CreatedBy = User.Claims.Select(x => x.Value).First();
                    pamonthlyTest.UpdatedBy = User.Claims.Select(x => x.Value).First();
                    _context.Update(pamonthlyTest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                   
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name", pamonthlyTest.CourseId);
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", pamonthlyTest.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", pamonthlyTest.UpdatedBy);

            return View(pamonthlyTest);
        }

        // GET: Questionnaires/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paMonthlyTest = await _context.PamonthlyTest
                .Include(q => q.Course)
                .Include(q => q.CreatedByNavigation)
                .Include(q => q.UpdatedByNavigation)
                .FirstOrDefaultAsync(m => m.MonthlyTestId == id);
            if (paMonthlyTest == null)
            {
                return NotFound();
            }

            return View(paMonthlyTest);
        }

        // POST: Questionnaires/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var pamonthlyTest = await _context.PamonthlyTest.FindAsync(id);
            _context.PamonthlyTest.Remove(pamonthlyTest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}

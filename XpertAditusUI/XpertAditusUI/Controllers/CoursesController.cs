using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    public class CoursesController : Controller
    {
        private readonly XpertAditusDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private string message;

        public CoursesController(XpertAditusDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult ImportCourses()
        {
            return View();
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var XpertAditusDbContext = _context.Course.Include(c => c.CreatedByNavigation).Include(c => c.ModifiedByNavigation);
           /// string Name = User.Claims.Select(x => x.Value).First();

            return View(await XpertAditusDbContext.ToListAsync());
        }
        // GET: Courses/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.CreatedByNavigation)
                .Include(c => c.ModifiedByNavigation)
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }
       
        // GET: Courses/Create
        public IActionResult Create()
        {
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");

            return View();

        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,Name,Fee,Description,IsActive,Type,ModifiedBy,ModifiedDate,CreatedDate,CreatedBy")] Course course)
        {
             bool IsCourseNameExist = _context.Course.Any(x => 
                                     x.Name == course.Name && 
                                     x.CourseId != 
                                     course.CourseId);
            if (IsCourseNameExist == true)
            {
                ModelState.AddModelError("Name", "Course Name already exists");
            }
            if (ModelState.IsValid)
            {
                course.CourseId = Guid.NewGuid();
                course.CreatedDate = DateTime.Now;
                course.ModifiedDate = DateTime.Now;
                course.CreatedBy = User.Claims.Select(x => x.Value).First();
                    course.ModifiedBy = User.Claims.Select(x => x.Value).First();
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
          //  ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "UserName", course.CreatedBy);
           // ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "UserName", course.ModifiedBy);
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", course.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", course.ModifiedBy);
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CourseId,Name,Fee,Description,IsActive,Type,ModifiedBy,ModifiedDate,CreatedDate,CreatedBy")] Course course)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }

            bool IsCourseNameExist = _context.Course.Any(x =>
                                    x.Name == course.Name &&
                                    x.CourseId !=
                                    course.CourseId);
            if (IsCourseNameExist == true)
            {
                ModelState.AddModelError("Name", "Course Name already exists");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    course.CreatedDate = DateTime.Now;
                    course.ModifiedDate = DateTime.Now;
                    course.CreatedBy = User.Claims.Select(x => x.Value).First();
                    course.ModifiedBy = User.Claims.Select(x => x.Value).First();
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseId))
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
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "UserName", course.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "UserName", course.ModifiedBy);
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.CreatedByNavigation)
                .Include(c => c.ModifiedByNavigation)
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var course = await _context.Course.FindAsync(id);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(Guid id)
        {
            return _context.Course.Any(e => e.CourseId == id);
        }


        [HttpPost]
        public IActionResult Import(string fileName)
        {

            IFormFile file = Request.Form.Files[0];
            string folderName = "UploadExcel";
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            StringBuilder sb = new StringBuilder();
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            if (file.Length > 0)
            {
                string sFileExtension = Path.GetExtension(file.FileName).ToLower();
                ISheet sheet;
                string fullPath = Path.Combine(newPath, file.FileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    stream.Position = 0;
                    if (sFileExtension == ".xls")
                    {
                        HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                    }
                    else
                    {
                        XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                        sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
                    }




                    FileInfo fileInfo = new FileInfo(fullPath);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (ExcelPackage package = new ExcelPackage(fileInfo))
                    {
                        try
                        {

                            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                            int colcount = worksheet.Dimension.End.Column;
                            int rowcount = worksheet.Dimension.End.Row;
                            for (int row = 2; row <= rowcount; row++)
                            {

                                Course cd = new Course();

                                cd.Name = worksheet.Cells[row, 1].Value.ToString();
                                cd.Fee = Convert.ToDecimal(worksheet.Cells[row, 2].Value.ToString());
                                cd.Description = worksheet.Cells[row, 3].Value.ToString();
                                cd.IsActive = worksheet.Cells[row, 4].Value.ToString();
                                cd.ModifiedDate = DateTime.Now;
                                cd.CreatedBy = User.Claims.Select(x => x.Value).First();
                                cd.ModifiedBy = User.Claims.Select(x => x.Value).First();

                                _context.Course.Add(cd);
                            }
                            _context.SaveChanges();
                            return RedirectToPage("./Index");
                        }
                        catch (Exception ex)
                        {
                            message = "Please Fill Mandatory Sample CSV File";
                        }


                    }
                }
            }
            return RedirectToPage("./Index");
        }
    }
}

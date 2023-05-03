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
    public class PAQuestionnairesController : Controller
    {
        private readonly XpertAditusDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IHostingEnvironment _hostingEnvironment;
        private string message;

        public PAQuestionnairesController(XpertAditusDbContext context, IWebHostEnvironment environment, IHostingEnvironment hostingEnvironment)
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
            var result = this._context.PatrainingContentMaster.Include(e => e.Course).Select(e =>
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
            var XpertAditusDbContext = _context.Paquestionnaire.Include(q => q.Course)
                .Include(q => q.CreatedByNavigation)
                .Include(q => q.ModifiedByNavigation).Include(q => q.PatrainingContent);
            return View(await XpertAditusDbContext.ToListAsync());
        }

        // GET: Questionnaires/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionnaire = await _context.Paquestionnaire
                .Include(q => q.Course)
                .Include(q => q.CreatedByNavigation)
                .Include(q => q.ModifiedByNavigation)
                .Include(q => q.PatrainingContent)
                .FirstOrDefaultAsync(m => m.PaquestionnaireId == id);
            if (questionnaire == null)
            {
                return NotFound();
            }

            return View(questionnaire);
        }

        // GET: Questionnaires/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name");
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["TrainingContentId"] = new SelectList(_context.PatrainingContentMaster, "TrainingContentId", "Name");

            return View();
        }

        // POST: Questionnaires/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuestionnaireId,QuestionnaireType,QuestionText,Option1,Option2,Option3,Option4,CorrectAnswer,CourseId,PatrainingContentId,IsActive,ModifiedBy,ModifiedDate,CreatedDate,CreatedBy")] Paquestionnaire questionnaire)
        {
            if (ModelState.IsValid)
            {


                questionnaire.PaquestionnaireId = Guid.NewGuid();
                questionnaire.CreatedDate = DateTime.Now;
                questionnaire.ModifiedDate = DateTime.Now;
                questionnaire.CreatedBy = User.Claims.Select(x => x.Value).First();
                questionnaire.ModifiedBy = User.Claims.Select(x => x.Value).First();
                _context.Add(questionnaire);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name", questionnaire.CourseId);
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", questionnaire.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", questionnaire.ModifiedBy);
            ViewData["TrainingContentId"] = new SelectList(_context.TrainingContentsMaster, "TrainingContentId", "Name", questionnaire.PatrainingContentId);
            return View(questionnaire);
        }

        // GET: Questionnaires/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionnaire = await _context.Paquestionnaire.FindAsync(id);
            if (questionnaire == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name", questionnaire.CourseId);
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", questionnaire.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", questionnaire.ModifiedBy);
            ViewData["PatrainingContentId"] = new SelectList(_context.PatrainingContentMaster, "TrainingContentId", "Name", questionnaire.PatrainingContentId);
            return View(questionnaire);
        }

        // POST: Questionnaires/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PaquestionnaireId,QuestionnaireType,QuestionText,Option1,Option2,Option3,Option4,CorrectAnswer,CourseId,PatrainingContentId,IsActive,ModifiedBy,ModifiedDate,CreatedDate,CreatedBy")] Paquestionnaire questionnaire)
        {
            if (id != questionnaire.PaquestionnaireId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    questionnaire.CreatedDate = DateTime.Now;
                    questionnaire.ModifiedDate = DateTime.Now;
                    questionnaire.CreatedBy = User.Claims.Select(x => x.Value).First();
                    questionnaire.ModifiedBy = User.Claims.Select(x => x.Value).First();
                    _context.Update(questionnaire);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionnaireExists(questionnaire.PaquestionnaireId))
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
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name", questionnaire.CourseId);
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", questionnaire.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", questionnaire.ModifiedBy);
            ViewData["TrainingContentId"] = new SelectList(_context.PatrainingContentMaster, "TrainingContentId", "Name", questionnaire.PatrainingContentId);

            return View(questionnaire);
        }

        // GET: Questionnaires/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionnaire = await _context.Paquestionnaire
                .Include(q => q.Course)
                .Include(q => q.CreatedByNavigation)
                .Include(q => q.ModifiedByNavigation)
                .Include(q => q.PatrainingContent)
                .FirstOrDefaultAsync(m => m.PaquestionnaireId == id);
            if (questionnaire == null)
            {
                return NotFound();
            }

            return View(questionnaire);
        }

        // POST: Questionnaires/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var questionnaire = await _context.Paquestionnaire.FindAsync(id);
            _context.Paquestionnaire.Remove(questionnaire);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionnaireExists(Guid id)
        {
            return _context.Paquestionnaire.Any(e => e.PaquestionnaireId == id);
        }

        [HttpPost]
        public IActionResult Import(string fileName)
        {

            IFormFile file = Request.Form.Files[0];
            string folderName = "UploadQuestion";
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

                                Paquestionnaire qt = new Paquestionnaire();

                                qt.QuestionnaireType = worksheet.Cells[row, 1].Value.ToString();
                                qt.QuestionText = worksheet.Cells[row, 2].Value.ToString();
                                qt.Option1 = worksheet.Cells[row, 3].Value == null ? string.Empty : worksheet.Cells[row, 3].Value.ToString();
                                qt.Option2 = worksheet.Cells[row, 4].Value == null ? string.Empty : worksheet.Cells[row, 4].Value.ToString();

                                qt.Option3 = worksheet.Cells[row, 5].Value == null ? string.Empty : worksheet.Cells[row, 5].Value.ToString();


                                qt.Option4 = worksheet.Cells[row, 6].Value == null ? string.Empty : worksheet.Cells[row, 5].Value.ToString();
                                qt.CorrectAnswer = worksheet.Cells[row, 7].Value.ToString();
                                qt.IsActive = bool.Parse(worksheet.Cells[row, 8].Value.ToString());
                                qt.CourseId = Guid.Parse((worksheet.Cells[row, 9]).Value.ToString());
                                qt.PatrainingContentId = Guid.Parse((worksheet.Cells[row, 10]).Value.ToString());
                                qt.ModifiedDate = DateTime.Now;
                                qt.CreatedDate = DateTime.Now;
                                qt.CreatedBy = User.Claims.Select(x => x.Value).First();
                                qt.ModifiedBy = User.Claims.Select(x => x.Value).First();

                                _context.Paquestionnaire.Add(qt);
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

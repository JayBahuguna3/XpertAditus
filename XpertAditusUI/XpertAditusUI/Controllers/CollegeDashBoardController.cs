using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.DTO;
using XpertAditusUI.Models;

namespace XpertAditusUI.Controllers
{
    public class CollegeDashBoardController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly XpertAditusDbContext _context;

        public CollegeDashBoardController(UserManager<IdentityUser> userManager, XpertAditusDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public IActionResult CollegeDashBoard()
        {
            var userid = this._userManager.GetUserId(this.User);
            var collegeProfile = this._context.CollegeProfile.Where(u => u.LoginId == userid).FirstOrDefault();
            ViewBag.AppliedCountForCollege = _context.CollegeStudentMapping.Where(c => c.CollegeProfileId == collegeProfile.CollegeProfileId).Count();
            return View();            
        }
        [HttpGet]
        public async Task<IActionResult> AppliedStudentList()
        {
            var userid = this._userManager.GetUserId(this.User);
            var collegeProfileid = this._context.CollegeProfile.Where(u => u.LoginId == userid).FirstOrDefault();
            var Appliedforspecificcollege = (from us in _context.UserProfile
                                                 join cc in _context.CollegeStudentMapping on us.LoginId equals cc.LoginId
                                                 join col in _context.CollegeProfile on cc.CollegeProfileId equals col.CollegeProfileId
                                                 join cou in _context.CourseMaster on cc.CourseId equals cou.CourseId                                                 
                                                 where  col.CollegeProfileId == collegeProfileid.CollegeProfileId
                                                 select new AppliedStudentList()
                                                 {   RegistrationNumber =  us.RegistrationNumber,
                                                     FirstName = us.FirstName,
                                                     LastName = us.LastName,
                                                     MiddleName = us.MiddleName,
                                                     Email = us.Email,
                                                     MobileNumber = us.MobileNumber,
                                                     Dob = us.Dob,
                                                     CourseName = cou.Name,
                                                     UserProfileId = us.UserProfileId }).ToList();
            var admittedStudentList = _context.Admission.ToList();
            foreach(Admission ad in  admittedStudentList)
            {
                if (Appliedforspecificcollege.Where(e => e.UserProfileId == ad.UserProfileId).Count() > 0)
                    Appliedforspecificcollege.RemoveAll(e => e.UserProfileId == ad.UserProfileId);
            }
            ViewBag.Appliedforspecificcollege = Appliedforspecificcollege;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AdmissionForm(Guid id)
        {
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["UpdatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name");
            ViewBag.userProfile = _context.UserProfile.Where(u => u.UserProfileId == id).FirstOrDefault();         
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AdmissionForm([Bind("Id,UserProfileId,FirstName,MiddleName,LastName,MobileNumber,EmailAddress,FeeAmount,FeeMode,ReceiptNumber,PaidDate,PaidBankName,PaidBankIfsccode,CourseId,Comments,CollegeProfileId,CreatedBy,UpdatedBy,CreatedDate,UpdatedDate,IsActive")] Admission admission)
        {
            var userid = this._userManager.GetUserId(this.User);
            var colgProfile = this._context.CollegeProfile.Where(u => u.LoginId == userid).FirstOrDefault();

            if (ModelState.IsValid)
            {

                if(_context.Admission.Where(e => e.MobileNumber == admission.MobileNumber).Count() > 0)      
                {
                    ModelState.AddModelError(nameof(admission.MobileNumber), "Admission for this mobile number is already done.");
                }
                else
                {
                    admission.Id = Guid.NewGuid();
                    admission.CreatedDate = DateTime.Now;
                    admission.UpdatedDate = DateTime.Now;
                    admission.CollegeProfileId = colgProfile.CollegeProfileId;
                    admission.CreatedBy = User.Claims.Select(x => x.Value).First();
                    admission.UpdatedBy = User.Claims.Select(x => x.Value).First();
                    admission.IsActive = true;
                    _context.Add(admission);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(AppliedStudentList));
                }

            }
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["UpdatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["CourseId"] = new SelectList(_context.CourseMaster, "CourseId", "Name");
            ViewBag.userProfile = _context.UserProfile.Where(u => u.UserProfileId == admission.Id).FirstOrDefault();
            return View(admission);                
        }

        [HttpGet]
         public async Task<IActionResult> AdmittedStudentList()
        {
            var userid = this._userManager.GetUserId(this.User);
            var collegeProfileid = this._context.CollegeProfile.Where(u => u.LoginId == userid).FirstOrDefault();
            var AdmittedStudentList = _context.Admission
                .Include(d => d.Course)
                .Include(d => d.CreatedByNavigation)
                .Include(d => d.UpdatedByNavigation)
                .Where(col => col.CollegeProfileId == collegeProfileid.CollegeProfileId);
            return View(await AdmittedStudentList.ToListAsync());
        }
        [HttpGet]
         public async Task<IActionResult> AdmittedStudentDetail(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admission = await _context.Admission
                .Include(c => c.Course)
                .Include(c => c.CreatedByNavigation)
                .Include(c => c.UpdatedByNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admission == null)
            {
                return NotFound();
            }
            return View(admission);
        }



    }
}

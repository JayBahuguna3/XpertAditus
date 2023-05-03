using ClosedXML.Excel;
using CsvHelper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.Models;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    public class CollegeProfileController :  BaseController
    {
        private readonly UserProfileService _userProfileService;
        private readonly XpertAditusDbContext _context;


        public CollegeProfileController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, XpertAditusDbContext context,
            UserProfileService userProfileService) : base(logger, userManager)
        {
            _userProfileService = userProfileService;
            _context = context;
        }

        public IActionResult CollegeProfile()
        {
            var userid = this._userManager.GetUserId(this.User);
            CollegeProfile collegeProfile = _userProfileService.GetCollegeProfileInfo(userid);

            var Countries = _userProfileService.GetCountries().ToList();
            ViewData["Countries"] = Countries.Select(e => new SelectListItem(e.Name, e.CountryId.ToString())).ToList();

            var States = _userProfileService.GetStates().ToList();
            ViewData["States"] = States.Select(e => new SelectListItem(e.Name, e.StateId.ToString())).ToList();

            if (collegeProfile.StateId != null && collegeProfile.StateId != "")
            {
                var Districts = _userProfileService.GetDistricts(collegeProfile.StateId).ToList();
                ViewData["Districts"] = Districts.Select(e => new SelectListItem(e.Name, e.DistrictId.ToString())).ToList();
            }
            else
            {
                ViewData["Districts"] = null;
            }

            if (collegeProfile.DistrictId != null && collegeProfile.DistrictId != "")
            {
                var Cities = _userProfileService.GetCities(collegeProfile.DistrictId).ToList();
                ViewData["Cities"] = Cities.Select(e => new SelectListItem(e.Name, e.DistrictId.ToString())).ToList();
            }
            else
            {
                ViewData["Cities"] = null;
            }
            return View(collegeProfile);
        }

        [HttpPost]
        public async Task<ActionResult> CollegeProfile([FromForm] CollegeProfile collegeProfile)
        {
            var userid = this._userManager.GetUserId(this.User);
            _userProfileService.SaveCollegeProfileAsync(collegeProfile, userid);

            var Countries = _userProfileService.GetCountries().ToList();
            ViewData["Countries"] = Countries.Select(e => new SelectListItem(e.Name, e.CountryId.ToString())).ToList();

            var States = _userProfileService.GetStates().ToList();
            ViewData["States"] = States.Select(e => new SelectListItem(e.Name, e.StateId.ToString())).ToList();

            if(collegeProfile.StateId != null && collegeProfile.StateId != "")
            {
                var Districts = _userProfileService.GetDistricts(collegeProfile.StateId).ToList();
                ViewData["Districts"] = Districts.Select(e => new SelectListItem(e.Name, e.DistrictId.ToString())).ToList();
            }
            else
            {
                ViewData["Districts"] = null;
            }

            if (collegeProfile.DistrictId != null && collegeProfile.DistrictId != "")
            {
                var Cities = _userProfileService.GetCities(collegeProfile.DistrictId).ToList();
                ViewData["Cities"] = Cities.Select(e => new SelectListItem(e.Name, e.DistrictId.ToString())).ToList();
            }
            else
            {
                ViewData["Cities"] = null;
            }

            return View(collegeProfile);
        }


        [HttpGet("DeleteResume")]
        public async Task<IActionResult> DeleteAttachment()
        {
            var userid = this._userManager.GetUserId(this.User);
            _userProfileService.DeleteAttachment(userid);
            return RedirectToAction("CollegeProfile");
        }
        // GET: CollegeProfiles
        public async Task<IActionResult> Index()
        {

            List<CollegeProfile> collegeProfile = _context.CollegeProfile.ToList();
            return View(collegeProfile);
        }

        // GET: CollegeProfiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collegeProfile = await _context.CollegeProfile
                .Include(c => c.CreatedByNavigation)
                .Include(c => c.Login)
                .Include(c => c.UpdatedByNavigation)
                .FirstOrDefaultAsync(m => m.CollegeProfileId == id);
            if (collegeProfile == null)
            {
                return NotFound();
            }

            return View(collegeProfile);
        }

        // GET: CollegeProfiles/Create
        public IActionResult Create()
        {
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["LoginId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["UpdatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            ViewData["CountryId"] = new SelectList(_context.CountryMaster, "CountryId", "Name");
            ViewData["StateId"] = new SelectList(_context.StateMaster, "StateId", "Name");
            ViewData["CityId"] = new SelectList(_context.CityMaster, "CityId", "Name");
            ViewData["DistrictId"] = new SelectList(_context.DistrictMaster, "DistrictId", "Name");
            return View();
        }

        // POST: CollegeProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CollegeProfileId,Name,LoginId,IsActive,CollegeContact,CollegeEmail,LogoPath,AttachementPath,Reviews,Ratings,CollegeWebsiteLink,About,CollegeAddress,CityId,DistrictId,StateId,CountryId,CreatedDate,UpdatedDate,CreatedBy,UpdatedBy")] CollegeProfile collegeProfile)
        {
            if (ModelState.IsValid)
            {
                collegeProfile.CollegeProfileId = Guid.NewGuid();
                _context.Add(collegeProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", collegeProfile.CreatedBy);
            ViewData["LoginId"] = new SelectList(_context.AspNetUsers, "Id", "Id", collegeProfile.LoginId);
            ViewData["UpdatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", collegeProfile.UpdatedBy);
            return View(collegeProfile);
        }

        // GET: CollegeProfiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collegeProfile = await _context.CollegeProfile.FindAsync(id);
            if (collegeProfile == null)
            {
                return NotFound();
            }
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", collegeProfile.CreatedBy);
            ViewData["LoginId"] = new SelectList(_context.AspNetUsers, "Id", "Id", collegeProfile.LoginId);
            ViewData["UpdatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", collegeProfile.UpdatedBy);
            ViewData["CountryId"] = new SelectList(_context.CountryMaster, "CountryId", "Name");
            ViewData["StateId"] = new SelectList(_context.StateMaster, "StateId", "Name");
            ViewData["CityId"] = new SelectList(_context.CityMaster, "CityId", "Name");
            ViewData["DistrictId"] = new SelectList(_context.DistrictMaster, "DistrictId", "Name");
            return View(collegeProfile);
        }

        // POST: CollegeProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("CollegeProfileId,Name,LoginId,IsActive,CollegeContact,CollegeEmail,LogoPath,AttachementPath,Reviews,Ratings,CollegeWebsiteLink,About,CollegeAddress,CityId,DistrictId,StateId,CountryId,CreatedDate,UpdatedDate,CreatedBy,UpdatedBy")] CollegeProfile collegeProfile)
        {
            if (id != collegeProfile.CollegeProfileId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(collegeProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollegeProfileExists(collegeProfile.CollegeProfileId))
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
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "UserName", collegeProfile.CreatedBy);
            ViewData["LoginId"] = new SelectList(_context.AspNetUsers, "Id", "UserName", collegeProfile.LoginId);
            ViewData["UpdatedBy"] = new SelectList(_context.AspNetUsers, "Id", "UserName", collegeProfile.UpdatedBy);
            ViewData["CountryId"] =  new SelectList(_context.CountryMaster, "CountryId", "Name");
            ViewData["StateId"] = new SelectList(_context.StateMaster, "StateId", "Name");
            ViewData["CityId"] = new SelectList(_context.CityMaster, "CityId", "Name");
            ViewData["DistrictId"] = new SelectList(_context.DistrictMaster, "DistrictId", "Name");
            return View(collegeProfile);
        }

        // GET: CollegeProfiles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collegeProfile = await _context.CollegeProfile
                .Include(c => c.CreatedByNavigation)
                .Include(c => c.Login)
                .Include(c => c.UpdatedByNavigation)
                .FirstOrDefaultAsync(m => m.CollegeProfileId == id);
            if (collegeProfile == null)
            {
                return NotFound();
            }

            return View(collegeProfile);
        }

        // POST: CollegeProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var collegeProfile = await _context.CollegeProfile.FindAsync(id);
            _context.CollegeProfile.Remove(collegeProfile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CollegeProfileExists(Guid id)
        {
            return _context.CollegeProfile.Any(e => e.CollegeProfileId == id);
        }
        // GET: RegisteredCollegesList
        // Checkbox filter
        [HttpGet]
        public async Task<IActionResult> RegisteredColleges(string GetAllColleges)
        {
            if(GetAllColleges != "on")
            {
                List<CollegeProfile> collegeProfile = _context.CollegeProfile.Where(c => c.IsActive == true).ToList();
                ViewBag.CheckStatus = false;
                return View(collegeProfile);
            }
            else
            {
                List<CollegeProfile> collegeProfile = _context.CollegeProfile.ToList();
                ViewBag.CheckStatus = true;
                return View(collegeProfile);
            }
            
        }
        // GET: RegisteredCollegesList/ViewDetails/5
        public async Task<IActionResult> ViewDetails(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collegeProfile = await _context.CollegeProfile
                .Include(c => c.CreatedByNavigation)
                .Include(c => c.Login)
                .Include(c => c.UpdatedByNavigation)
                .FirstOrDefaultAsync(m => m.CollegeProfileId == id);
            if (collegeProfile == null)
            {
                return NotFound();
            }

            return View(collegeProfile);
        }

        // Export RegisteredCollegesList
        public async Task<IActionResult> Export_CollegesList()
        {
            var collegeProfile = _context.CollegeProfile.Where(c => c.IsActive == true).ToList();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("CollegeProfileId,Name,IsActive,CollegeContact,CollegeEmail,LogoPath,AttachementPath,CollegeWebsiteLink,About,CollegeAddress,AttachementPath,DistrictId,StateId,CountryId,CreatedDate");
            foreach (var colgProfile in collegeProfile)
            {
                stringBuilder.AppendLine(
                    $"{colgProfile.CollegeProfileId}, {colgProfile.Name}, {colgProfile.IsActive}, {colgProfile.CollegeContact},{colgProfile.CollegeEmail},{colgProfile.LogoPath},{colgProfile.AttachementPath},{colgProfile.CollegeWebsiteLink},{colgProfile.About},{colgProfile.CollegeAddress},{colgProfile.AttachementPath},{colgProfile.DistrictId},{colgProfile.StateId},{colgProfile.CountryId},{colgProfile.CreatedDate}"
                    );
            }
            return File(Encoding.UTF8.GetBytes(stringBuilder.ToString()), "text/csv", "RegisteredCollegesList_Report.csv");
        }
    }
}

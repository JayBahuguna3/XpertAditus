using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class CompanyProfileController : BaseController
    {
        private readonly UserProfileService _userProfileService;
        private readonly XpertAditusDbContext _context;

        public CompanyProfileController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, XpertAditusDbContext context,
            UserProfileService userProfileService) :base(logger,userManager)
        {
            _userProfileService = userProfileService;
            _context = context;
        }

        public IActionResult CompanyProfile()
        {
            var userid = this._userManager.GetUserId(this.User);
            CompanyProfile companyProfile = _userProfileService.GetCompanyProfileInfo(userid);

            var Countries = _userProfileService.GetCountries().ToList();
            ViewData["Countries"] = Countries.Select(e => new SelectListItem(e.Name, e.CountryId.ToString())).ToList();

            var States = _userProfileService.GetStates().ToList();
            ViewData["States"] = States.Select(e => new SelectListItem(e.Name, e.StateId.ToString())).ToList();

            if (companyProfile.StateId != null && companyProfile.StateId != "")
            {
                var Districts = _userProfileService.GetDistricts(companyProfile.StateId).ToList();
                ViewData["Districts"] = Districts.Select(e => new SelectListItem(e.Name, e.DistrictId.ToString())).ToList();
            }
            else
            {
                ViewData["Districts"] = null;
            }

            if (companyProfile.DistrictId != null && companyProfile.DistrictId != "")
            {
                var Cities = _userProfileService.GetCities(companyProfile.DistrictId).ToList();
                ViewData["Cities"] = Cities.Select(e => new SelectListItem(e.Name, e.DistrictId.ToString())).ToList();
            }
            else
            {
                ViewData["Cities"] = null;
            }

            return View(companyProfile);
        }


        [HttpPost]
        public async Task<ActionResult> CompanyProfile([FromForm] CompanyProfile companyProfile)
        {
            var userid = this._userManager.GetUserId(this.User);
            _userProfileService.SaveCompanyProfileAsync(companyProfile, userid);

            var Countries = _userProfileService.GetCountries().ToList();
            ViewData["Countries"] = Countries.Select(e => new SelectListItem(e.Name, e.CountryId.ToString())).ToList();

            var States = _userProfileService.GetStates().ToList();
            ViewData["States"] = States.Select(e => new SelectListItem(e.Name, e.StateId.ToString())).ToList();

            if (companyProfile.StateId != null && companyProfile.StateId != "")
            {
                var Districts = _userProfileService.GetDistricts(companyProfile.StateId).ToList();
                ViewData["Districts"] = Districts.Select(e => new SelectListItem(e.Name, e.DistrictId.ToString())).ToList();
            }
            else
            {
                ViewData["Districts"] = null;
            }

            if (companyProfile.DistrictId != null && companyProfile.DistrictId != "")
            {
                var Cities = _userProfileService.GetCities(companyProfile.DistrictId).ToList();
                ViewData["Cities"] = Cities.Select(e => new SelectListItem(e.Name, e.DistrictId.ToString())).ToList();
            }
            else
            {
                ViewData["Cities"] = null;
            }

            return View(companyProfile);
        }
    }
}

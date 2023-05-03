using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    public class GetAddressInfoController : Controller
    {
        private readonly UserProfileService _userProfileService;
        public GetAddressInfoController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
            UserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        public ActionResult GetCountries()
        {
            var Countries = _userProfileService.GetCountries();
            return new JsonResult(Countries);
        }

        public ActionResult GetStates(string Id)
        {
           var States =  _userProfileService.GetStates(Id);
           return new JsonResult(States);
        }

        public ActionResult GetDistricts(string Id)
        {
            var States = _userProfileService.GetDistricts(Id);
            return new JsonResult(States);
        }

        public ActionResult GetCities(string Id)
        {
            var Cities = _userProfileService.GetCities(Id);
            return new JsonResult(Cities);
        }
    }
}

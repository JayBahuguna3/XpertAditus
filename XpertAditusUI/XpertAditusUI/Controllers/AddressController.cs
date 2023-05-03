using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XpertAditusUI.Models;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : BaseController
    {
        private readonly UserProfileService _userProfileService;

        public AddressController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
            UserProfileService userProfileService) : base(logger, userManager)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet]
        public async Task<ActionResult> get()
        {
            var userid = this._userManager.GetUserId(this.User);
            var addressInfo = _userProfileService.GetUserAddressInfo(userid);
            dynamic OfficeAddress = "";
            dynamic HomeAddress = "";
            for (int i = 0; i <= addressInfo.Count - 1; i++)
            {
                if(addressInfo[i].AddressType == "Office")
                {
                    OfficeAddress = _userProfileService.GetUserOfficeAddressInfo(addressInfo[i].AddressId);
                }
                else if (addressInfo[i].AddressType == "Home")
                {
                    HomeAddress = _userProfileService.GetUserHomeAddressInfo(addressInfo[i].AddressId);
                }
            }
            return new JsonResult(new { addressInfo, OfficeAddress, HomeAddress });
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Address[] address)
        {
            var userid = this._userManager.GetUserId(this.User);
            _userProfileService.SaveUserAddress(address, userid);
            return new JsonResult("Success");
        }

    }
}

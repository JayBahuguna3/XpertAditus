using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
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
    public class RegistrationController : ControllerBase
    {

        private readonly RegistrationService _registrationService;
        private IConfiguration _configuration;
        private bool Error;
        private string Message;
        readonly UtilityService _utilityService;

        public RegistrationController(RegistrationService registrationService, UtilityService utilityService, IConfiguration configuration)
        {
            _registrationService = registrationService;
            _configuration = configuration;
            _utilityService = utilityService;
        }

        //[HttpGet]
        //public async Task<ActionResult> get()
        //{
        //    string msg = string.Format(_configuration["SendMessage:Msg_VerifyMobileNumber"].ToString(),"1234");
        //    string InfoBipprincipalEntityId = _configuration["SendMessage:InfoBipprincipalEntityId"].ToString();
        //     string InfoBipcontentTemplateId = _configuration["SendMessage:Temp_VerifyMobileNumber"].ToString();
        //    _utilityService.SendSMS( msg, "9921886835", InfoBipprincipalEntityId, InfoBipcontentTemplateId);
        //    return new JsonResult(false);
        //}

        [HttpPost]
        public async Task<ActionResult> Post(Registration registrationinfo)
        {
            try
            {
                if (registrationinfo.validationType == "Generate")
                {
                    string result = await _registrationService.VerifyUser(registrationinfo);

                    if (result != "Verify")
                    {
                        return new JsonResult(result.Remove(result.Length - 1, 1).Split(','));
                    }

                    Registration registration = new Registration();

                    long mobileOtp = _utilityService.GenerateOtp();

                    //long emailOtp = _utilityService.GenerateOtp();


                    _utilityService.SaveOtp(registrationinfo.UserName, long.Parse(registrationinfo.MobileNo), registrationinfo.Email, mobileOtp/*emailOtp*/);


                    //await _utilityService.SendMail(registrationinfo.FirstName, registrationinfo.Email, emailOtp);

                    string msg = string.Format(_configuration["SendMessage:Msg_VerifyMobileNumber"].ToString(), mobileOtp);
                    string InfoBipprincipalEntityId = _configuration["SendMessage:InfoBipprincipalEntityId"].ToString();
                    string InfoBipcontentTemplateId = _configuration["SendMessage:Temp_VerifyMobileNumber"].ToString();

                    _utilityService.SendSMS(msg, registrationinfo.MobileNo, InfoBipprincipalEntityId, InfoBipcontentTemplateId);

                    return new JsonResult(true);
                }
                else if (registrationinfo.validationType == "Check")
                {
                    string otpresult = _registrationService.VerifyOtp(registrationinfo.UserName, long.Parse(registrationinfo.MobileNo), registrationinfo.Email, Convert.ToInt32(registrationinfo.MobileOtp)/*, Convert.ToInt32(registrationinfo.EmailOtp)*/);

                    if (otpresult == "Correct")
                    {
                        var result = await _registrationService.CreateUserAsync(registrationinfo);
                        string Userid = _registrationService.GetUserId(registrationinfo.UserName);
                        if (result.Succeeded)
                        {
                            await _utilityService.SendMail(registrationinfo.FirstName, registrationinfo.Email, registrationinfo.UserName, registrationinfo.Password);

                            Registration registration = new Registration();
                            _registrationService.SaveUserInfo(registrationinfo, Userid);
                            if(registrationinfo.Type == "College")
                            {
                                _registrationService.SaveCollegeInfo(registrationinfo, Userid);
                            }
                            else if (registrationinfo.Type == "Company")
                            {
                                _registrationService.SaveCompanyInfo(registrationinfo, Userid);
                            }
                            return new JsonResult(true);
                        }

                        else
                        {
                            return new JsonResult(result.ToString());
                        }
                        
                    }
                    else
                    {
                        return new JsonResult(otpresult);
                    }
                }
                else
                {
                    return new JsonResult("Invalid API");

                }
            }
            catch(Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }
    }
}
    
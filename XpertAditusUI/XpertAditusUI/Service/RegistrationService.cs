using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using XpertAditusUI.Data;
using XpertAditusUI.Models;

namespace XpertAditusUI.Service
{
    public class RegistrationService
    {
        private XpertAditusDbContext _XpertAditusDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        public RegistrationService(UserManager<IdentityUser> userManager, XpertAditusDbContext XpertAditusDbContext)
        {
            _XpertAditusDbContext = XpertAditusDbContext;
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateUserAsync(Registration registration)
        {
            var user = new IdentityUser()
            {
                UserName = registration.UserName,
                Email = registration.Email,
                EmailConfirmed = true,
                PhoneNumber = registration.MobileNo,
                PhoneNumberConfirmed = true,
                LockoutEnabled = true,
            };

            var result = await _userManager.CreateAsync(user, registration.Password);
            //Adding role
            await _userManager.AddToRoleAsync(user, registration.Type);
            return result;
        }

        public string VerifyOtp(string userName, long mobileNo, string email, int mobileOtp /*int emailOtp*/)
        {

            var otpInfo = _XpertAditusDbContext.OtpMaster.Where(a => a.Username == userName).Take(2).OrderByDescending(a => a.CreatedDate).ToList();
            int mobileResultOtp = otpInfo.Where(r => r.MobileNo == mobileNo && r.Otp == mobileOtp).Count();
            // int emailResultOtp = otpInfo.Where(r => r.EmailId == email && r.Otp == emailOtp).Count();
            string result = "";
            if (mobileResultOtp == 1 /*&& emailResultOtp == 1*/)
            {
                for (int i = 0; i < otpInfo.Count; i++)
                {
                    //if (otpInfo[i].EmailId != null)
                    //{
                    //    var mobileOtpIsVerified = otpInfo.Where(r => r.EmailId == email && r.Otp == emailOtp).FirstOrDefault();
                    //    mobileOtpIsVerified.IsVerified = "Yes";
                    //    _XpertAditusDbContext.SaveChanges();
                    //}
                    if (otpInfo[i].MobileNo != null)
                    {
                        var emailOtpIsVerified = otpInfo.Where(r => r.MobileNo == mobileNo && r.Otp == mobileOtp).FirstOrDefault();
                        emailOtpIsVerified.IsVerified = "Yes";
                        _XpertAditusDbContext.SaveChanges();
                    }
                }
                result = "Correct";
            }
            else
            {
                if (mobileResultOtp == 0)
                {
                    result = "Mobile Otp is Wrong." + "<br>";
                }

                //if (emailResultOtp == 0)
                //{
                //    result += "Email Otp is Wrong." + "<br>";
                //}
            }
            return result;
        }

        public async Task<string> VerifyUser(Registration registration)
        {
            int checkUserName = _userManager.Users.Where(a => a.UserName == registration.UserName).Count();
            int checkEmail = _userManager.Users.Where(a => a.Email == registration.Email).Count();
            int checkPhoneNo = _userManager.Users.Where(a => a.PhoneNumber == registration.MobileNo).Count();
            String result = "";

            if (checkUserName == 0 && checkEmail == 0 && checkPhoneNo == 0)
            {
                result = "Verify";
            }
            else
            {
                if (checkUserName != 0)
                {
                    result += "This UserName already used." + "<br>";
                }
                if (checkEmail != 0)
                {
                    result += "This Email already used." + "<br>";
                }
                if (checkPhoneNo != 0)
                {
                    result += "This Mobile No already used." + "<br>";
                }
            }
            return result;
        }

        public string GetUserId(string userName)
        {
            string UserId = _userManager.Users.Where(a => a.UserName == userName).Select(a => a.Id).FirstOrDefault();
            return UserId;
        }

        //public async Task<bool> GotoLoginPageAsync(string userName)
        //{
        //    bool loginAllow = false;
        //    var UserId = _userManager.Users.Where(a => a.UserName == userName).FirstOrDefault();
        //    var result = await _signInManager.PasswordSignInAsync(UserId.UserName, UserId.PasswordHash, UserId.LockoutEnabled, lockoutOnFailure: false);
        //    if (result.Succeeded)
        //    {
        //        loginAllow = true;
        //    }
        //    else
        //    {
        //        loginAllow = false;
        //    }
        //    return loginAllow;
        //}

        public long GenerateRegistrationNumber()
        {

            if (_XpertAditusDbContext.UserProfile.Count() > 0)
            {
                return _XpertAditusDbContext.UserProfile.OrderByDescending(e => e.RegistrationNumber).First().RegistrationNumber + 1;
            }
            else
            {
                return 1;
            }

        }

        public void SaveUserInfo(Registration registration, string userId)
        {
            UserProfile userProfile = new UserProfile();
            userProfile.UserProfileId = Guid.NewGuid();
            userProfile.LoginId = userId;
            userProfile.RegistrationNumber = GenerateRegistrationNumber();
            userProfile.UserProfileType = registration.Type;
            userProfile.FirstName = registration.FirstName;
            userProfile.MiddleName = registration.MiddleName;
            userProfile.LastName = registration.Lastname;
            userProfile.MobileNumber = long.Parse(registration.MobileNo);
            userProfile.Email = registration.Email;
            userProfile.Dob = DateTime.Now;
            _XpertAditusDbContext.UserProfile.Add(userProfile);
            _XpertAditusDbContext.SaveChanges();
        }

        public void SaveCollegeInfo(Registration registration, string userId)
        {
            CollegeProfile collegeProfile = new CollegeProfile();
            collegeProfile.CollegeProfileId = Guid.NewGuid();
            collegeProfile.LoginId = userId;
            collegeProfile.IsActive = true;
            collegeProfile.CreatedBy = userId;
            collegeProfile.CreatedDate = DateTime.Now;
            _XpertAditusDbContext.CollegeProfile.Add(collegeProfile);
            _XpertAditusDbContext.SaveChanges();
        }


        public void SaveCompanyInfo(Registration registration, string userId)
        {
            CompanyProfile companyProfile = new CompanyProfile();
            companyProfile.CompanyProfileId = Guid.NewGuid();
            companyProfile.LoginId = userId;
            companyProfile.IsActive = true;
            companyProfile.CreatedBy = userId;
            companyProfile.CreatedDate = DateTime.Now;
            _XpertAditusDbContext.CompanyProfile.Add(companyProfile);
            _XpertAditusDbContext.SaveChanges();
        }
    }
}

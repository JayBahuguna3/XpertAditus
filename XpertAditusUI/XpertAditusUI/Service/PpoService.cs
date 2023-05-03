using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.Models;

namespace XpertAditusUI.Service
{
    public class PpoService
    {
        private XpertAditusDbContext _XpertAditusDbContext;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<IdentityUser> _userManager;
        public PpoService(UserManager<IdentityUser> userManager, XpertAditusDbContext XpertAditusDbContext, IWebHostEnvironment env)
        {
            _XpertAditusDbContext = XpertAditusDbContext;
            _userManager = userManager;
            _env = env;
        }

        public bool checkPpoCertificate(UserProfile UserProfile)
        {
            bool IsActive = false;
            var ppoInfo = _XpertAditusDbContext.PpoInfo.Where(p => p.UserProfileId == UserProfile.UserProfileId).FirstOrDefault();
            if(ppoInfo != null)
            {
                IsActive = true;
            }
            return IsActive;
        }
        
        public PpoInfo GetPpoInfo(UserProfile UserProfile)
        {
            var ppoInfo = _XpertAditusDbContext.PpoInfo.Where(p => p.UserProfileId == UserProfile.UserProfileId).FirstOrDefault();
            return ppoInfo;
        }

    }
}

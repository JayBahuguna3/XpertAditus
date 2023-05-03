using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.Models;

namespace Microsoft.AspNetCore.Identity
{
    public static class ExtensionMethods
    {
        public static UserProfile GetUserProfile(this UserManager<IdentityUser> userManager, ClaimsPrincipal User, XpertAditusDbContext XpertAditusDbContext)
        {

            var userid = userManager.GetUserId(User);
            if (userid != null)
                return XpertAditusDbContext.UserProfile.Where(u => u.LoginId == userid).FirstOrDefault();
            else
                return null;
        }

        public static bool DeleteFileFromFolder(string path)
        {
            try
            {

                // Check if file exists with its full path    
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    return true;
                }
            }
            catch (IOException ioExp)
            {
                Console.WriteLine(ioExp.Message);
            }
            return false;
        }
    }
}

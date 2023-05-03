using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using XpertAditusUI.Data;

namespace XpertAditusUI.Models
{
    public class Registration
    {
        public string Type { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string MobileNo { get; set; }
        public string validationType { get; set; }
        public string MobileOtp { get; set; }
        public string EmailOtp { get; set; }

       
        

      
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.Models;

namespace XpertAditusUI.Service
{
    public class UtilityService
    {
        private IConfiguration _configuration;
        private IWebHostEnvironment _hostEnvironment;
        private XpertAditusDbContext _XpertAditusDbContext;       
        public UtilityService(IConfiguration Configuration, XpertAditusDbContext XpertAditusDbContext, IWebHostEnvironment environment)
        {
            _configuration = Configuration;
            _hostEnvironment = environment;
            _XpertAditusDbContext = XpertAditusDbContext;
        }
        public long GenerateOtp()
        {
            return new Random().Next(1000, 9999);
        }
        public async Task<bool> SendMail(string firstName, string email, string userName, string Password)
        {
            string filePath = Path.Combine(_hostEnvironment.ContentRootPath,
                          "RegistrationEmail_Template\\RegistrationEmail.html");
            StreamReader str = new StreamReader(filePath);
            string MailText = str.ReadToEnd();
            str.Close();
            MailMessage mailMessage = new MailMessage(_configuration["SMTPConfig:SenderAddress"].ToString(), email);
            mailMessage.Subject = "Registration Verification";
            mailMessage.Body = MailText
              .Replace("{Name}", firstName.ToString())
              .Replace("{URL}", _configuration["SMTPConfig:URL"].ToString())
            .Replace("{Password}", Password.ToString())
             .Replace("{UserName}", userName.ToString());
            //"Dear " + firstName + ",<br><br>" +
            //"Hello," + "<br><br>" +
            //"Many thanks for creating an account with AIMS - AFIML. We are more than excited to welcome you on board. You can access your account area and take online courses at: https://courses.XpertAditus.in. To start a new course, simply select it from our course list. After the checkout process, you will have instant course access and can study at your own pace. You will receive your personal certificate at the end of the course." + "<br><br>" +
            //"Your UserName :-" + userName + "<br><br>" +
            //"Your Password:- " + Password + "<br><br>" +
            ////"Please use OTP " + otp.ToString() + " to verify your email address for registration at." +
            //"Best regards from the AarohiinfoTeam";
            mailMessage.IsBodyHtml = Convert.ToBoolean(_configuration["SMTPConfig:IsBodyHTML"]);

            SmtpClient smtpClient = new SmtpClient(_configuration["SMTPConfig:host"].ToString(), int.Parse(_configuration["SMTPConfig:Port"]));
            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = _configuration["SMTPConfig:UserName"].ToString(),
                Password = _configuration["SMTPConfig:Password"].ToString()
            };
            smtpClient.EnableSsl = Convert.ToBoolean(_configuration["SMTPConfig:EnableSSL"]);
            smtpClient.Send(mailMessage);
            return true;
        }

        public async Task<bool> SendMailForForgotPassword( string email, string Subject, string html)
        {
            MailMessage mailMessage = new MailMessage(_configuration["SMTPConfig:SenderAddress"].ToString(), email);
            mailMessage.Subject = Subject;
            mailMessage.Body = html;
        
            mailMessage.IsBodyHtml = Convert.ToBoolean(_configuration["SMTPConfig:IsBodyHTML"]);

            SmtpClient smtpClient = new SmtpClient(_configuration["SMTPConfig:host"].ToString(), int.Parse(_configuration["SMTPConfig:Port"]));
            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = _configuration["SMTPConfig:UserName"].ToString(),
                Password = _configuration["SMTPConfig:Password"].ToString()
            };
            smtpClient.EnableSsl = Convert.ToBoolean(_configuration["SMTPConfig:EnableSSL"]);
            smtpClient.Send(mailMessage);
            return true;
        }
        public void SendSMS(string msg, string mo, string InfoBipprincipalEntityId, string InfoBipcontentTemplateId)
        {
            if (_configuration["SendMessage:InfoBip"].ToLower() == "true")
            {
                var client = new RestClient(_configuration["SendMessage:InfoBipURL"].ToString());
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", _configuration["SendMessage:InfoBipHash"].ToString());
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Accept", "application/json");
                request.AddParameter("application/json", "{ \"messages\": [ {\"from\": \"" + _configuration["SendMessage:InfoBipRegisterKey"].ToString() + "\", \"destinations\": [{\"to\": \"91" + mo + "\"}], \"text\": \"" + msg + "\", \"regional\":{ \"indiaDlt\":{ \"contentTemplateId\": \"" + InfoBipcontentTemplateId + "\", \"principalEntityId\": \"" + InfoBipprincipalEntityId + "\" } } }]}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
            }
        }

        public void SaveOtp(string userName, long mobileNo, string email, long mobileOtp /*long emailOtp*/)
        {
            bool mobileOtpSave = true;
            //bool emailOtpSave = true;

            if (mobileOtpSave)
            {
                OtpMaster otpMaster = new OtpMaster();
                otpMaster.OtpMasterId = Guid.NewGuid();
                otpMaster.Username = userName;
                otpMaster.MobileNo = mobileNo;
                otpMaster.Otp = mobileOtp;
                otpMaster.CreatedDate = DateTime.Now;
                _XpertAditusDbContext.OtpMaster.Add(otpMaster);
            }
            //if (emailOtpSave)
            //{
            //    OtpMaster otpMaster = new OtpMaster();
            //    otpMaster.OtpMasterId = Guid.NewGuid();
            //    otpMaster.Username = userName;
            //    otpMaster.EmailId = email;
            //    otpMaster.Otp = emailOtp;
            //    otpMaster.CreatedDate = DateTime.Now;
            //    _XpertAditusDbContext.OtpMaster.Add(otpMaster);
            //}
            _XpertAditusDbContext.SaveChanges();

        }
        public async Task<bool> SendMail_ForgotUserName(string email, string Subject, string html)
        {
            MailMessage mailMessage = new MailMessage(_configuration["SMTPConfig:SenderAddress"].ToString(), email);
            mailMessage.Subject = Subject;
            mailMessage.Body = html;

            mailMessage.IsBodyHtml = Convert.ToBoolean(_configuration["SMTPConfig:IsBodyHTML"]);

            SmtpClient smtpClient = new SmtpClient(_configuration["SMTPConfig:host"].ToString(), int.Parse(_configuration["SMTPConfig:Port"]));
            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = _configuration["SMTPConfig:UserName"].ToString(),
                Password = _configuration["SMTPConfig:Password"].ToString()
            };
            smtpClient.EnableSsl = Convert.ToBoolean(_configuration["SMTPConfig:EnableSSL"]);
            smtpClient.Send(mailMessage);
            return true;
        }
    }
}

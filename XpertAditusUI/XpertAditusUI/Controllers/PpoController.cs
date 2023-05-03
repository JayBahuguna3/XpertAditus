using AutoMapper.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.Models;
using XpertAditusUI.Service;


namespace XpertAditusUI.Controllers
{
    public class PpoController : CandidateController
    {
        private readonly XpertAditusDbContext _db;
        private PpoService _ppoService;
        private readonly IHostingEnvironment _hostingEnvironment;


        public PpoController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
            XpertAditusDbContext db, PpoService ppoService,
            CandidateService candidateService, IHostingEnvironment hostingEnvironment) :
            base(logger, userManager, candidateService)
        {
            _db = db;
            _ppoService = ppoService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Ppo()
        {
            this.InitializeViewBag();
            UserProfile userProfile = ViewBag.UserProfile;
            if (checkPPOEnableForCollege(userProfile))
            {
                ViewBag.candidateResultStatus = null;
            }
            else
            {
                //ViewData["IsAllow"]  = _ppoService.checkPpoCertificate(userProfile);
                var userCourse = _db.UserCourses.Where(r => r.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
                if (userCourse != null)
                {
                    var candidateresult = this._db.CandidateResult.Where(e => e.UserCoursesId == userCourse.UserCoursesId).OrderByDescending(e => e.CreatedDate).FirstOrDefault();
                    if (candidateresult.IsCleared == true && candidateresult.Score != null)
                    {
                        ViewBag.candidateResultStatus = candidateresult;

                        string filename = userProfile.FirstName + "_" + userProfile.UserProfileId + "_Ppo.pdf";
                        string filepath = Path.Combine(_hostingEnvironment.WebRootPath, "Ppo\\Ppo.docx");
                        string savepath = Path.Combine(_hostingEnvironment.WebRootPath, "Ppo\\" + filename);
                        if (System.IO.File.Exists(savepath))
                        {

                        }
                        else
                        {
                            UserCourses userCourses = _db.UserCourses.Where(r => r.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
                            CandidateResult candidateResult = _db.CandidateResult
                                 .Where(r => r.IsCompleted == true && r.IsCleared == true && r.UserCoursesId == userCourses.UserCoursesId).
                                 FirstOrDefault();
                            Address address = _db.Address.Where(r => r.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
                            var ppoInfo = _ppoService.GetPpoInfo(userProfile);
                            if (ppoInfo == null)
                            {
                                List<PpoLogic> ppoLogics = _db.PpoLogic.Where(r => r.IsActive == true).ToList();
                                for (int i = 0; i < ppoLogics.Count; i++)
                                {
                                    if (candidateResult.Score >= ppoLogics[i].StartScore || candidateResult.Score <= ppoLogics[i].EndScore)
                                    {
                                        PpoInfo ppoInfo1 = new PpoInfo();
                                        ppoInfo1.Id = Guid.NewGuid();
                                        ppoInfo1.UserProfileId = userProfile.UserProfileId;
                                        ppoInfo1.Score = candidateResult.Score;
                                        ppoInfo1.Position = ppoLogics[i].Position;
                                        ppoInfo1.Salary = ppoLogics[i].Salary;
                                        ppoInfo1.IsDownloded = true;
                                        ppoInfo1.PpoLogicId = ppoLogics[i].PpoLogicId;
                                        ppoInfo1.CreatedDate = DateTime.Now;
                                        ppoInfo1.CreatedBy = this._userManager.GetUserId(this.User);
                                        _db.Add(ppoInfo1);
                                        _db.SaveChanges();
                                        ppoInfo = _ppoService.GetPpoInfo(userProfile);
                                        break;
                                    }
                                }
                            }

                            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                            TextInfo textInfo = cultureInfo.TextInfo;

                            Document doc = new Document();
                            doc.LoadFromFile(filepath);

                            doc.Replace("[ADDRESS]", address.Line1 + "" + address.Line2 + "" + address.Line3, true, true);
                            doc.Replace("[DATE]", DateTime.Today.ToShortDateString(), true, true);

                            if (userProfile.MiddleName != null)
                                doc.Replace("[CANDIDATENAME]", userProfile.FirstName + "  " + userProfile.MiddleName + "  " + userProfile.LastName, true, true);
                            else
                                doc.Replace("[CANDIDATENAME]", userProfile.FirstName + "  " + userProfile.LastName, true, true);

                            //doc.Replace("[SCORE]", ppoInfo.Score.ToString(), true, true);
                            doc.Replace("[POSITION]", ppoInfo.Position, true, true);
                            doc.Replace("[SALARY]", ppoInfo.Salary.ToString(), true, true);
                            doc.Watermark = null;
                            doc.SaveToFile(savepath, FileFormat.PDF);
                        }
                        ViewBag.PdfFilePath = @"/Ppo/" + filename;
                    }
                }

            }
            return View();
        }

        public IActionResult DownLoadCertificate()
        {
            this.InitializeViewBag();
            UserProfile userProfile = ViewBag.UserProfile;
            UserCourses userCourses = _db.UserCourses.Where(r => r.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
            CandidateResult candidateResult = _db.CandidateResult
                 .Where(r => r.IsCompleted == true && r.IsCleared == true && r.UserCoursesId == userCourses.UserCoursesId).
                 FirstOrDefault();
            Address address = _db.Address.Where(r => r.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
            var ppoInfo = _ppoService.GetPpoInfo(userProfile);
            if (ppoInfo == null)
            {
                List<PpoLogic> ppoLogics = _db.PpoLogic.Where(r => r.IsActive == true).ToList();
                for (int i = 0; i < ppoLogics.Count; i++)
                {
                    if (candidateResult.Score >= ppoLogics[i].StartScore && candidateResult.Score <= ppoLogics[i].EndScore)
                    {
                        PpoInfo ppoInfo1 = new PpoInfo();
                        ppoInfo1.Id = Guid.NewGuid();
                        ppoInfo1.UserProfileId = userProfile.UserProfileId;
                        ppoInfo1.Score = candidateResult.Score;
                        ppoInfo1.Position = ppoLogics[i].Position;
                        ppoInfo1.Salary = ppoLogics[i].Salary;
                        ppoInfo1.IsDownloded = true;
                        ppoInfo1.PpoLogicId = ppoLogics[i].PpoLogicId;
                        ppoInfo1.CreatedDate = DateTime.Now;
                        ppoInfo1.CreatedBy = this._userManager.GetUserId(this.User);
                        _db.Add(ppoInfo1);
                        _db.SaveChanges();
                        ppoInfo = _ppoService.GetPpoInfo(userProfile);
                        break;
                    }
                }
            }


            var memory = new MemoryStream();

            string filename = userProfile.FirstName + "_" + userProfile.UserProfileId + "_Ppo.pdf";
            string filepath = Path.Combine(_hostingEnvironment.WebRootPath, "Ppo\\Ppo.docx");
            string savepath = Path.Combine(_hostingEnvironment.WebRootPath, "Ppo\\" + filename);

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            Document doc = new Document();
            doc.LoadFromFile(filepath);

            doc.Replace("[CANDIDATEFIRSTNAME]", textInfo.ToTitleCase(userProfile.FirstName), true, true);
            doc.Replace("[CANDIDATELASTNAME]", userProfile.LastName, true, true);

            doc.Replace("[ADDRESS]", address.Line1 + "" + address.Line2 + "" + address.Line3, true, true);
            doc.Replace("[DATE]", DateTime.Today.ToShortDateString(), true, true);

            if (userProfile.MiddleName != null)
                doc.Replace("[CANDIDATENAME]", userProfile.FirstName + "  " + userProfile.MiddleName + "  " + userProfile.LastName, true, true);

            //doc.Replace("[SCORE]", ppoInfo.Score.ToString(), true, true);
            doc.Replace("[POSITION]", ppoInfo.Position, true, true);
            doc.Replace("[SALARY]", ppoInfo.Salary.ToString(), true, true);

            doc.Watermark = null;


            doc.SaveToFile(savepath, FileFormat.PDF);

            Stream stream = System.IO.File.OpenRead(savepath);

            memory.Position = 0;

            return File(stream, "application/pdf", filename);
        }

        public bool checkPPOEnableForCollege(UserProfile userProfile)
        {
            /**
               Query to add Column in CollegeProfile table
               Alter table CollegeProfile
               Add EnablePPOFlag bit
            */
            //UserProfile userProfile1 = _db.UserProfile.Where(r => r.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
            CollegeStudentMapping collegeStudentMapping = _db.CollegeStudentMapping.Where(r => r.LoginId == userProfile.LoginId).FirstOrDefault();
            if(collegeStudentMapping == null)
            {
                return true;
            }
            else
            {
                CollegeProfile collegeProfile = _db.CollegeProfile.Where(r => r.CollegeProfileId == collegeStudentMapping.CollegeProfileId).FirstOrDefault();
                //Enable means disable
                if (collegeProfile.EnablePpoflag == true)
                {
                    return true;
                }
            }
            
            
            return false;
        }

    }
}

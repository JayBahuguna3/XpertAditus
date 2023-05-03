using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XpertAditusUI.Data;
using XpertAditusUI.DTO;
using XpertAditusUI.Models;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    public class JobMastersController : BaseController
    {

        private readonly XpertAditusDbContext _context;
        private readonly UserProfileService _userProfileService;
        private readonly TestService _testService;
        private IConfiguration _configuration;

        public JobMastersController(XpertAditusDbContext context, UserProfileService userProfileService, TestService testService, IConfiguration configurationManager,
          ILogger<HomeController> logger, UserManager<IdentityUser> userManager)
         : base(logger, userManager)
        {
            _context = context;
            _userProfileService = userProfileService;
            _testService = testService;
            _configuration = configurationManager;
        }

        public IActionResult JobPostForm()
        {
            return View();
        }
        public IActionResult ManageJobs()
        {
            return View();
        }

        
            // GET: JobMasters
            public async Task<IActionResult> Index()
            {
                var userId = this._userManager.GetUserId(this.User);
                var userProfile = this._testService.GetUserProfile(userId);
                var xpertresourceDbContext = _context.JobMaster.Include(j => j.State)
                   .Include(j => j.District).Include(j => j.City).Include(j => j.CreatedByNavigation)
                   .Include(j => j.ModifiedByNavigation).Include(j => j.UserProfile)
                   .Where( j => j.CreatedBy == User.Claims.Select(x => x.Value).First());
                    

                return View(await xpertresourceDbContext.ToListAsync());
            }

            // GET: JobMasters/Details/5
            public async Task<IActionResult> Details(Guid? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var jobMaster = await _context.JobMaster
                    .Include(j => j.District)
                    .Include(j => j.State)
                    .Include(j => j.City)

                    .Include(j => j.CreatedByNavigation)
                    .Include(j => j.ModifiedByNavigation)
                    .Include(j => j.UserProfile)
                    .FirstOrDefaultAsync(m => m.JobId == id);
                    var userId = this._userManager.GetUserId(this.User);
                    var userProfile = this._testService.GetUserProfile(userId);
                    ViewBag.EmailId = userProfile.Email;
                    ViewBag.CompanyName = userProfile.CompanyName;
                if (jobMaster == null)
                {
                    return NotFound();
                }

                return View(jobMaster);
            }

            // GET: JobMasters/Create
            public IActionResult Create()
            {
                string a = _configuration.GetSection("WorkShift").GetSection("Slots").Value;
                string[] values = a.Split(',').Select(sValue => sValue.Trim()).ToArray();
                List<SelectListItem> dropDownsA = new List<SelectListItem>();
                for (int i = 0; i < values.Length; i++)
                {
                    dropDownsA.Add(new SelectListItem { Text = values[i], Value = values[i] });
                }
                ViewData["WorkShift"] = dropDownsA;


                string b = _configuration.GetSection("JobLevel").GetSection("Levels").Value;
                string[] values1 = b.Split(',').Select(sValue => sValue.Trim()).ToArray();
                List<SelectListItem> dropDownsB = new List<SelectListItem>();
                for (int i = 0; i < values1.Length; i++)
                {
                    dropDownsB.Add(new SelectListItem { Text = values1[i], Value = values1[i] });
                }
                ViewData["JobLevel"] = dropDownsB;



                string c = _configuration.GetSection("Industry").GetSection("I").Value;
                string[] values2 = c.Split(',').Select(sValue => sValue.Trim()).ToArray();
                List<SelectListItem> dropDownsC = new List<SelectListItem>();
                for (int i = 0; i < values2.Length; i++)
                {
                    dropDownsC.Add(new SelectListItem { Text = values2[i], Value = values2[i] });
                }
                ViewData["Industry"] = dropDownsC;



                string d = _configuration.GetSection("Category").GetSection("ctgry").Value;
                string[] values3 = d.Split(',').Select(sValue => sValue.Trim()).ToArray();
                List<SelectListItem> dropDownsD = new List<SelectListItem>();
                for (int i = 0; i < values3.Length; i++)
                {
                    dropDownsD.Add(new SelectListItem { Text = values3[i], Value = values3[i] });
                }
                ViewData["Category"] = dropDownsD;

          /*  ViewData[" EducationId"] = new SelectList(_context.EducationMaster, "EducationId", "Name");*/

                ViewData["StateId"] = new SelectList(_context.StateMaster, "StateId", "Name");
                ViewData["CityId"] = new SelectList(_context.CityMaster, "CityId", "Name");
                ViewData["DistrictId"] = new SelectList(_context.DistrictMaster, "DistrictId", "Name");
                ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
                ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id");
                //ViewData["UserProfileId"] = new SelectList(_context.UserProfile, "UserProfileId", "Email");
                var userId = this._userManager.GetUserId(this.User);
                var userProfile = this._testService.GetUserProfile(userId);
                ViewBag.EmailId = userProfile.Email;
                ViewBag.CompanyName = userProfile.CompanyName;
                return View();
            }

            // POST: JobMasters/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("JobId,UserProfileId,JobDesignation,CompanyName,CityId,StateId,DistrictId,MinExperience,MaxExperience,Description,LastDate,Ctc,ExpectedJoiningDate,NoOfOpening,Category,Industry,WorkShift,JobLevel,IsActive,ModifiedBy,ModifiedDate,CreatedDate,CreatedBy")] JobMaster jobMaster)
            {
                var userId = this._userManager.GetUserId(this.User);
                var userProfile = this._testService.GetUserProfile(userId);
                if (!ModelState.IsValid)
                {
                    jobMaster.JobId = Guid.NewGuid();
                    jobMaster.CreatedDate = DateTime.Now;
                    jobMaster.ModifiedDate = DateTime.Now;
                    jobMaster.CreatedBy = User.Claims.Select(x => x.Value).First();
                    jobMaster.ModifiedBy = User.Claims.Select(x => x.Value).First();

                    ViewData["StateId"] = new SelectList(_context.StateMaster, "StateId", "Name");
                    ViewData["CityId"] = new SelectList(_context.CityMaster, "CityId", "Name");
                    ViewData["DistrictId"] = new SelectList(_context.DistrictMaster, "DistrictId", "Name");
                    //ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", jobMaster.CreatedBy);
                    //ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", jobMaster.ModifiedBy);
                    _context.Add(jobMaster);

                    ViewBag.EmailId = userProfile.Email;
                    jobMaster.UserProfileId = userProfile.UserProfileId;
                   // jobMaster.CompanyName = userProfile.CompanyName;

                  /*    foreach(var jobQualification in _context.JobQualification)
                {
                    jobQualification.JobId = jobMaster.JobId;
                    jobQualification.EducationId = jobMaster.MinQualification.EducationId;
                    jobQualification.ModifiedBy = jobMaster.ModifiedBy;
                    jobQualification.CreatedBy = jobMaster.CreatedBy;
                    jobQualification.ModifiedDate = jobMaster.ModifiedDate;
                    jobQualification.CreatedDate = jobMaster.CreatedDate;
                    _context.Add(jobQualification);
                }*/
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                     return RedirectToPage("/JobMasters");
                }
                
                return View(jobMaster);
            }

            // GET: JobMasters/Edit/5
            public async Task<IActionResult> Edit(Guid? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var jobMaster = await _context.JobMaster.FindAsync(id);
                if (jobMaster == null)
                {
                    return NotFound();
                }
            string a = _configuration.GetSection("WorkShift").GetSection("Slots").Value;
            string[] values = a.Split(',').Select(sValue => sValue.Trim()).ToArray();
            List<SelectListItem> dropDownsA = new List<SelectListItem>();
            for (int i = 0; i < values.Length; i++)
            {
                dropDownsA.Add(new SelectListItem { Text = values[i], Value = values[i] });
            }
            ViewData["WorkShift"] = dropDownsA;


            string b = _configuration.GetSection("JobLevel").GetSection("Levels").Value;
            string[] values1 = b.Split(',').Select(sValue => sValue.Trim()).ToArray();
            List<SelectListItem> dropDownsB = new List<SelectListItem>();
            for (int i = 0; i < values1.Length; i++)
            {
                dropDownsB.Add(new SelectListItem { Text = values1[i], Value = values1[i] });
            }
            ViewData["JobLevel"] = dropDownsB;



            string c = _configuration.GetSection("Industry").GetSection("I").Value;
            string[] values2 = c.Split(',').Select(sValue => sValue.Trim()).ToArray();
            List<SelectListItem> dropDownsC = new List<SelectListItem>();
            for (int i = 0; i < values2.Length; i++)
            {
                dropDownsC.Add(new SelectListItem { Text = values2[i], Value = values2[i] });
            }
            ViewData["Industry"] = dropDownsC;



            string d = _configuration.GetSection("Category").GetSection("ctgry").Value;
            string[] values3 = d.Split(',').Select(sValue => sValue.Trim()).ToArray();
            List<SelectListItem> dropDownsD = new List<SelectListItem>();
            for (int i = 0; i < values3.Length; i++)
            {
                dropDownsD.Add(new SelectListItem { Text = values3[i], Value = values3[i] });
            }
            ViewData["Category"] = dropDownsD;
            ViewData["MinQualificationId"] = new SelectList(_context.EducationMaster, "EducationId", "Name");
            ViewData["StateId"] = new SelectList(_context.StateMaster, "StateId", "Name");
            ViewData["CityId"] = new SelectList(_context.CityMaster, "CityId", "Name");
            ViewData["DistrictId"] = new SelectList(_context.DistrictMaster, "DistrictId", "Name");
            ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", jobMaster.CreatedBy);
            ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", jobMaster.ModifiedBy);
            var userId = this._userManager.GetUserId(this.User);
            var userProfile = this._testService.GetUserProfile(userId);
            ViewBag.EmailId = userProfile.Email;
            ViewBag.CompanyName = userProfile.CompanyName;
            return View(jobMaster);
            }

            // POST: JobMasters/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(Guid id, [Bind("JobId,UserProfileId,JobDesignation,CompanyName,CityId,StateId,DistrictId,MinExperience,MaxExperience,Description,LastDate,Ctc,ExpectedJoiningDate,NoOfOpening,Category,Industry,WorkShift,JobLevel,IsActive,ModifiedBy,ModifiedDate,CreatedDate,CreatedBy")] JobMaster jobMaster)
            {
                if (id != jobMaster.JobId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        jobMaster.CreatedDate = DateTime.Now;
                        jobMaster.ModifiedDate = DateTime.Now;
                        jobMaster.CreatedBy = User.Claims.Select(x => x.Value).First();
                        jobMaster.ModifiedBy = User.Claims.Select(x => x.Value).First();
                        _context.Update(jobMaster);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!JobMasterExists(jobMaster.JobId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }

                ViewData["StateId"] = new SelectList(_context.StateMaster, "StateId", "Name");
                ViewData["CityId"] = new SelectList(_context.CityMaster, "CityId", "Name");
                ViewData["DistrictId"] = new SelectList(_context.DistrictMaster, "DistrictId", "Name");
                ViewData["CreatedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", jobMaster.CreatedBy);
                ViewData["ModifiedBy"] = new SelectList(_context.AspNetUsers, "Id", "Id", jobMaster.ModifiedBy);
                //ViewData["UserProfileId"] = new SelectList(_context.UserProfile, "UserProfileId", "Email", jobMaster.UserProfileId);
                return View(jobMaster);
            }

            // GET: JobMasters/Delete/5
            public async Task<IActionResult> Delete(Guid? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var jobMaster = await _context.JobMaster
                    .Include(j => j.State)
                    .Include(j => j.District)
                    .Include(j => j.City)

                    .Include(j => j.CreatedByNavigation)
                    .Include(j => j.ModifiedByNavigation)
                    .Include(j => j.UserProfile)
                    .FirstOrDefaultAsync(m => m.JobId == id);
                if (jobMaster == null)
                {
                    return NotFound();
                }

                return View(jobMaster);
            }

            // POST: JobMasters/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(Guid id)
            {
                var jobMaster = await _context.JobMaster.FindAsync(id);
                _context.JobMaster.Remove(jobMaster);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool JobMasterExists(Guid id)
            {
                return _context.JobMaster.Any(e => e.JobId == id);
            }



        [HttpGet("AppliedCandidateList")]
        public IActionResult AppliedCandidateList(Guid Id)
        {

            //var userId = this._userManager.GetUserId(this.User);
            //var userProfile = this._testService.GetUserProfile(userId);
            //ViewBag.applyinfo = _userProfileService.GetAppliedJobs(userProfile.UserProfileId);
            ViewBag.apply = _userProfileService.GetJobInfo();
            ViewBag.AppliedCandidatelist = _userProfileService.GetAppliedCandidate(Id);
            ViewBag.JobId = _context.AppliedJobs.Where(e => e.JobId == Id).Select(e => e.JobId).FirstOrDefault();

            ViewBag.ApprovedJobList = _context.ApprovalJobs.Where(m => m.JobId == Id).ToList();
            return View();
        }

        [HttpPost("AppliedCandidateList")]
        public IActionResult AppliedCandidateList(Guid JobId, string Sortby)
        {
            var appliedCandidatelist = _userProfileService.GetAppliedCandidate(JobId);
            var approvedJobList = _context.ApprovalJobs.Where(a => a.JobId == JobId).ToList();
            ViewBag.JobId = _context.AppliedJobs.Where(e => e.JobId == JobId).Select(e => e.JobId).FirstOrDefault();

            if (Sortby == "newest")
            {
                ViewBag.AppliedCandidatelist = appliedCandidatelist.OrderByDescending(m => m.CreatedDate).ToList();
                ViewBag.ApprovedJobList = approvedJobList.OrderByDescending(a => a.CreatedDate).ToList();
            }
            if (Sortby == "oldest")
            {
                ViewBag.AppliedCandidatelist = appliedCandidatelist.OrderBy(m => m.CreatedDate).ToList();
                ViewBag.ApprovedJobList = approvedJobList.OrderBy(a => a.CreatedDate).ToList();
            }
            if (Sortby == "random")
            {
                ViewBag.AppliedCandidatelist = appliedCandidatelist;
                ViewBag.ApprovedJobList = approvedJobList;
            }

            return View();
        }


        [HttpPost]
        public IActionResult Send_JobApprovalMail(Guid userProfileId, Guid jobId)
        {
            //CandidateId
            UserProfile userProfile = _context.UserProfile.Where(e => e.UserProfileId == userProfileId).FirstOrDefault();

            var jobInfo = _context.AppliedJobs.Where(m => m.UserProfileId == userProfileId && m.JobId == jobId).FirstOrDefault();

            ApprovalJobs approvalJobInfo = _context.ApprovalJobs.Where(m => m.UserProfileId == userProfileId &&
                                                m.JobId == jobId).FirstOrDefault();

            ViewBag.approvalJobInfo = approvalJobInfo;

            if (approvalJobInfo == null)
            {
                ApprovalJobs approvaljobs = new ApprovalJobs()
                {
                    ApprovalJobId = Guid.NewGuid(),
                    UserProfileId = userProfileId,
                    JobId = jobInfo.JobId,
                    ApprovalDate = DateTime.Now,
                    IsActive = "True",
                    Status = "Approved",
                    CreatedBy = this._userManager.GetUserId(this.User),
                    CreatedDate = DateTime.Now,
                    ModifiedBy = User.Claims.Select(x => x.Value).First(),
                    ModifiedDate = DateTime.Now

                };
                _context.ApprovalJobs.Add(approvaljobs);
                _context.SaveChanges();
            }
            else
            {
                NotFound();
            }

            MailMessage mailMessage = new MailMessage(_configuration["SMTPConfig:SenderAddress"].ToString(), userProfile.Email);
            mailMessage.Subject = "Congratulations! You have successfully approved for job";
            mailMessage.Body =
            "Dear " + userProfile.FirstName + " " + userProfile.MiddleName + " " + userProfile.LastName + ",<br><br>" +
            "Bravo ! ... ";

            mailMessage.IsBodyHtml = Convert.ToBoolean(_configuration["SMTPConfig:IsBodyHTML"]);

            SmtpClient smtpClient = new SmtpClient(_configuration["SMTPConfig:host"].ToString(), int.Parse(_configuration["SMTPConfig:Port"]));
            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = _configuration["SMTPConfig:UserName"].ToString(),
                Password = _configuration["SMTPConfig:Password"].ToString()
            };
            smtpClient.EnableSsl = Convert.ToBoolean(_configuration["SMTPConfig:EnableSSL"]);
            smtpClient.Send(mailMessage);

            return Ok(new ResponseResult()
            {
                Error = false,
                Message = "You have successfully send mail.",
            });

        }



        //[HttpPost("AppliedCandidateList")]
        //public async Task<IActionResult> AppliedCandidateList()
        //{
        //    return View();
        //}


        [HttpGet]
        public IActionResult AppliedCandidatesOperation(Guid jobId, int sortBy)
        {
            var result = _userProfileService.GetAppliedJobsInfo(jobId, sortBy);
            return new JsonResult(result);
        }

    }
}

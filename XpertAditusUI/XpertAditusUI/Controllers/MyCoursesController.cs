using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using XpertAditusUI.Data;
using XpertAditusUI.Models;
using XpertAditusUI.Service;
using Microsoft.Extensions.Configuration;
namespace XpertAditusUI.Controllers
{
    [Route("[controller]")]
    public class MyCoursesController : CandidateController
    {
        private readonly XpertAditusDbContext _db;
        private IConfiguration _configuration;
        public MyCoursesController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
            XpertAditusDbContext db, IConfiguration configurationManager,
            CandidateService candidateService) :
            base(logger, userManager, candidateService)
        {
            _db = db;
            _configuration = configurationManager;


        }
        [HttpGet("Index")]
        public ActionResult Index()
        {
            this.InitializeViewBag();
            return View(this._candidateService.GetAllUserCoursesWithTrainingStatus(ViewBag.UserProfile));
            
        }
    }
}

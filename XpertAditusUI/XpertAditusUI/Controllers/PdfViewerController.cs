using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using XpertAditusUI.Models;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using Microsoft.Extensions.Configuration;
using XpertAditusUI.Service;
using XpertAditusUI.DTO;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;
using Spire.Doc;
using System.Threading;

namespace XpertAditusUI.Controllers
{
    [Route("[controller]")]
    public class PdfViewerController : Controller
    {
        public PdfViewerController()
        {

        }

        [HttpGet("PdfViewer")]
        public IActionResult PdfViewer([FromQuery(Name = "FilePath")] string FilePath)
        {
            ViewBag.PdfFilePath = FilePath;
            return View();
        }

    }
}

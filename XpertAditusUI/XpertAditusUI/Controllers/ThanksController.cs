using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using XpertAditusUI.DTO;
using XpertAditusUI.Models;

namespace XpertAditusUI.Controllers
{
    public class ThanksController : Controller
    {
        // GET: Thanks
        public ActionResult Thanks()
        {
            PaymentDTO payment = new PaymentDTO();
       
            payment.responseId = HttpContext.Request.Query["pid"];
            payment.candidateId = HttpContext.Request.Query["cid"];
            payment.prefill_name = "Nikhil";
            payment.amount = "250";
            payment.userName = HttpContext.Request.Query["unm"];
        

            return View(payment);
        }

        public ActionResult DownloadPymntRcLetter(string pid)
        {
          
            return null;
        }

    }
}

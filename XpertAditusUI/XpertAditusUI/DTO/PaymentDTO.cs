using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XpertAditusUI.DTO
{
    public class PaymentDTO
    {
        //TODO Change properties name in PascalCase
        public string key { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string prefill_name { get; set; }
        public string prefill_email { get; set; }
        public string prefill_contact { get; set; }
        public string notes_add { get; set; }
        public string theme_color { get; set; }
        public string responseId { get; set; }
        public string candidateId { get; set; }
        public string userName { get; set; }

    }
}
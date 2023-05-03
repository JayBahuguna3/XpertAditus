using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XpertAditusUI.Models
{
    public class DeleteInterviewModel
    {
        public string QuestionText { get; set; }

        public string VideoAbsolutePath { get; set; }

        public Guid QuestionnaireId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XpertAditusUI.Models
{
    public class CandidateDetail
    {
        public Guid UserProfileID { get; set; }
        public string CandidateName { get; set; }
        public string Qualification { get; set; }
        public string TotalExperience { get; set; }
        public string Designation { get; set; }
        public string FacebookLink { get; set; }
        public string LinkedinLink { get; set; }
        public string TwitterLink { get; set; }
        public string ResumePath { get; set; }
        public string PhotoPath { get; set; }
        public List<InterviewResult> InterviewResults { get; set; }
        public List<Qualification> Qualifications { get; set; }
        public List<Experience> Experiences { get; set; }
    }
}

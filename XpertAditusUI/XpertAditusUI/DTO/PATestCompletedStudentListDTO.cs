using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XpertAditusUI.DTO
{
    public class PATestCompletedStudentListDTO
    {
        public Guid UserProfileId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string CourseName { get; set; }
        public string TestCourseName { get; set; }
        public string Status { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public decimal Score { get; set; }
        public Guid PACandidateResultId { get; set; }

    }
}

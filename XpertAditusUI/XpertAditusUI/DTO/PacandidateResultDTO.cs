using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XpertAditusUI.DTO
{
    public partial class PacandidateResultDTO
    {
        public Guid PacandidateResultId { get; set; }
        public Guid? UserCoursesId { get; set; }
        public int? TestDuration { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? IsCleared { get; set; }
        public DateTime? TestStarted { get; set; }
        public DateTime? RemainingTime { get; set; }
        public DateTime? TestEnd { get; set; }
        public Guid? PamonthlyTestId { get; set; }
        public int? TestAttempt { get; set; }
        public bool? IsActive { get; set; }
        public decimal? Score { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}

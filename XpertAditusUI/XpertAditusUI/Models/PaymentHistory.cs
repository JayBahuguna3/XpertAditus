﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace XpertAditusUI.Models
{
    public partial class PaymentHistory
    {
        public PaymentHistory()
        {
            CandidateResult = new HashSet<CandidateResult>();
        }

        [Key]
        public Guid PaymentHistoryId { get; set; }
        [Column("UserCoursesID")]
        public Guid? UserCoursesId { get; set; }
        [StringLength(100)]
        public string TransactionId { get; set; }
        public int? TransactionNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? TransactionDate { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Amount { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        [StringLength(450)]
        public string CreatedBy { get; set; }
        public int OrderId { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        [InverseProperty(nameof(AspNetUsers.PaymentHistory))]
        public virtual AspNetUsers CreatedByNavigation { get; set; }
        [ForeignKey(nameof(UserCoursesId))]
        [InverseProperty("PaymentHistory")]
        public virtual UserCourses UserCourses { get; set; }
        [InverseProperty("PaymentHistory")]
        public virtual ICollection<CandidateResult> CandidateResult { get; set; }
    }
}
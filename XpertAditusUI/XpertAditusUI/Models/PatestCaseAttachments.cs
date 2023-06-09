﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace XpertAditusUI.Models
{
    [Table("PATestCaseAttachments")]
    public partial class PatestCaseAttachments
    {
        [Key]
        public Guid TestCaseAttachmentId { get; set; }
        public Guid? PaCandidateResultId { get; set; }
        [StringLength(500)]
        public string AttachmentPath { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        [StringLength(450)]
        public string CreatedBy { get; set; }
        public string Feedback { get; set; }
        public int? Score { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        [InverseProperty(nameof(AspNetUsers.PatestCaseAttachments))]
        public virtual AspNetUsers CreatedByNavigation { get; set; }
        [ForeignKey(nameof(PaCandidateResultId))]
        [InverseProperty(nameof(PacandidateResult.PatestCaseAttachments))]
        public virtual PacandidateResult PaCandidateResult { get; set; }
    }
}
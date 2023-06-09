﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace XpertAditusUI.Models
{
    public partial class InterviewResult
    {
        [Key]
        public Guid InterviewResultId { get; set; }
        [Column("UserProfileID")]
        public Guid? UserProfileId { get; set; }
        [StringLength(300)]
        public string VideoAbsolutePath { get; set; }
        [StringLength(300)]
        public string VideoName { get; set; }
        public Guid? QuestionnaireId { get; set; }
        public int? QuestionOrder { get; set; }
        [StringLength(450)]
        public string ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        [StringLength(450)]
        public string CreatedBy { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        [InverseProperty(nameof(AspNetUsers.InterviewResultCreatedByNavigation))]
        public virtual AspNetUsers CreatedByNavigation { get; set; }
        [ForeignKey(nameof(ModifiedBy))]
        [InverseProperty(nameof(AspNetUsers.InterviewResultModifiedByNavigation))]
        public virtual AspNetUsers ModifiedByNavigation { get; set; }
        [ForeignKey(nameof(QuestionnaireId))]
        [InverseProperty("InterviewResult")]
        public virtual Questionnaire Questionnaire { get; set; }
        [ForeignKey(nameof(UserProfileId))]
        [InverseProperty("InterviewResult")]
        public virtual UserProfile UserProfile { get; set; }
    }
}
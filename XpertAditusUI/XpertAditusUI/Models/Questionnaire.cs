﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace XpertAditusUI.Models
{
    public partial class Questionnaire
    {
        public Questionnaire()
        {
            InterviewResult = new HashSet<InterviewResult>();
            QuestionnaireResult = new HashSet<QuestionnaireResult>();
        }

        [Key]
        public Guid QuestionnaireId { get; set; }
        [Required]
        [StringLength(100)]
        public string QuestionnaireType { get; set; }
        public string QuestionText { get; set; }
        [StringLength(100)]
        public string Option1 { get; set; }
        [StringLength(100)]
        public string Option2 { get; set; }
        [StringLength(100)]
        public string Option3 { get; set; }
        [StringLength(100)]
        public string Option4 { get; set; }
        [StringLength(100)]
        public string CorrectAnswer { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? TrainingContentId { get; set; }
        [StringLength(10)]
        public string IsActive { get; set; }
        [StringLength(450)]
        public string ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        [StringLength(450)]
        public string CreatedBy { get; set; }

        [ForeignKey(nameof(CourseId))]
        [InverseProperty("Questionnaire")]
        public virtual Course Course { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        [InverseProperty(nameof(AspNetUsers.QuestionnaireCreatedByNavigation))]
        public virtual AspNetUsers CreatedByNavigation { get; set; }
        [ForeignKey(nameof(ModifiedBy))]
        [InverseProperty(nameof(AspNetUsers.QuestionnaireModifiedByNavigation))]
        public virtual AspNetUsers ModifiedByNavigation { get; set; }
        [ForeignKey(nameof(TrainingContentId))]
        [InverseProperty(nameof(TrainingContentsMaster.Questionnaire))]
        public virtual TrainingContentsMaster TrainingContent { get; set; }
        [InverseProperty("Questionnaire")]
        public virtual ICollection<InterviewResult> InterviewResult { get; set; }
        [InverseProperty("Questionnaire")]
        public virtual ICollection<QuestionnaireResult> QuestionnaireResult { get; set; }
    }
}
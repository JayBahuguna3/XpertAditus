﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace XpertAditusUI.Models
{
    [Table("PAQuestionnaire")]
    public partial class Paquestionnaire
    {
        public Paquestionnaire()
        {
            PaquestionnaireResult = new HashSet<PaquestionnaireResult>();
            PavideoQuestionResult = new HashSet<PavideoQuestionResult>();
        }

        [Key]
        [Column("PAQuestionnaireId")]
        public Guid PaquestionnaireId { get; set; }
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
        [Column("PATrainingContentId")]
        public Guid? PatrainingContentId { get; set; }
        public bool? IsActive { get; set; }
        [StringLength(450)]
        public string ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        [StringLength(450)]
        public string CreatedBy { get; set; }

        [ForeignKey(nameof(CourseId))]
        [InverseProperty(nameof(CourseMaster.Paquestionnaire))]
        public virtual CourseMaster Course { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        [InverseProperty(nameof(AspNetUsers.PaquestionnaireCreatedByNavigation))]
        public virtual AspNetUsers CreatedByNavigation { get; set; }
        [ForeignKey(nameof(ModifiedBy))]
        [InverseProperty(nameof(AspNetUsers.PaquestionnaireModifiedByNavigation))]
        public virtual AspNetUsers ModifiedByNavigation { get; set; }
        [ForeignKey(nameof(PatrainingContentId))]
        [InverseProperty(nameof(PatrainingContentMaster.Paquestionnaire))]
        public virtual PatrainingContentMaster PatrainingContent { get; set; }
        [InverseProperty("Paquestionnaire")]
        public virtual ICollection<PaquestionnaireResult> PaquestionnaireResult { get; set; }
        [InverseProperty("Paquestionnaire")]
        public virtual ICollection<PavideoQuestionResult> PavideoQuestionResult { get; set; }
    }
}
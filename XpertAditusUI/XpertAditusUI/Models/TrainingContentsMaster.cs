﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace XpertAditusUI.Models
{
    public partial class TrainingContentsMaster
    {
        public TrainingContentsMaster()
        {
            CandidateTrainingStatus = new HashSet<CandidateTrainingStatus>();
            Questionnaire = new HashSet<Questionnaire>();
            TestScenario = new HashSet<TestScenario>();
        }

        [Key]
        public Guid TrainingContentId { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Path { get; set; }
        [StringLength(20)]
        public string ContentType { get; set; }
        public Guid? CourseId { get; set; }
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
        [InverseProperty("TrainingContentsMaster")]
        public virtual Course Course { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        [InverseProperty(nameof(AspNetUsers.TrainingContentsMasterCreatedByNavigation))]
        public virtual AspNetUsers CreatedByNavigation { get; set; }
        [ForeignKey(nameof(ModifiedBy))]
        [InverseProperty(nameof(AspNetUsers.TrainingContentsMasterModifiedByNavigation))]
        public virtual AspNetUsers ModifiedByNavigation { get; set; }
        [InverseProperty("TrainingContent")]
        public virtual ICollection<CandidateTrainingStatus> CandidateTrainingStatus { get; set; }
        [InverseProperty("TrainingContent")]
        public virtual ICollection<Questionnaire> Questionnaire { get; set; }
        [InverseProperty("TrainingContents")]
        public virtual ICollection<TestScenario> TestScenario { get; set; }
    }
}
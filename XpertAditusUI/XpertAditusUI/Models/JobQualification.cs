﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace XpertAditusUI.Models
{
    public partial class JobQualification
    {
        [Key]
        public Guid JobQualificationId { get; set; }
        public Guid? JobId { get; set; }
        public Guid? EducationId { get; set; }
        [StringLength(450)]
        public string ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        [StringLength(450)]
        public string CreatedBy { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        [InverseProperty(nameof(AspNetUsers.JobQualificationCreatedByNavigation))]
        public virtual AspNetUsers CreatedByNavigation { get; set; }
        [ForeignKey(nameof(EducationId))]
        [InverseProperty(nameof(EducationMaster.JobQualification))]
        public virtual EducationMaster Education { get; set; }
        [ForeignKey(nameof(JobId))]
        [InverseProperty(nameof(JobMaster.JobQualification))]
        public virtual JobMaster Job { get; set; }
        [ForeignKey(nameof(ModifiedBy))]
        [InverseProperty(nameof(AspNetUsers.JobQualificationModifiedByNavigation))]
        public virtual AspNetUsers ModifiedByNavigation { get; set; }
    }
}
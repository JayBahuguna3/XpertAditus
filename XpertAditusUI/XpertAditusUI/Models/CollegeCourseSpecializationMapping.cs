﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

#nullable disable

namespace XpertAditusUI.Models
{
    public partial class CollegeCourseSpecializationMapping
    {
        [Key]
        public Guid CollegeCourseSpecializationId { get; set; }
        public Guid? CollegeProfileId { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? SpecialzationId { get; set; }
        public Guid? EducationId { get; set; }
        public bool? IsActive { get; set; }
        public bool HigherSecondary { get; set; }
        public bool Graduate { get; set; }
        [Column("Min_Qualification")]
        [StringLength(100)]
        public string MinQualification { get; set; }
        [Column("Min_TestScore", TypeName = "decimal(18, 2)")]
        public decimal? MinTestScore { get; set; }
        [Column("Min_AcademicPercentage")]
        public int? MinAcademicPercentage { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }
        [StringLength(450)]
        public string CreatedBy { get; set; }
        [StringLength(450)]
        public string UpdatedBy { get; set; }

        [ForeignKey(nameof(CollegeProfileId))]
        [InverseProperty("CollegeCourseSpecializationMapping")]
        public virtual CollegeProfile CollegeProfile { get; set; }
        [ForeignKey(nameof(CourseId))]
        [InverseProperty(nameof(CourseMaster.CollegeCourseSpecializationMapping))]
        public virtual CourseMaster Course { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        [InverseProperty(nameof(AspNetUsers.CollegeCourseSpecializationMappingCreatedByNavigation))]
        public virtual AspNetUsers CreatedByNavigation { get; set; }
        [ForeignKey(nameof(EducationId))]
        [InverseProperty(nameof(EducationMaster.CollegeCourseSpecializationMapping))]
        public virtual EducationMaster Education { get; set; }
        [ForeignKey(nameof(SpecialzationId))]
        [InverseProperty(nameof(SpecializationMaster.CollegeCourseSpecializationMapping))]
        public virtual SpecializationMaster Specialzation { get; set; }
        [ForeignKey(nameof(UpdatedBy))]
        [InverseProperty(nameof(AspNetUsers.CollegeCourseSpecializationMappingUpdatedByNavigation))]
        public virtual AspNetUsers UpdatedByNavigation { get; set; }
    }
}
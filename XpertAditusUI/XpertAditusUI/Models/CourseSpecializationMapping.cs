﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace XpertAditusUI.Models
{
    public partial class CourseSpecializationMapping
    {
        [Key]
        public Guid CourseSpecializationId { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? SpecializationId { get; set; }
        public bool? IsActive { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedDate { get; set; }
        [StringLength(450)]
        public string CreatedBy { get; set; }
        [StringLength(450)]
        public string UpdatedBy { get; set; }

        [ForeignKey(nameof(CourseId))]
        [InverseProperty(nameof(CourseMaster.CourseSpecializationMapping))]
        public virtual CourseMaster Course { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        [InverseProperty(nameof(AspNetUsers.CourseSpecializationMappingCreatedByNavigation))]
        public virtual AspNetUsers CreatedByNavigation { get; set; }
        [ForeignKey(nameof(SpecializationId))]
        [InverseProperty(nameof(SpecializationMaster.CourseSpecializationMapping))]
        public virtual SpecializationMaster Specialization { get; set; }
        [ForeignKey(nameof(UpdatedBy))]
        [InverseProperty(nameof(AspNetUsers.CourseSpecializationMappingUpdatedByNavigation))]
        public virtual AspNetUsers UpdatedByNavigation { get; set; }
    }
}
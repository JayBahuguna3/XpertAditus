﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace XpertAditusUI.Models
{
    public partial class JobMaster
    {
        public JobMaster()
        {
            AppliedJobs = new HashSet<AppliedJobs>();
            ApprovalJobs = new HashSet<ApprovalJobs>();
            JobQualification = new HashSet<JobQualification>();
            ShortlistedCandidates = new HashSet<ShortlistedCandidates>();
        }

        [Key]
        public Guid JobId { get; set; }
        public Guid? UserProfileId { get; set; }
        [Required]
        [StringLength(150)]
        public string JobDesignation { get; set; }
        [Required]
        [StringLength(150)]
        public string CompanyName { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? StateId { get; set; }
        public Guid? DistrictId { get; set; }
        [Column("CityID")]
        public Guid? CityId { get; set; }
        public long MinExperience { get; set; }
        public long MaxExperience { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "date")]
        public DateTime? LastDate { get; set; }
        [Column("CTC")]
        [StringLength(100)]
        public string Ctc { get; set; }
        [Column(TypeName = "date")]
        public DateTime? ExpectedJoiningDate { get; set; }
        public long? NoOfOpening { get; set; }
        [StringLength(100)]
        public string Category { get; set; }
        [StringLength(100)]
        public string Industry { get; set; }
        [StringLength(100)]
        public string WorkShift { get; set; }
        [StringLength(100)]
        public string JobLevel { get; set; }
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

        [ForeignKey(nameof(CityId))]
        [InverseProperty(nameof(CityMaster.JobMaster))]
        public virtual CityMaster City { get; set; }
        [ForeignKey(nameof(CountryId))]
        [InverseProperty(nameof(CountryMaster.JobMaster))]
        public virtual CountryMaster Country { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        [InverseProperty(nameof(AspNetUsers.JobMasterCreatedByNavigation))]
        public virtual AspNetUsers CreatedByNavigation { get; set; }
        [ForeignKey(nameof(DistrictId))]
        [InverseProperty(nameof(DistrictMaster.JobMaster))]
        public virtual DistrictMaster District { get; set; }
        [ForeignKey(nameof(ModifiedBy))]
        [InverseProperty(nameof(AspNetUsers.JobMasterModifiedByNavigation))]
        public virtual AspNetUsers ModifiedByNavigation { get; set; }
        [ForeignKey(nameof(StateId))]
        [InverseProperty(nameof(StateMaster.JobMaster))]
        public virtual StateMaster State { get; set; }
        [ForeignKey(nameof(UserProfileId))]
        [InverseProperty("JobMaster")]
        public virtual UserProfile UserProfile { get; set; }
        [InverseProperty("Job")]
        public virtual ICollection<AppliedJobs> AppliedJobs { get; set; }
        [InverseProperty("Job")]
        public virtual ICollection<ApprovalJobs> ApprovalJobs { get; set; }
        [InverseProperty("Job")]
        public virtual ICollection<JobQualification> JobQualification { get; set; }
        [InverseProperty("Job")]
        public virtual ICollection<ShortlistedCandidates> ShortlistedCandidates { get; set; }
    }
}
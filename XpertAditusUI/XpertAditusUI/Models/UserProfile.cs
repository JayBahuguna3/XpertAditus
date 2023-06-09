﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace XpertAditusUI.Models
{
    public partial class UserProfile
    {
        public UserProfile()
        {
            Address = new HashSet<Address>();
            Admission = new HashSet<Admission>();
            AppliedJobs = new HashSet<AppliedJobs>();
            ApprovalJobs = new HashSet<ApprovalJobs>();
            CandidateCollegeMapping = new HashSet<CandidateCollegeMapping>();
            DisclaimerAccetped = new HashSet<DisclaimerAccetped>();
            Experience = new HashSet<Experience>();
            InterviewResult = new HashSet<InterviewResult>();
            JobMaster = new HashSet<JobMaster>();
            PacandidateResult = new HashSet<PacandidateResult>();
            PadisclaimerAccetped = new HashSet<PadisclaimerAccetped>();
            PavideoQuestionResult = new HashSet<PavideoQuestionResult>();
            PpoInfo = new HashSet<PpoInfo>();
            Qualification = new HashSet<Qualification>();
            SentNotificationsCandidate = new HashSet<SentNotifications>();
            SentNotificationsEmployer = new HashSet<SentNotifications>();
            ShortlistedCandidatesCandidate = new HashSet<ShortlistedCandidates>();
            ShortlistedCandidatesEmployer = new HashSet<ShortlistedCandidates>();
            UserCourses = new HashSet<UserCourses>();
        }

        [Key]
        public Guid UserProfileId { get; set; }
        [Required]
        [StringLength(450)]
        public string LoginId { get; set; }
        public long RegistrationNumber { get; set; }
        [Required]
        [StringLength(100)]
        public string UserProfileType { get; set; }
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        [StringLength(100)]
        public string MiddleName { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        [StringLength(100)]
        public string Email { get; set; }
        public long MobileNumber { get; set; }
        [Column("DOB", TypeName = "date")]
        public DateTime Dob { get; set; }
        public long? OfficeContactNumber { get; set; }
        public long? OfficeFaxNumber { get; set; }
        [StringLength(100)]
        public string BusinessType { get; set; }
        [StringLength(100)]
        public string CompanyName { get; set; }
        public string ResumePath { get; set; }
        public string PhotoPath { get; set; }
        [StringLength(100)]
        public string FacebookLink { get; set; }
        [StringLength(100)]
        public string LinkedinLink { get; set; }
        [StringLength(100)]
        public string TwitterLink { get; set; }
        [StringLength(100)]
        public string MaritalStatus { get; set; }
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

        [ForeignKey(nameof(CreatedBy))]
        [InverseProperty(nameof(AspNetUsers.UserProfileCreatedByNavigation))]
        public virtual AspNetUsers CreatedByNavigation { get; set; }
        [ForeignKey(nameof(LoginId))]
        [InverseProperty(nameof(AspNetUsers.UserProfileLogin))]
        public virtual AspNetUsers Login { get; set; }
        [ForeignKey(nameof(ModifiedBy))]
        [InverseProperty(nameof(AspNetUsers.UserProfileModifiedByNavigation))]
        public virtual AspNetUsers ModifiedByNavigation { get; set; }
        [InverseProperty("UserProfile")]
        public virtual ICollection<Address> Address { get; set; }
        [InverseProperty("UserProfile")]
        public virtual ICollection<Admission> Admission { get; set; }
        [InverseProperty("UserProfile")]
        public virtual ICollection<AppliedJobs> AppliedJobs { get; set; }
        [InverseProperty("UserProfile")]
        public virtual ICollection<ApprovalJobs> ApprovalJobs { get; set; }
        [InverseProperty("UserProfile")]
        public virtual ICollection<CandidateCollegeMapping> CandidateCollegeMapping { get; set; }
        [InverseProperty("UserProfile")]
        public virtual ICollection<DisclaimerAccetped> DisclaimerAccetped { get; set; }
        [InverseProperty("UserProfile")]
        public virtual ICollection<Experience> Experience { get; set; }
        [InverseProperty("UserProfile")]
        public virtual ICollection<InterviewResult> InterviewResult { get; set; }
        [InverseProperty("UserProfile")]
        public virtual ICollection<JobMaster> JobMaster { get; set; }
        [InverseProperty("UserProfile")]
        public virtual ICollection<PacandidateResult> PacandidateResult { get; set; }
        [InverseProperty("UserProfile")]
        public virtual ICollection<PadisclaimerAccetped> PadisclaimerAccetped { get; set; }
        [InverseProperty("UserProfile")]
        public virtual ICollection<PavideoQuestionResult> PavideoQuestionResult { get; set; }
        [InverseProperty("UserProfile")]
        public virtual ICollection<PpoInfo> PpoInfo { get; set; }
        [InverseProperty("UserProfile")]
        public virtual ICollection<Qualification> Qualification { get; set; }
        [InverseProperty(nameof(SentNotifications.Candidate))]
        public virtual ICollection<SentNotifications> SentNotificationsCandidate { get; set; }
        [InverseProperty(nameof(SentNotifications.Employer))]
        public virtual ICollection<SentNotifications> SentNotificationsEmployer { get; set; }
        [InverseProperty(nameof(ShortlistedCandidates.Candidate))]
        public virtual ICollection<ShortlistedCandidates> ShortlistedCandidatesCandidate { get; set; }
        [InverseProperty(nameof(ShortlistedCandidates.Employer))]
        public virtual ICollection<ShortlistedCandidates> ShortlistedCandidatesEmployer { get; set; }
        [InverseProperty("UserProfile")]
        public virtual ICollection<UserCourses> UserCourses { get; set; }
    }
}
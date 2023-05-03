﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace XpertAditusUI.Models
{
    [Table("PADisclaimerAccetped")]
    public partial class PadisclaimerAccetped
    {
        [Key]
        [Column("PADisclaimerAcceptedID")]
        public Guid PadisclaimerAcceptedId { get; set; }
        public Guid? UserProfileId { get; set; }
        [Column("PADisclaimerId")]
        public Guid? PadisclaimerId { get; set; }
        public bool? IsAccepted { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        [StringLength(450)]
        public string CreatedBy { get; set; }
        [Column("MonthlyTestID")]
        public Guid? MonthlyTestId { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        [InverseProperty(nameof(AspNetUsers.PadisclaimerAccetped))]
        public virtual AspNetUsers CreatedByNavigation { get; set; }
        [ForeignKey(nameof(MonthlyTestId))]
        [InverseProperty(nameof(PamonthlyTest.PadisclaimerAccetped))]
        public virtual PamonthlyTest MonthlyTest { get; set; }
        [ForeignKey(nameof(PadisclaimerId))]
        [InverseProperty("PadisclaimerAccetped")]
        public virtual Padisclaimer Padisclaimer { get; set; }
        [ForeignKey(nameof(UserProfileId))]
        [InverseProperty("PadisclaimerAccetped")]
        public virtual UserProfile UserProfile { get; set; }
    }
}
﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace XpertAditusUI.Models
{
    public partial class Disclaimer
    {
        public Disclaimer()
        {
            DisclaimerAccetped = new HashSet<DisclaimerAccetped>();
        }

        [Key]
        public Guid DisclaimerId { get; set; }
        [Required]
        public string Instruction { get; set; }
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
        [InverseProperty("Disclaimer")]
        public virtual Course Course { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        [InverseProperty(nameof(AspNetUsers.DisclaimerCreatedByNavigation))]
        public virtual AspNetUsers CreatedByNavigation { get; set; }
        [ForeignKey(nameof(ModifiedBy))]
        [InverseProperty(nameof(AspNetUsers.DisclaimerModifiedByNavigation))]
        public virtual AspNetUsers ModifiedByNavigation { get; set; }
        [InverseProperty("Disclaimer")]
        public virtual ICollection<DisclaimerAccetped> DisclaimerAccetped { get; set; }
    }
}
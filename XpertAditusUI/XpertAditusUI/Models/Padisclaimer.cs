﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace XpertAditusUI.Models
{
    [Table("PADisclaimer")]
    public partial class Padisclaimer
    {
        public Padisclaimer()
        {
            PadisclaimerAccetped = new HashSet<PadisclaimerAccetped>();
        }

        [Key]
        [Column("PADisclaimerId")]
        public Guid PadisclaimerId { get; set; }
        [Required]
        public string Instruction { get; set; }
        public Guid? CourseId { get; set; }
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
        [InverseProperty(nameof(CourseMaster.Padisclaimer))]
        public virtual CourseMaster Course { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        [InverseProperty(nameof(AspNetUsers.PadisclaimerCreatedByNavigation))]
        public virtual AspNetUsers CreatedByNavigation { get; set; }
        [ForeignKey(nameof(ModifiedBy))]
        [InverseProperty(nameof(AspNetUsers.PadisclaimerModifiedByNavigation))]
        public virtual AspNetUsers ModifiedByNavigation { get; set; }
        [InverseProperty("Padisclaimer")]
        public virtual ICollection<PadisclaimerAccetped> PadisclaimerAccetped { get; set; }
    }
}
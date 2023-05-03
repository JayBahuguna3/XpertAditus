﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace XpertAditusUI.Models
{
    public partial class CityMaster
    {
        public CityMaster()
        {
            Address = new HashSet<Address>();
            JobMaster = new HashSet<JobMaster>();
        }

        [Key]
        public Guid CityId { get; set; }
        [Required]
        public string Name { get; set; }
        public Guid? DistrictId { get; set; }
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
        [InverseProperty(nameof(AspNetUsers.CityMasterCreatedByNavigation))]
        public virtual AspNetUsers CreatedByNavigation { get; set; }
        [ForeignKey(nameof(DistrictId))]
        [InverseProperty(nameof(DistrictMaster.CityMaster))]
        public virtual DistrictMaster District { get; set; }
        [ForeignKey(nameof(ModifiedBy))]
        [InverseProperty(nameof(AspNetUsers.CityMasterModifiedByNavigation))]
        public virtual AspNetUsers ModifiedByNavigation { get; set; }
        [InverseProperty("City")]
        public virtual ICollection<Address> Address { get; set; }
        [InverseProperty("City")]
        public virtual ICollection<JobMaster> JobMaster { get; set; }
    }
}
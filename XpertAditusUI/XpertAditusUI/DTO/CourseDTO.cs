﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace XpertAditusUI.DTO
{
    public partial class CourseDTO
    {
        public Guid CourseId { get; set; }
        public string Name { get; set; }
        public decimal Fee { get; set; }
        public string Description { get; set; }
        public string IsActive { get; set; }
        public int? TestDuration { get; set; }
    }
}
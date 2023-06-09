﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace XpertAditusUI.DTO
{
    public partial class CandidateResultDTO
    {

        public Guid CandidateResultId { get; set; }
        public Guid? UserCoursesId { get; set; }
        public int? TestDuration { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? IsCleared { get; set; }
        public DateTime? TestStarted { get; set; }
        public DateTime? RemainingTime { get; set; }
        public DateTime? TestEnd { get; set; }
        public Guid? PaymentHistoryId { get; set; }
        public int? TestAttempt { get; set; }
        public bool? IsActive { get; set; }
        public decimal? Score { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
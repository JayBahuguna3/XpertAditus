using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XpertAditusUI.Models;

namespace XpertAditusUI.DTO
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CandidateResultDTO, CandidateResult>();
            CreateMap<CandidateResult, CandidateResultDTO>();
            CreateMap<CourseDTO, Course>();
            CreateMap<Course, CourseDTO>();
            CreateMap<JobMasterDTO, JobMaster>();
            CreateMap<JobMaster, JobMasterDTO>();
            CreateMap<PacandidateResultDTO, PacandidateResult>();
            CreateMap<PacandidateResult, PacandidateResultDTO>();
        }
    }
}

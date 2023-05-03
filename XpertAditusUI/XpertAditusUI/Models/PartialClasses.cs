using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XpertAditusUI.Models;

namespace XpertAditusUI.Models 
{

    [ModelMetadataType(typeof(CourseMetaData))]
    public partial class Course
    {
       
    }
    [ModelMetadataType(typeof(TrainingContentsMasterMetaData))]
    public partial class TrainingContentsMaster
    {

    }

    [ModelMetadataType(typeof(JobMasterMetaData))]
    public partial class JobMaster
    {
        [NotMapped]
        public string CityName { get; set; }

    }
    [ModelMetadataType(typeof(QuestionnaireMetaData))]
    public partial class Questionnaire
    {
        [NotMapped]
        public int? QuestionOrder { get; set; }
        [NotMapped]
        public string? AnswerProvided { get; set; }
        [NotMapped]
        public string TotalQuestion { get; set; }
        [NotMapped]
        public string AttemptedQuestion { get; set; }
    }

    [ModelMetadataType(typeof(PaquestionnaireMetaData))]
    public partial class Paquestionnaire
    {
        [NotMapped]
        public int? QuestionOrder { get; set; }
        [NotMapped]
        public string? AnswerProvided { get; set; }
        [NotMapped]
        public string TotalQuestion { get; set; }
        [NotMapped]
        public string AttemptedQuestion { get; set; }
    }

    public partial class PamonthlyTest
    {
        [NotMapped]
        public List <IFormFile> Attachments { get; set; }

        [NotMapped]
        public Guid CandidateResultId { get; set;}
    }
    public partial class UserProfile
    {
        [NotMapped]
        public IFormFile ResumeFile { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
    }

    [ModelMetadataType(typeof(CollegeProfileMetaData))]
    public partial class CollegeProfile
    {
        [NotMapped]
        public IFormFile Attachment { get; set; }
        [NotMapped]
        public IFormFile Logo { get; set; }
    }

    public partial class CompanyProfile
    {
        [NotMapped]
        public IFormFile Attachment { get; set; }
        [NotMapped]
        public IFormFile Logo { get; set; }
    }

    [ModelMetadataType(typeof(CollegeCourseSpecializationMappingMetadata))]
    public partial class CollegeCourseSpecializationMapping
    {

    }
    public partial class Qualification
    {

    }
}


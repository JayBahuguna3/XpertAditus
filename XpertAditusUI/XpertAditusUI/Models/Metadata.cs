using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace XpertAditusUI.Models
{
    public class Metadata
    {

    }

    public class CourseMetaData
    {

        public Guid CourseId { get; set; }

        [Required(ErrorMessage = "This Course Name field is required")]

        public string Name { get; set; }
        [Required(ErrorMessage = "This Course Fees field is required")]
        public decimal Fee { get; set; }
        [Required(ErrorMessage = "This Course Descrption field is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "This  field is required")]
        public string IsActive { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }
    }

    public class TrainingContentsMasterMetaData
    {

        public Guid TrainingContentId { get; set; }

        [Required(ErrorMessage = "Content Name is required")]
        public string Name { get; set; }

        public string Path { get; set; }
        [Required(ErrorMessage = " Select Content Type")]
        public string ContentType { get; set; }
        [Required(ErrorMessage = " Cousrse Name is required")]
        public Guid? CourseId { get; set; }
        [Required]
        public string IsActive { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }
    }

    public class PaquestionnaireMetaData
    {

    }

    public class QuestionnaireMetaData
    {

        public Guid QuestionnaireId { get; set; }
        [Required(ErrorMessage = " Select Questionnaire Type")]
        public string QuestionnaireType { get; set; }
        [Required(ErrorMessage = " Question is required")]
        public string QuestionText { get; set; }
        [Required(ErrorMessage = " Option1 is required")]
        public string Option1 { get; set; }
        [Required(ErrorMessage = " Option2 is required")]
        public string Option2 { get; set; }
        // [Required(ErrorMessage = " Option3 is required")]
        public string Option3 { get; set; }
        // [Required(ErrorMessage = " Option4 is required")]
        public string Option4 { get; set; }
        [Required(ErrorMessage = " Select Correct Answer ")]
        public string CorrectAnswer { get; set; }
        [Required(ErrorMessage = " Select Course")]
        public Guid? CourseId { get; set; }
        [Required(ErrorMessage = " Select Training Content ")]
        public Guid? TrainingContentId { get; set; }
        [StringLength(10)]
        [Required(ErrorMessage = " This Field is required")]
        public string IsActive { get; set; }
        [StringLength(450)]
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime? CreatedDate { get; set; }
        [StringLength(450)]
        public string CreatedBy { get; set; }
    }

    public class JobMasterMetaData
    {

        public Guid JobId { get; set; }
        [Required(ErrorMessage = " Select User")]
        public Guid? UserProfileId { get; set; }
        [Required(ErrorMessage = " Job Desidgnation is required")]
        public string JobDesignation { get; set; }
        [Required(ErrorMessage = " Company Name is required")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = " Select State ")]
        public Guid? StateId { get; set; }
        [Required(ErrorMessage = " Select District")]
        public Guid? DistrictId { get; set; }
        [Required(ErrorMessage = " Select City ")]
        public Guid? CityId { get; set; }
        [Required(ErrorMessage = " Min Experience is required")]

        public long MinExperience { get; set; }
        [Required(ErrorMessage = " Max Experience is required")]
        public long MaxExperience { get; set; }
        [Required(ErrorMessage = " Decription is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "LastDate required")]
        public DateTime? LastDate { get; set; }
        [Required(ErrorMessage = " Ctc is required")]
        public string Ctc { get; set; }
        [Required(ErrorMessage = " Expected joining date is required")]
        public DateTime? ExpectedJoiningDate { get; set; }
        [Required(ErrorMessage = " No of opening  is required")]
        public long? NoOfOpening { get; set; }
        [Required(ErrorMessage = " Category is required")]
        public string Category { get; set; }
        [Required(ErrorMessage = "Industry is required")]
        public string Industry { get; set; }
        [Required(ErrorMessage = " Work Shift is required")]
        public string WorkShift { get; set; }
        [Required(ErrorMessage = " JobLevel is required")]

        public string JobLevel { get; set; }
        [Required(ErrorMessage = " This field  is required")]

        public string IsActive { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

    }

    public class CollegeProfileMetaData
    {
        [Required(ErrorMessage = "This field is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public long? CollegeContact { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string CollegeEmail { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string CollegeWebsiteLink { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string CollegeAddress { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string CountryId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string StateId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string DistrictId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string CityId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string About { get; set; }
    }

    public partial class CollegeCourseSpecializationMappingMetadata
    {
        public Guid CollegeCourseSpecializationId { get; set; }
        public Guid? CollegeProfileId { get; set; }
        [Required(ErrorMessage = " Select Course")]
        public Guid? CourseId { get; set; }
        public Guid? SpecialzationId { get; set; }
        public Guid? EducationId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public bool? IsActive { get; set; }
        public bool HigherSecondary { get; set; }
        public bool Graduate { get; set; }
        public string MinQualification { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public decimal? MinTestScore { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public int? MinAcademicPercentage { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }
    }

    public class QalificationMetaData {

      
        public string Name { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string UniversityName { get; set; }
  
        public decimal TotalMarks { get; set; }
     
        public decimal MarksObtained { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public decimal Percentage { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public DateTime CompletionYear { get; set; }
        public long ChronologicalOrder { get; set; }
      
        public string ModifiedBy { get; set; }
   
        public DateTime? ModifiedDate { get; set; }
 
        public DateTime? CreatedDate { get; set; }
      
        public string CreatedBy { get; set; }

       
        public Guid? EducationId { get; set; }
        public bool? IsHighestQualification { get; set; }

      
        
    }
}



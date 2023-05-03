using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using XpertAditusUI.Data;
using XpertAditusUI.Models;

namespace XpertAditusUI.Service
{
    public class UserProfileService
    {
        private XpertAditusDbContext _XpertAditusDbContext;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<IdentityUser> _userManager;
        public UserProfileService(UserManager<IdentityUser> userManager, XpertAditusDbContext XpertAditusDbContext, IWebHostEnvironment env)
        {
            _XpertAditusDbContext = XpertAditusDbContext;
            _userManager = userManager;
            _env = env;
        }

        public async Task SaveUserProfileAsync(UserProfile UserProfile, string userId)
        {
            //dynamic userDetail = "";
            var userDetail = _XpertAditusDbContext.UserProfile.Where(u => u.LoginId == userId).FirstOrDefault();
            userDetail.FirstName = UserProfile.FirstName;
            userDetail.MiddleName = UserProfile.MiddleName;
            userDetail.LastName = UserProfile.LastName;
            userDetail.Dob = UserProfile.Dob;
            userDetail.FacebookLink = UserProfile.FacebookLink;
            userDetail.LinkedinLink = UserProfile.LinkedinLink;
            userDetail.TwitterLink = UserProfile.TwitterLink;
            userDetail.ModifiedDate = DateTime.Now;
            userDetail.ModifiedBy = userId;
            userDetail.CreatedBy = userId;
            if (UserProfile.Photo != null)
            {
                string folder = "images/ProfilePicture/";
                folder += Guid.NewGuid().ToString() + "_" + UserProfile.Photo.FileName;
                string serverFolder = Path.Combine(_env.WebRootPath, folder);
                UserProfile.Photo.CopyTo(new FileStream(serverFolder, FileMode.Create));
                userDetail.PhotoPath = folder.ToString();
            }
            if (UserProfile.ResumeFile != null)
            {
                //string filepath = Path.Combine(_env.WebRootPath, userDetail.ResumePath);
                //FileInfo fileInfo = new FileInfo(filepath);
                //if (fileInfo != null)
                //{
                //    File.Delete(filepath);
                //    fileInfo.Delete();
                //}

                //string folder = "fileContent/resume/";
                //folder += Guid.NewGuid().ToString() + "_" + UserProfile.ResumeFile.FileName.Replace(" ", "");
                //string serverFolder = Path.Combine(_env.WebRootPath, folder);
                //UserProfile.ResumeFile.CopyTo(new FileStream(serverFolder, FileMode.Create));
                //userDetail.ResumePath = folder.ToString();
                
                 string folder = "fileContent/resume/";
                    if (!Directory.Exists(Path.Combine(_env.WebRootPath, folder)))
                    {
                        Directory.CreateDirectory(Path.Combine(_env.WebRootPath, folder));
                    }
                    folder += "Resume_" + UserProfile.FirstName + "_"
                        + UserProfile.UserProfileId + System.IO.Path.GetExtension(UserProfile.ResumeFile.FileName);

                  string serverFolder = Path.Combine(_env.WebRootPath, folder);
                    using (FileStream resumefileStream = new FileStream(serverFolder, FileMode.Create))
                    {
                        UserProfile.ResumeFile.CopyTo(resumefileStream);
                    }
                    userDetail.ResumePath = folder.ToString();
                }
            
            UserProfile.PhotoPath = userDetail.PhotoPath;
            UserProfile.Email = UserProfile.Email;
            UserProfile.MobileNumber = UserProfile.MobileNumber;
            UserProfile.UserProfileType = UserProfile.UserProfileType;
            UserProfile.ResumePath = userDetail.ResumePath;
            _XpertAditusDbContext.SaveChanges();
        }
        public UserProfile GetUserInfo(string Id)
        {
            return _XpertAditusDbContext.UserProfile.Where(u => u.LoginId == Id).FirstOrDefault();
        }

        public UserProfile GetUserProfile(ClaimsPrincipal user)
        {
            return _userManager.GetUserProfile(user,_XpertAditusDbContext);
        }

        public bool IsAdmissionDone(Guid userprofileId)
        {
            return _XpertAditusDbContext.Admission.Where(e => e.UserProfileId == userprofileId).Count() > 0;
        }

        #region Experience 
        public void SaveUserExperienceInfo(Experience[] experiencesInfo, string Id)
        {

            Guid profileId = _XpertAditusDbContext.UserProfile.Where(u => u.LoginId == Id).
                Select(u => u.UserProfileId).FirstOrDefault();
            for (int i = 0; i < experiencesInfo.Length; i++)
            {
                if (experiencesInfo[i].ExperienceId == Guid.Empty)
                {
                        Experience experience = new Experience();
                        experience.ExperienceId = Guid.NewGuid();
                        experience.UserProfileId = profileId;
                        experience.CompanyName = experiencesInfo[i].CompanyName;
                        experience.StartDate = experiencesInfo[i].StartDate;
                        experience.EndDate = experiencesInfo[i].EndDate;
                        experience.CompanyContact = experiencesInfo[i].CompanyContact;
                        experience.DesignationName = experiencesInfo[i].DesignationName;
                        experience.ChronologicalOrder = i + 1;
                        experience.CreatedDate = DateTime.Now;
                        experience.CreatedBy = Id;
                        _XpertAditusDbContext.Experience.Add(experience);
                        _XpertAditusDbContext.SaveChanges();
                }
                else
                {
                    var experience = _XpertAditusDbContext.Experience.Where(e => e.ExperienceId == experiencesInfo[i].ExperienceId).FirstOrDefault();
                    experience.CompanyName = experiencesInfo[i].CompanyName;
                    experience.StartDate = experiencesInfo[i].StartDate;
                    experience.EndDate = experiencesInfo[i].EndDate;
                    experience.CompanyContact = experiencesInfo[i].CompanyContact;
                    experience.DesignationName = experiencesInfo[i].DesignationName;
                    experience.ModifiedDate = DateTime.Now;
                    experience.ModifiedBy = Id;
                    _XpertAditusDbContext.SaveChanges();
                }
            }
        }

        public List<Experience> GetUserExperienceInfo(string id)
        {
            Guid profileId = _XpertAditusDbContext.UserProfile.Where(u => u.LoginId == id).Select(u => u.UserProfileId).FirstOrDefault();
            var experienceInfo = _XpertAditusDbContext.Experience.Where(a => a.UserProfileId == profileId).OrderBy(a => a.ChronologicalOrder).ToList();
            return experienceInfo;
        }
        public void DeleteExperienceInfo(Experience experiencesInfo)
        {
            var experience = _XpertAditusDbContext.Experience.Where(q => q.ExperienceId == experiencesInfo.ExperienceId).FirstOrDefault();
            _XpertAditusDbContext.Experience.Remove(experience);
            _XpertAditusDbContext.SaveChanges();
        }
        #endregion

        public void deleteResume(string userId)
        {
            var userDetail = _XpertAditusDbContext.UserProfile.Where(u => u.LoginId == userId).FirstOrDefault();
                string filepath = Path.Combine(_env.WebRootPath, userDetail.ResumePath);
                FileInfo fileInfo = new FileInfo(filepath);
                if (fileInfo != null)
                {
                    File.Delete(filepath);
                    fileInfo.Delete();
                }
                userDetail.ResumePath = null;
                _XpertAditusDbContext.SaveChanges();
        }
        #region Qualification

        public void SaveUserQualificationInfo(Qualification[] qualificationInfo, string Id)
        {
            Guid profileId = _XpertAditusDbContext.UserProfile.Where(u => u.LoginId == Id).Select(u => u.UserProfileId).FirstOrDefault();
            for (int i = 0; i < qualificationInfo.Length; i++)
            {
                if (qualificationInfo[i].QualificaitonId == Guid.Empty)
                {
                    Qualification qualification = new Qualification();
                    qualification.QualificaitonId = Guid.NewGuid();
                    qualification.UserProfileId = profileId;
                    qualification.UniversityName = qualificationInfo[i].UniversityName;
                    qualification.Name = qualificationInfo[i].Name;
                    qualification.Percentage = qualificationInfo[i].Percentage;
                    qualification.CompletionYear = qualificationInfo[i].CompletionYear;
                    qualification.ChronologicalOrder = i + 1;
                    qualification.CreatedBy = Id;
                    qualification.TotalMarks = qualificationInfo[i].TotalMarks;
                    qualification.MarksObtained = qualificationInfo[i].MarksObtained;
                    qualification.CreatedDate = DateTime.Now;
                    qualification.IsHighestQualification = qualificationInfo[i].IsHighestQualification;
                    qualification.EducationId = qualificationInfo[i].EducationId;
                    _XpertAditusDbContext.Qualification.Add(qualification);
                    _XpertAditusDbContext.SaveChanges();
                }
                else
                {
                    var qualification = _XpertAditusDbContext.Qualification.Where(q => q.QualificaitonId == qualificationInfo[i].QualificaitonId).FirstOrDefault();
                    qualification.UniversityName = qualificationInfo[i].UniversityName;
                    qualification.Name = qualificationInfo[i].Name;
                    qualification.Percentage = qualificationInfo[i].Percentage;
                    qualification.CompletionYear = qualificationInfo[i].CompletionYear;
                    qualification.TotalMarks = qualificationInfo[i].TotalMarks;
                    qualification.MarksObtained = qualificationInfo[i].MarksObtained;
                    qualification.ModifiedBy = Id;
                    qualification.ModifiedDate = DateTime.Now;
                    qualification.IsHighestQualification = qualificationInfo[i].IsHighestQualification;
                    qualification.EducationId = qualificationInfo[i].EducationId;
                    _XpertAditusDbContext.Qualification.Update(qualification);
                    _XpertAditusDbContext.SaveChanges();
                }
            }
        }


        public dynamic GetUserQualificationInfo(string id)
        {
            Guid profileId = _XpertAditusDbContext.UserProfile.Where(u => u.LoginId == id).Select(u => u.UserProfileId).FirstOrDefault();
            dynamic qualificationInfo = _XpertAditusDbContext.Qualification.Where(a => a.UserProfileId == profileId).OrderBy(a => a.ChronologicalOrder).ToList();
            return qualificationInfo;
        }

        public void DeleteQualificationInfo(string qualificationInfo)
        {
            var qualification = _XpertAditusDbContext.Qualification.Where(q => q.QualificaitonId == new Guid(qualificationInfo)).FirstOrDefault();
            _XpertAditusDbContext.Qualification.Remove(qualification);
            _XpertAditusDbContext.SaveChanges();
        }

        #endregion

        #region Address

        public Address GetAddress(Guid id)
        {
            return _XpertAditusDbContext.Address.Where(a => a.UserProfileId == id && a.AddressType == "Home").FirstOrDefault();
        }
        public bool IsProfileComplete(Guid id)
        {
             var AddressInfo = GetAddress(id);
            if (AddressInfo != null)
            {
                if (AddressInfo.CountryId != null && AddressInfo.StateId != null && AddressInfo.CityId != null && AddressInfo.DistrictId != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public List<Address> GetUserAddressInfo(string id)
        {
            Guid profileId = _XpertAditusDbContext.UserProfile.Where(u => u.LoginId == id).Select(u => u.UserProfileId).FirstOrDefault();
            var experienceInfo = _XpertAditusDbContext.Address.Where(a => a.UserProfileId == profileId).ToList();
            return experienceInfo;
        }

        public void SaveUserAddress(Address[] address, string Id)
        {
            Guid profileId = _XpertAditusDbContext.UserProfile.Where(u => u.LoginId == Id).Select(u => u.UserProfileId).FirstOrDefault();
            for (int i = 0; i < address.Length; i++)
            {
                try
                {
                    if (address[i].AddressId == Guid.Empty)
                    {
                        Address Address = new Address();
                        Address.AddressId = Guid.NewGuid();
                        Address.UserProfileId = profileId;
                        Address.AddressType = address[i].AddressType;
                        Address.Line1 = address[i].Line1;
                        Address.Line2 = address[i].Line2;
                        Address.Line3 = address[i].Line3;
                        Address.CountryId = address[i].CountryId == Guid.Empty ? null : address[i].CountryId;
                        Address.StateId = address[i].StateId == Guid.Empty ? null : address[i].StateId;
                        Address.DistrictId = address[i].DistrictId == Guid.Empty ? null : address[i].DistrictId;
                        Address.CityId = address[i].CityId == Guid.Empty ? null : address[i].CityId;
                        Address.CreatedBy = Id;
                        Address.CreatedDate = DateTime.Now;
                        _XpertAditusDbContext.Address.Add(Address);
                        _XpertAditusDbContext.SaveChanges();
                    }
                    else
                    {
                        var Address = _XpertAditusDbContext.Address.Where(q => q.AddressId == address[i].AddressId).FirstOrDefault();
                        Address.Line1 = address[i].Line1;
                        Address.Line2 = address[i].Line2;
                        Address.Line3 = address[i].Line3;
                        Address.CountryId = address[i].CountryId == Guid.Empty ? null : address[i].CountryId;
                        Address.StateId = address[i].StateId == Guid.Empty ? null : address[i].StateId;
                        Address.DistrictId = address[i].DistrictId == Guid.Empty ? null : address[i].DistrictId;
                        Address.CityId = address[i].CityId == Guid.Empty ? null : address[i].CityId;
                        Address.ModifiedBy = Id;
                        Address.ModifiedDate = DateTime.Now;
                        _XpertAditusDbContext.SaveChanges();
                    }
                }
                catch(Exception ex)
                {

                }
               
            }
        }
        #endregion

        #region GetAddress

        public List<CountryMaster> GetCountries()
        {
            var countriesInfo = _XpertAditusDbContext.CountryMaster.Where(s => s.Name == "India").ToList();
            return countriesInfo;
        }

        public List<StateMaster> GetStates(string id = "")
        {
            var statesInfo = _XpertAditusDbContext.StateMaster.OrderBy(c => c.Name).ToList();

            if (id != "")
            {
             statesInfo = _XpertAditusDbContext.StateMaster.Where(s => s.CountryId == new Guid(id)).OrderBy(c => c.Name).ToList();
            }

            return statesInfo;
        }

        public List<DistrictMaster> GetDistricts(string id)
        {
            var districtInfo = _XpertAditusDbContext.DistrictMaster.Where(d=> d.StateId == new Guid(id)).OrderBy(c => c.Name).ToList();
            return districtInfo;
        }

        public List<CityMaster> GetCities(string id)
        {
            var cityInfo = _XpertAditusDbContext.CityMaster.Where(c => c.DistrictId == new Guid(id)).OrderBy(c => c.Name).ToList();
            return cityInfo;
        }

        public dynamic GetUserOfficeAddressInfo(Guid Id)
        {
            dynamic OfficeAddress = (from ADD in _XpertAditusDbContext.Address
                                     join CM in _XpertAditusDbContext.CountryMaster
                                     on ADD.CountryId equals CM.CountryId
                                     join STM in _XpertAditusDbContext.StateMaster
                                     on ADD.StateId equals STM.StateId
                                     join DSM in _XpertAditusDbContext.DistrictMaster
                                     on ADD.DistrictId equals DSM.DistrictId
                                     join CTM in _XpertAditusDbContext.CityMaster
                                     on ADD.CityId equals CTM.CityId
                                     where ADD.AddressId == Id
                                     select new
                                     {
                                         CountryName = CM.Name,
                                         StateName = STM.Name,
                                         DistrictName =  DSM.Name,
                                         CityName = CTM.Name
                                     }).FirstOrDefault();

            return OfficeAddress;
        }

        public dynamic GetUserHomeAddressInfo(Guid Id)
        {
            dynamic HomeAddress = (from ADD in _XpertAditusDbContext.Address
                                   join CM in _XpertAditusDbContext.CountryMaster
                                   on ADD.CountryId equals CM.CountryId
                                   join STM in _XpertAditusDbContext.StateMaster
                                   on ADD.StateId equals STM.StateId
                                   join DSM in _XpertAditusDbContext.DistrictMaster
                                   on ADD.DistrictId equals DSM.DistrictId
                                   join CTM in _XpertAditusDbContext.CityMaster
                                   on ADD.CityId equals CTM.CityId
                                   where ADD.AddressId == Id
                                   select new
                                   {
                                       CountryName = CM.Name,
                                       StateName = STM.Name,
                                       DistrictName = DSM.Name,
                                       CityName = CTM.Name
                                   }).FirstOrDefault();

            return HomeAddress;
        }
        #endregion

        #region Questionnaire
        public List<Questionnaire> GetQuestionnaire()
        {
            Random rand = new Random();
            int toSkip = rand.Next(0, 2);
            var questionnaire = _XpertAditusDbContext.Questionnaire.Skip(toSkip).Take(12).ToList();
            return questionnaire;
        }
        #endregion

        #region GetDisclaimer

        public Disclaimer GetDisclaimer()
        {
            //courseId = Guid.NewGuid('CEA783A1-C656-43D2-97A4-3235AE4FD75B');
            var disclaimer = _XpertAditusDbContext.Disclaimer.FirstOrDefault();
            return disclaimer;
        }

        public Disclaimer GetDisclaimerById(string guid)
        {
            //courseId = Guid.NewGuid('CEA783A1-C656-43D2-97A4-3235AE4FD75B');
            var disclaimer = _XpertAditusDbContext.Disclaimer.Where(e => e.DisclaimerId.ToString() == guid).FirstOrDefault();
            return disclaimer;
        }

        public Disclaimer GetDisclaimerByCourseId(Guid guid)
        {
            //courseId = Guid.NewGuid('CEA783A1-C656-43D2-97A4-3235AE4FD75B');
            var disclaimer = _XpertAditusDbContext.Disclaimer.Where(e => e.CourseId == guid).FirstOrDefault();
            return disclaimer;
        }

        public void SaveDisclaimerInfo(Guid DisclaimerId, UserProfile userProfile, Guid usercourseid)
        {
            DisclaimerAccetped disclaimer = new DisclaimerAccetped();
            {
                disclaimer.DisclaimerAcceptedId = Guid.NewGuid();
                disclaimer.DisclaimerId = DisclaimerId;
                disclaimer.IsAccepted = true;
                disclaimer.UserProfileId = userProfile.UserProfileId;
                disclaimer.CreatedDate = DateTime.Now;
                disclaimer.CreatedBy = userProfile.LoginId;
                disclaimer.UserCoursesId = usercourseid;
                _XpertAditusDbContext.DisclaimerAccetped.Add(disclaimer);
                _XpertAditusDbContext.SaveChanges();
            }
        }

        public void SavePADisclaimerInfo(Guid DisclaimerId, UserProfile userProfile, string monthlytestid)
        {
            try
            {
                PadisclaimerAccetped padisclaimer = new PadisclaimerAccetped();
                {
                    padisclaimer.PadisclaimerId = DisclaimerId;
                    padisclaimer.IsAccepted = true;
                    padisclaimer.UserProfileId = userProfile.UserProfileId;
                    padisclaimer.CreatedDate = DateTime.Now;
                    padisclaimer.CreatedBy = userProfile.LoginId;
                    padisclaimer.MonthlyTestId = new Guid(monthlytestid);
                    _XpertAditusDbContext.PadisclaimerAccetped.Add(padisclaimer);
                    _XpertAditusDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public DisclaimerAccetped GetDisclaimerAcceptedById(string userid, string guid, string usercourseId)
        {
            Guid profileId = _XpertAditusDbContext.UserProfile.Where(u => u.LoginId == userid).
               Select(u => u.UserProfileId).FirstOrDefault();
            var disclaimerAccetped = _XpertAditusDbContext.DisclaimerAccetped
                .Where(e => e.DisclaimerId.ToString() == guid && e.UserProfileId == profileId 
                && e.IsAccepted == true && e.UserCoursesId.ToString() == usercourseId).FirstOrDefault();
            return disclaimerAccetped;
        }
        #endregion

        #region JobInfo
        public List<UserProfile> GetAppliedJobsInfo(Guid jobId, int sortBy = 0)
        {

            var data = (from m in _XpertAditusDbContext.AppliedJobs
                        join n in _XpertAditusDbContext.UserProfile
                        on m.UserProfileId equals n.UserProfileId
                        where m.JobId == jobId
                        select n).AsQueryable();

            if (sortBy != 0)
            {
                if (sortBy == 1)
                {
                    data = data.OrderByDescending(e => e.CreatedDate);
                }
                if (sortBy == 2)
                {
                    data = data.OrderBy(j => j.CreatedDate);
                }
            }
            return data.ToList();
        }


        public List<JobMaster> GetJobInfo(int sortBy = 0, string city = "", string category = "", string keyword = "",
            bool SalaryRange1 = false, bool SalaryRange2 = false, bool SalaryRange3 = false,
            bool SalaryRange4 = false, bool SalaryRange5 = false, int? valuetake = 10, int? valueskip = 0)
        {

            var data = _XpertAditusDbContext.JobMaster
                        .Where(j => j.IsActive == "True")
                      .AsQueryable();

            var query = (from j in _XpertAditusDbContext.JobMaster
                         select new JobMaster
                         {
                             JobDesignation = j.JobDesignation,
                             CompanyName = j.CompanyName,
                             CountryId = j.CountryId,
                             StateId = j.StateId,
                             CityId = j.CityId,
                             MinExperience = j.MinExperience,
                             MaxExperience = j.MaxExperience,
                             Description = j.Description,
                             LastDate = j.LastDate,
                             Ctc = j.Ctc,
                             ExpectedJoiningDate = j.ExpectedJoiningDate,
                             NoOfOpening = j.NoOfOpening,
                             Category = j.Category,
                             Industry = j.Industry,
                             WorkShift = j.WorkShift,
                             JobLevel = j.JobLevel,
                             IsActive = j.IsActive,
                             JobId = j.JobId,
                             CityName = _XpertAditusDbContext.CityMaster.Where(c => c.CityId == j.CityId).Select(c => c.Name).FirstOrDefault(),
                             CreatedDate = j.CreatedDate,
                         });

            if (valuetake == null && valueskip == null)
            {
                int take = 10;
                int skip = 0;
                data = query.Skip(skip).Take(take);
            }
            else
            {
                int skip = Convert.ToInt32(valueskip);
                int take = Convert.ToInt32(valuetake);
                data = query.Skip(skip).Take(take);
            }

            if (sortBy == (int)Service.SortBy.Newest)
            {
                data = query.OrderByDescending(j => j.CreatedDate);
            }
            else if (sortBy == (int)Service.SortBy.Oldest)
            {
                data = query.OrderBy(j => j.CreatedDate);
            }
            else if (sortBy == 0)
            {
                data = data;
            }

            if (city != null && city != "")
            {
                data = data.Where(j => city.Contains(j.CityName));
            }

            if (category != null && category != "")
            {
                data = data.Where(j => category.Contains(j.Category));
            }

            if (keyword != null && keyword != "")
            {
                data = data.Where(j => j.JobDesignation.Contains(keyword));
            }

            if (SalaryRange1 == true)
            {
                data = data.Where(j => Convert.ToDecimal(j.Ctc) <= 10000);
            }

            if (SalaryRange2 == true)
            {
                data = data.Where(j => Convert.ToDecimal(j.Ctc) >= 10000 && Convert.ToDecimal(j.Ctc) <= 30000);
            }

            if (SalaryRange3 == true)
            {
                data = data.Where(j => Convert.ToDecimal(j.Ctc) >= 30000 && Convert.ToDecimal(j.Ctc) <= 50000);
            }

            if (SalaryRange4 == true)
            {
                data = data.Where(j => Convert.ToDecimal(j.Ctc) >= 50000 && Convert.ToDecimal(j.Ctc) <= 100000);
            }

            if (SalaryRange5 == true)
            {
                data = data.Where(j => Convert.ToDecimal(j.Ctc) >= 100000);
            }


            return data.ToList();
        }

        public List<JobMaster> GetJobInfoById(Guid jobId)
        {
            return _XpertAditusDbContext.JobMaster.Where(j => j.IsActive == "True" && j.JobId == jobId).ToList();
        }


        public List<AppliedJobs> GetAppliedJobs(Guid UserProfileId)
        {
            return _XpertAditusDbContext.AppliedJobs.Where(a => a.UserProfileId == UserProfileId && a.Status == "Applied").ToList();
        }
        public AppliedJobs GetAppliedJobsById(Guid jobId, Guid UserProfileId)
        {
            return _XpertAditusDbContext.AppliedJobs.Where(a => a.JobId == jobId && a.UserProfileId == UserProfileId && a.Status == "Applied").FirstOrDefault();
        }

        public List<JobMaster> GetAppliedJobList(Guid UserProfileId)
        {
            // var appliedjoblist = _XpertAditusDbContext.AppliedJobs.Where(a => a.UserProfileId == UserProfileId && a.Status == "Applied").ToList();
            var appliedlist = (from Jm in _XpertAditusDbContext.JobMaster
                               join Aj in _XpertAditusDbContext.AppliedJobs
                               on Jm.JobId equals Aj.JobId
                               where Aj.UserProfileId == UserProfileId
                               select Jm).ToList();


            return appliedlist;
        }

        public List<UserProfile> GetAppliedCandidate(Guid JobId)
        {
            var appliedCandidateList = (from Aj in _XpertAditusDbContext.AppliedJobs
                                        join Up in _XpertAditusDbContext.UserProfile
                                        on Aj.UserProfileId equals Up.UserProfileId
                                        where Aj.JobId == JobId
                                        select Up).ToList();

            return appliedCandidateList;

        }

        #endregion

        #region CollegeInfo
        public CollegeProfile GetCollegeProfileInfo(string Id)
        {
            return _XpertAditusDbContext.CollegeProfile.Where(u => u.LoginId == Id).FirstOrDefault();
        }

        public async Task SaveCollegeProfileAsync(CollegeProfile CollegeProfile, string userId)
        {
            var collegeDetail = _XpertAditusDbContext.CollegeProfile.Where(u => u.LoginId == userId && u.IsActive == true).FirstOrDefault();

                collegeDetail.Name = CollegeProfile.Name;
                collegeDetail.CollegeContact = CollegeProfile.CollegeContact;
                collegeDetail.CollegeEmail = CollegeProfile.CollegeEmail;
                collegeDetail.CollegeWebsiteLink = CollegeProfile.CollegeWebsiteLink;

                collegeDetail.CollegeAddress = CollegeProfile.CollegeAddress;
                collegeDetail.CountryId = CollegeProfile.CountryId;
                collegeDetail.StateId = CollegeProfile.StateId;
                collegeDetail.DistrictId = CollegeProfile.DistrictId;
                collegeDetail.CityId = CollegeProfile.CityId;

                collegeDetail.About = CollegeProfile.About;
                collegeDetail.Reviews = CollegeProfile.Reviews;
                collegeDetail.Ratings = CollegeProfile.Ratings;

                collegeDetail.UpdatedBy = userId;
                collegeDetail.UpdatedDate = DateTime.Now;

                if (CollegeProfile.Logo != null)
                {
                    string folder = "images/CollegeLogo/";
                    folder += Guid.NewGuid().ToString() + "_" + CollegeProfile.Logo.FileName.Replace(" ", "");
                    string serverFolder = Path.Combine(_env.WebRootPath, folder);
                    CollegeProfile.Logo.CopyTo(new FileStream(serverFolder, FileMode.Create));
                    CollegeProfile.LogoPath = "/" + folder.ToString();
                    collegeDetail.LogoPath = CollegeProfile.LogoPath;
                }
                else
                {
                     CollegeProfile.LogoPath = collegeDetail.LogoPath;
                }
                

            if (CollegeProfile.Attachment != null)
                {
                    //string filepath = Path.Combine(_env.WebRootPath, userDetail.ResumePath);
                    //FileInfo fileInfo = new FileInfo(filepath);
                    //if (fileInfo != null)
                    //{
                    //    File.Delete(filepath);
                    //    fileInfo.Delete();
                    //}

                    string folder = "fileContent/CollegeAttachments/";
                    folder += Guid.NewGuid().ToString() + "_" + CollegeProfile.Attachment.FileName.Replace(" ", "");
                    string serverFolder = Path.Combine(_env.WebRootPath, folder);
                    var fileserver = new FileStream(serverFolder, FileMode.Create);
                    CollegeProfile.Attachment.CopyTo(fileserver);
                fileserver.Dispose();
                    CollegeProfile.AttachementPath = "/" + folder.ToString();
                    collegeDetail.AttachementPath = CollegeProfile.AttachementPath;
                }
                else
                {
                    CollegeProfile.AttachementPath = collegeDetail.AttachementPath;
                }
            _XpertAditusDbContext.SaveChanges();
        }


        public void DeleteAttachment(string userId)
        {
            var collegeProfile = _XpertAditusDbContext.CollegeProfile.Where(u => u.LoginId == userId).FirstOrDefault();
            string path = collegeProfile.AttachementPath.Substring(1, collegeProfile.AttachementPath.Length - 1);
            string filepath = Path.Combine(_env.WebRootPath, path);
            //FileInfo fileInfo = new FileInfo(filepath);
            //if (fileInfo != null)
            //{
            //    File.Delete(filepath);
            //    fileInfo.Delete();
            //}
            if( System.IO.File.Exists(filepath)){
                System.IO.File.Delete(filepath);
            }
            collegeProfile.AttachementPath = null;
            _XpertAditusDbContext.SaveChanges();
        }

        #endregion

        #region CompanyInfo

        public CompanyProfile GetCompanyProfileInfo(string Id)
        {
            return _XpertAditusDbContext.CompanyProfile.Where(u => u.LoginId == Id).FirstOrDefault();
        }

        public async Task SaveCompanyProfileAsync(CompanyProfile CompanyProfile, string userId)
        {
            var companyProfile = _XpertAditusDbContext.CompanyProfile.Where(u => u.LoginId == userId && u.IsActive == true).FirstOrDefault();

            companyProfile.Name = CompanyProfile.Name;
            companyProfile.CompanyContact = CompanyProfile.CompanyContact;
            companyProfile.CompanyEmail = CompanyProfile.CompanyEmail;
            companyProfile.CompanyWebsiteLink = CompanyProfile.CompanyWebsiteLink;

            companyProfile.CompanyAddress = CompanyProfile.CompanyAddress;
            companyProfile.CountryId = CompanyProfile.CountryId;
            companyProfile.StateId = CompanyProfile.StateId;
            companyProfile.DistrictId = CompanyProfile.DistrictId;
            companyProfile.CityId = CompanyProfile.CityId;

            companyProfile.About = CompanyProfile.About;
            companyProfile.Reviews = CompanyProfile.Reviews;
            companyProfile.Ratings = CompanyProfile.Ratings;

            companyProfile.UpdatedBy = userId;
            companyProfile.UpdatedDate = DateTime.Now;

            if (CompanyProfile.Logo != null)
            {
                string folder = "images/CompanyLogo/";
                folder += Guid.NewGuid().ToString() + "_" + CompanyProfile.Logo.FileName.Replace(" ", "");
                string serverFolder = Path.Combine(_env.WebRootPath, folder);
                CompanyProfile.Logo.CopyTo(new FileStream(serverFolder, FileMode.Create));
                CompanyProfile.LogoPath = "/" + folder.ToString();
                companyProfile.LogoPath = CompanyProfile.LogoPath;
            }
            else
            {
                CompanyProfile.LogoPath = companyProfile.LogoPath;
            }


            //if (CollegeProfile.Attachment != null)
            //{
            //    //string filepath = Path.Combine(_env.WebRootPath, userDetail.ResumePath);
            //    //FileInfo fileInfo = new FileInfo(filepath);
            //    //if (fileInfo != null)
            //    //{
            //    //    File.Delete(filepath);
            //    //    fileInfo.Delete();
            //    //}

            //    string folder = "fileContent/CollegeAttachments/";
            //    folder += Guid.NewGuid().ToString() + "_" + CollegeProfile.Attachment.FileName.Replace(" ", "");
            //    string serverFolder = Path.Combine(_env.WebRootPath, folder);
            //    CollegeProfile.Attachment.CopyTo(new FileStream(serverFolder, FileMode.Create));
            //    CollegeProfile.AttachementPath = "/" + folder.ToString();
            //    collegeDetail.AttachementPath = CollegeProfile.AttachementPath;
            //}
            //else
            //{
            //    CollegeProfile.AttachementPath = collegeDetail.AttachementPath;
            //}
            _XpertAditusDbContext.SaveChanges();
        }

        #endregion
        #region CandidateDashBoard

        public List<CandidateDashBoard> GetCandidateInfo(Guid location, int sortBy = 0, string category = "", string keyword = "", int valuetake = 10, int valueskip = 0)
        {
            List<CandidateDashBoard> candidateDashBoards = new List<CandidateDashBoard>();

            List<UserProfile> userProfiles = new List<UserProfile>();

            if (location != Guid.Empty)
            {
                var userProfiles1 = (from UP in _XpertAditusDbContext.UserProfile
                                     join AD in _XpertAditusDbContext.Address
                                     on UP.UserProfileId equals AD.UserProfileId
                                     where
                                     UP.UserProfileType == "Candidate" &&
                                     AD.DistrictId == location
                                     select UP
                                     ).ToList();

                userProfiles = userProfiles1.ToList();
            }
            else
            {
                userProfiles = _XpertAditusDbContext.UserProfile.
                Where(r => r.UserProfileType == "Candidate").ToList();
            }

            var candidateProfile = (from m in _XpertAditusDbContext.UserProfile
                                    join n in _XpertAditusDbContext.Experience
                                    on m.UserProfileId equals n.UserProfileId
                                    where
                                    m.UserProfileType == "Candidate" &&
                                    n.DesignationName == category
                                    select m).ToList();

            if (category != null && category != "")
            {
                userProfiles = candidateProfile.OrderByDescending(m => m.CreatedDate).ToList();
            }

            if (sortBy != 0)
            {
                if (sortBy == 1)
                {
                    userProfiles = userProfiles.OrderByDescending(j => j.CreatedDate).ToList();
                }
                else if (sortBy == 2)
                {
                    userProfiles = (List<UserProfile>)userProfiles.OrderBy(j => j.CreatedDate).ToList();
                }
                else if (sortBy == 3)
                {
                    Random rand = new Random();
                    int toSkip = rand.Next(0, 1);
                    userProfiles = (List<UserProfile>)userProfiles.Skip(toSkip).ToList();
                }
            }

            userProfiles = userProfiles.Skip(valueskip).Take(valuetake).ToList();

            foreach (UserProfile userProfile in userProfiles)
            {
                try
                {
                    CandidateDashBoard candidateDashBoard = new CandidateDashBoard();
                    candidateDashBoard.userProfileId = userProfile.UserProfileId.ToString();
                    candidateDashBoard.CandidateName = userProfile.FirstName + " " + userProfile.LastName;
                    candidateDashBoard.photoPath = userProfile.PhotoPath;

                    Address address = _XpertAditusDbContext.Address.Where(r => r.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
                    if (address != null)
                    {
                        DistrictMaster districtMaster = _XpertAditusDbContext.DistrictMaster.Where(r => r.DistrictId == address.DistrictId).FirstOrDefault();
                        if (districtMaster != null)
                            candidateDashBoard.CityName = districtMaster.Name;
                        else
                            candidateDashBoard.CityName = "";
                    }
                    else
                        candidateDashBoard.CityName = "";

                    UserCourses userCourses = _XpertAditusDbContext.UserCourses.Where(r => r.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
                    if (userCourses != null)
                    {
                        CandidateResult candidateResult = _XpertAditusDbContext.CandidateResult.Where(r => r.UserCoursesId == userCourses.UserCoursesId).FirstOrDefault();

                        if (candidateResult != null)
                            candidateDashBoard.scrore = candidateResult.Score.ToString();
                        else
                            candidateDashBoard.scrore = "";

                    }
                    else
                        candidateDashBoard.scrore = "";

                    if (userCourses != null)
                    {
                        Course course = _XpertAditusDbContext.Course.Where(r => r.CourseId == userCourses.CourseId).FirstOrDefault();
                        candidateDashBoard.courseName = course.Name;
                    }
                    else
                    {
                        candidateDashBoard.courseName = "";
                    }


                    candidateDashBoards.Add(candidateDashBoard);
                }
                catch (Exception ex)
                {

                }
            }

            return candidateDashBoards;
        }
        #endregion
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.Models;
using XpertAditusUI.Service;

namespace XpertAditusUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        UserManager<IdentityUser> _userManager;
        private readonly XpertAditusDbContext _context;
        
        public HomeController(UserManager<IdentityUser> userManager, XpertAditusDbContext context)
        {
            _userManager = userManager;
            _context = context;

		}

		public IActionResult Index()
		{
			List<MonthlyTestData> monthlyTestDatas = new List<MonthlyTestData>();

            try
            {
                
                ViewData["PreAdmission"] = true;
                var userid = this._userManager.GetUserId(this.User);
                var userProfile = this._context.UserProfile.Where(u => u.LoginId == userid).FirstOrDefault();
                if (userProfile != null)
                {
                    ViewData["UserName"] = userProfile.FirstName + " " + userProfile.MiddleName + " " + userProfile.LastName;
                    ViewData["Photo"] = userProfile.PhotoPath;
                    ViewBag.CourseCount = _context.CourseMaster.Where(e => e.IsActive == true).Count();
                    ViewBag.CollegeCount = _context.CollegeProfile.Where(e => e.IsActive == true).Count();
                    var collegeProfile = this._context.CollegeProfile.Where(u => u.LoginId == userid).FirstOrDefault();
                    if (collegeProfile != null)
                    {
                        ViewBag.TotalAppliedStudent = _context.CollegeStudentMapping.Where(c => c.CollegeProfileId == collegeProfile.CollegeProfileId).Count();
                    }
                    ViewBag.MobileNumber = userProfile.MobileNumber;
                    ViewBag.Registration = userProfile.RegistrationNumber;
                    ViewBag.Email = userProfile.Email;
                    Qualification qualification = this._context.Qualification.Where(r => r.UserProfileId == userProfile.UserProfileId).OrderByDescending(r => r.ChronologicalOrder).FirstOrDefault();
                    EducationMaster eduMaster = this._context.EducationMaster.Where(r => r.EducationId.ToString() == qualification.Name).FirstOrDefault();
                    if (qualification != null)
                    {
                        ViewBag.Name = eduMaster == null ? string.Empty : eduMaster.Name;
                        ViewBag.UniversityName = qualification.UniversityName;
                        ViewBag.TotalMarks = qualification.TotalMarks;
                        ViewBag.MarksObtained = qualification.MarksObtained;
                        ViewBag.Percentage = qualification.Percentage;
                        ViewBag.CompletionYear = qualification.CompletionYear;
                    }
                    var userCourse = this._context.UserCourses.Where(e => e.UserProfileId == userProfile.UserProfileId).FirstOrDefault();
                    if (_context.CandidateResult
                                   .Where(r => r.UserCoursesId == userCourse.UserCoursesId && r.IsCleared == true).Count() > 0)
                    {
                        CandidateResult candidateResultss = _context.CandidateResult
                                   .Where(r => r.UserCoursesId == userCourse.UserCoursesId && r.IsCleared == true).FirstOrDefault();
                        ViewBag.Score = string.IsNullOrWhiteSpace(candidateResultss.Score.ToString()) ? 0 : candidateResultss.Score;
                    }
                    else
                    {
                        ViewBag.Score = 0;
                    }
                    if (_context.PpoInfo
                                   .Where(r => r.UserProfileId == userProfile.UserProfileId).Count() > 0)
                    {
                        ViewBag.OfferedSalary = _context.PpoInfo
                                   .Where(r => r.UserProfileId == userProfile.UserProfileId).FirstOrDefault().Salary;
                    }
                    else
                    {
                        ViewBag.OfferedSalary = 0.00;
                    }

                    var admission = this._context.Admission
                        .Where(e => e.UserProfileId == userProfile.UserProfileId &&
                          e.IsActive == true).FirstOrDefault();
                    if (admission != null)
                    {
                        ViewData["PreAdmission"] = false;
                        if (_context.
                                 PamonthlyTest.Where(r => r.CourseId == admission.CourseId).Count() > 0)
                        {
                            ViewData["MonthlyTestDone"] = true;
                            //var candidateResults = _context.CandidateResult
                            //        .Where(r => r.UserCoursesId == admission.CourseId).FirstOrDefault();


							//Admission admission = _context.Admission.Where(r => r.UserProfileId == userProfile.UserProfileId).FirstOrDefault();

							List<PamonthlyTest> pamonthlyTests = _context.
								 PamonthlyTest.Where(r => r.CourseId == admission.CourseId
                                 && ((r.UpdatedDate > admission.CreatedDate.Value)
                                 || r.Year >= admission.CreatedDate.Value.Year)).ToList();

							for (int i = 0; i < pamonthlyTests.Count; i++)
							{

								MonthlyTestData monthlyTestData = new MonthlyTestData();

								PacandidateResult pacandidateResult = _context.PacandidateResult
									.Where(r => r.MonthlyTestId == pamonthlyTests[i].MonthlyTestId
									&& r.UserProfileId == userProfile.UserProfileId)
									.FirstOrDefault();
								DateTime date = new DateTime(int.Parse(pamonthlyTests[i].Year.ToString()), int.Parse(pamonthlyTests[i].Month.ToString()), 1);
								monthlyTestData.Month = date.ToString("MMMM");
								monthlyTestData.Year = pamonthlyTests[i].Year.ToString();
								var exist = monthlyTestDatas.Where(e => e.Month == monthlyTestData.Month
								&& e.Year == monthlyTestData.Year).Count() > 0;
								if (exist)
								{
									monthlyTestData = monthlyTestDatas.Where(e => e.Month == monthlyTestData.Month
								&& e.Year == monthlyTestData.Year).FirstOrDefault();
								}
								else
								{
									monthlyTestData.MCQ_Score = "";
									monthlyTestData.Case_Study_Score = "";
									monthlyTestData.Video_Score = "";
								}


								if (pacandidateResult != null)
								{
									if (pamonthlyTests[i].TestType == "MCQ")
									{
										monthlyTestData.MCQ_Score = pacandidateResult.Score.ToString();
									}
									if (pamonthlyTests[i].TestType == "Video")
									{
										var l = _context.PavideoQuestionResult.Where(e => e.CandidateResultId
										== pacandidateResult.PacandidateResultId).ToList();
										var score = _context.PavideoQuestionResult.Where(e => e.CandidateResultId
										== pacandidateResult.PacandidateResultId).Sum(e => e.Score);

										monthlyTestData.Video_Score = score.ToString();
									}
									if (pamonthlyTests[i].TestType == "CaseStudy")
									{
										var score = _context.PatestCaseAttachments.Where(e => e.PaCandidateResultId
										== pacandidateResult.PacandidateResultId).Sum(e => e.Score);

										monthlyTestData.Case_Study_Score = score.ToString();
									}
								}
								if (exist)
								{
									monthlyTestDatas.Remove(monthlyTestData);
									monthlyTestDatas.Add(monthlyTestData);

								}
								else
								{
									monthlyTestDatas.Add(monthlyTestData);
								}
							}

						}
						else
						{
							ViewData["MonthlyTestDone"] = false;
							//ViewData["MonthlyTestData"] = new List<MonthlyTestData>();

						}
					}
					else
					{
						ViewData["PreAdmission"] = true;
					}

				}
				else
				{
					ViewData["UserName"] = "";
					ViewData["Photo"] = "";
				}
			}
			catch (Exception ex)
			{

			}
			GetTotalCountAppliedForCollege();
			ViewData["MonthlyTestData"] = monthlyTestDatas;

			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public void GetTotalCountAppliedForCollege()
        {
            //var userid = this._userManager.GetUserId(this.User);
            //var collegeProfile = this._context.CollegeProfile.Where(u => u.LoginId == userid).FirstOrDefault();
            
            try
            {
                var userProfile = this._userManager.GetUserProfile(this.User, this._context);
                var userCourse = this._context.UserCourses.Where(e => e.UserProfileId == userProfile.UserProfileId &&
                              e.IsActive == true).FirstOrDefault();

                CandidateResult candidateResults = _context.CandidateResult
                       .Where(r => r.UserCoursesId == userCourse.UserCoursesId && r.IsActive == true).FirstOrDefault();
                Qualification qualification = _context.Qualification.Where(r => r.UserProfileId == userProfile.UserProfileId)
                .FirstOrDefault();

                List<CollegeCourseSpecializationMapping> collegeCourseSpecializationMappings
                = _context.CollegeCourseSpecializationMapping.Distinct().ToList();
                List<string> collegeProfiles = new List<string>();
                List<string> eligibleCourses = new List<string>();

                if (candidateResults != null & candidateResults.IsCleared.Value)
                {


                    for (int i = 0; i < collegeCourseSpecializationMappings.Count; i++)
                    {
                        if (collegeCourseSpecializationMappings[i] != null)
                        {
                            if (collegeCourseSpecializationMappings[i].MinTestScore <= candidateResults.Score
                                && collegeCourseSpecializationMappings[i].MinAcademicPercentage <= qualification.Percentage)
                            {
                                if (!collegeProfiles.Contains(collegeCourseSpecializationMappings[i].CollegeProfileId.ToString()))
                                {
                                    collegeProfiles.Add(collegeCourseSpecializationMappings[i].CollegeProfileId.ToString());
                                }
                                if (!eligibleCourses.Contains(collegeCourseSpecializationMappings[i].CourseId.ToString()))
                                {
                                    eligibleCourses.Add(collegeCourseSpecializationMappings[i].CourseId.ToString());
                                }
                            }
                        }
                    }
                    ViewBag.TotalEligibleCollege = collegeProfiles.Count();
                    ViewBag.TotalEligibleCourses = eligibleCourses.Count();
                }
                else
                {
                    ViewBag.TotalEligibleCollege = 0;
                    ViewBag.TotalEligibleCourses = 0;
                }


            }
            catch (Exception ex)
            {
                ViewBag.TotalEligibleCollege = 0;
                ViewBag.TotalEligibleCourses = 0;
                //ViewBag.TotalEligibleCollege = 0;
            }

        }
        

    }

 }

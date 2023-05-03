using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XpertAditusUI.Data;
using XpertAditusUI.Models;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using AutoMapper;
using XpertAditusUI.DTO;

namespace XpertAditusUI.Controllers
{
    public class SelectCourseController: Controller
    {
        private readonly IMapper _mapper;
        private readonly XpertAditusDbContext _context;

        public SelectCourseController(XpertAditusDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public IActionResult SelectCourse()
        {
           // var coursesList = _context.Course.Where(e => e.IsActive == "True").ToList();
           // var courseDTOList = _mapper.Map<List<CourseDTO>>(coursesList);
            ViewBag.Courses = Course();
            return View();
        }
        //[HttpPost]
        //public IActionResult SelectCourse(string Description, string Name, decimal Fee)
        //{
        //    var coursesList = _context.Course.Where(e => e.IsActive == "True").ToList();
        //    var courseDTOList = _mapper.Map<List<CourseDTO>>(coursesList);
        //    ViewBag.Courses = courseDTOList;

        //    if (Name != "0")
        //    {
        //        var course = courseDTOList.Where(e => e.Name == Name).FirstOrDefault();
        //        ViewBag.SelectedCourse = course;
        //    }
        //    return View();
        //}
       [HttpGet("Course")]

       public List<Course> Course()
        {
            return _context.Course.ToList();
        }

    }

}
    


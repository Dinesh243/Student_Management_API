using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAppCore.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Student_Management.Controllers
{
    public class StudentController : Controller
    {
        #region List Student Marks
        [HttpGet]
        public IActionResult ListStudentMarks()
        {

            var StudentList = new List<StudentMarkDetails>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:53377/StudentApi/ListStudentMarks");
                //HTTP GET
                var result = client.GetAsync(client.BaseAddress).Result;
              
                if (result.IsSuccessStatusCode && result.Content != null)
                {
                    StudentList = result.Content.ReadAsAsync<List<StudentMarkDetails>>().Result;
                }
                int regId = StudentList.Select(x => x.RegId).FirstOrDefault();
                ViewBag.userid = regId;

                return View(StudentList);
            }
        }
        #endregion

        #region Log In Page Get And Post Method
        [HttpGet]
        public IActionResult Login()
        {
            TempData["isLoginPage"] = true;
            return View();
        }
        [HttpPost]
        public IActionResult Login(RegisterDetails details)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:53377/StudentApi/Login");
                var result = client.PostAsJsonAsync<RegisterDetails>(client.BaseAddress, details).Result;
                if (result.IsSuccessStatusCode)
                {
                    var LogInDetails = result.Content.ReadAsAsync<RegisterDetails>().Result;
                    if (LogInDetails.IsTeacher)
                    {
                        HttpContext.Session.SetString("RegId", LogInDetails.RegId.ToString());
                        return RedirectToAction("ListStudentMarks", new { RegId = LogInDetails.RegId });
                    }
                    else
                    {
                        HttpContext.Session.SetString("RegId", LogInDetails.RegId.ToString());
                        return RedirectToAction("ShowStudentDetails", new { RegId = LogInDetails.RegId });
                    }

                }
            }
            return RedirectToAction("Login");
        }
        #endregion

        #region Get Student Marks
        [HttpGet]
        public IActionResult GetStudentMark(int StdId)
        {

            if (StdId > 0)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:53377/StudentApi/DataForEditPage?StdId=");
                    var result = client.GetAsync(client.BaseAddress + StdId.ToString()).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<StudentMarkDetails>().Result;
                        return View(readTask);
                    }
                }
            }
            return View();
        }
        #endregion

        #region Save And Edit Student Marks
        [HttpPost]
        public IActionResult SaveandEditMarks(StudentMarkDetails details)
        {
            if (details != null)
            {
                if (details.Excel.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        details.Excel.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);
                        byte[] hj = Convert.FromBase64String(s);
                        // act on the Base64 data
                        details.excelPath = hj;
                        details.Excel.CopyTo(ms);
                        details.Excel = null;
                    }
                }
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:53377/StudentApi/SaveandUpdateMarks?StdId=");
                    var result = client.PostAsJsonAsync<StudentMarkDetails>(client.BaseAddress, details).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ListStudentMarks");
                    }
                }
            }
            return View();
        }
        #endregion

        #region Delete The Student Mark
        public IActionResult DeleteMarks(int StdId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:53377/StudentApi/DeleteMarks?StdId=");
                //HTTP GET
                var result = client.DeleteAsync(client.BaseAddress + StdId.ToString()).Result;

                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListStudentMarks");
                }
            }
            return RedirectToAction("ListStudentMarks");
        }
        #endregion

        #region Show Student Mark Result
        public IActionResult ShowStudentDetails(int RegId)
        {

            var StudentList = new List<StudentMarkDetails>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:53377/StudentApi/ShowStudentMarks?RegId=");
                //HTTP GET
                var result = client.GetAsync(client.BaseAddress + RegId.ToString()).Result;

                if (result.IsSuccessStatusCode && result.Content != null)
                {
                    StudentList = result.Content.ReadAsAsync<List<StudentMarkDetails>>().Result;
                }
                int regId = StudentList.Select(x => x.RegId).FirstOrDefault();
                ViewBag.userid = regId;

                return View(StudentList);
            }
        }
        #endregion
    }

}

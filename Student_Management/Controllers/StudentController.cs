using Microsoft.AspNetCore.Mvc;
using StudentAppCore.Core.Models;
using System;
using System.Collections.Generic;
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
            List<StudentMarkDetails> StudentList = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:53377/StudentApi/ListStudentMarks");

                //HttpGet
                var responseTask = client.GetAsync(client.BaseAddress);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<StudentMarkDetails>>();

                    readTask.Wait();

                    return View(StudentList);

                }
            }
            return View();
        }
        #endregion

        #region Log In Page Get And Post Method
        
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(RegisterDetails details)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:53377/StudentApi/ListStudentMarks");
                var postTask = client.PostAsJsonAsync<RegisterDetails>(client.BaseAddress, admin);
                postTask.Wait();
            }


                return View();
        }
        #endregion
        [HttpGet]
        public IActionResult GetStudentMark()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SaveandEditMarkss(RegisterDetails details)
        {
            return View();
        }

    }

}

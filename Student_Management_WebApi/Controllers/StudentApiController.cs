using Microsoft.AspNetCore.Mvc;
using StudentAppCore.Core.IService;
using StudentAppCore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Student_Management_WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class StudentApiController : Controller
    {
        private Iservice _Iservice;
        public StudentApiController(Iservice Iservice)
        {
            _Iservice = Iservice;
        }
        [HttpGet]
        public IActionResult ListStudentMarks()
        {
            //List<StudentMarkDetails> marks = new List<StudentMarkDetails>();
            var marks = _Iservice.ListMarks();
            return Ok(marks);
        }

    }
}

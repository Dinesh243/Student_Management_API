using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAppCore.Core.IService;
using StudentAppCore.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Student_ManagementWebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class StudentApiController : ControllerBase
    {
        readonly Iservice _Iservice;
        
        public StudentApiController(Iservice Iservice)
        {
            _Iservice = Iservice;
        }

        [HttpPost]
        public IActionResult Login(RegisterDetails details)
        {
            if (details != null)
            {
                var Logindetails = _Iservice.LogInDetails(details);
                if (Logindetails != null && Logindetails.RegId > 0 && Logindetails.IsTeacher)
                {
                    return Ok(Logindetails);
                }
                else if (Logindetails != null && Logindetails.RegId > 0 && !Logindetails.IsTeacher)
                {
                    return Ok(Logindetails);
                }
            }
            return NotFound();
        }

        [HttpGet]
        public IActionResult GetStudentMarks()
        {
            return Ok();
        }
    }

}



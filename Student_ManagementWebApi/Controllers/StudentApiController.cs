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
        #region Declration
        readonly Iservice _Iservice;
        public StudentApiController(Iservice Iservice)
        {
            _Iservice = Iservice;
        }
        #endregion

        #region Log In Page
        [HttpPost]
        public IActionResult Login(RegisterDetails details)
        {

            var LogInDetails = _Iservice.LogInDetails(details);
            return Ok(LogInDetails);
        }
        #endregion

        #region List Student Marks
        [HttpGet]
        public IActionResult ListStudentMarks()
        {
            var list = _Iservice.ListMarks();
            return Ok(list);
        }
        #endregion

        #region Save and Update Student Marks
        [HttpPost]
        public IActionResult SaveandUpdateMarks(List<StudentMarkDetails> marks)
        {
             _Iservice.AddandUpdateMarks(marks);
            return Ok();
        }
        [HttpPost]
        public IActionResult UpdateMarks(StudentMarkDetails marks)
        {
             _Iservice.UpdateMarks(marks);
            return Ok();
        }
        #endregion

        #region Delete Student Marks
        [HttpDelete]
        public IActionResult DeleteMarks(int StdId)
        {
          var delete= _Iservice.DeleteStudentMarks(StdId);
            return Ok(delete);
        }
        #endregion

        #region Give student marks to Edit Page
        [HttpGet]
        public IActionResult DataForEditPage(int StdId)
        {
           var Edit = _Iservice.GivenDataToEdit(StdId);
            return Ok(Edit);
        }
        #endregion

        #region Show Student Result
        [HttpGet]
        public IActionResult ShowStudentMarks(int RegId)
        {
            var list = _Iservice.ViewStudentMarks(RegId);
            return Ok(list);
        }
        #endregion

    }
}



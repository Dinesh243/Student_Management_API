using System;
using System.Collections.Generic;
using System.Text;

namespace StudentAppCore.Core.Models
{
    public class NewStudentRegitration
    {
        public int RegId { get; set; }
        public string RegisterName { get; set; }
        public string Password { get; set; }
        public bool IsTeacher { get; set; }
        public string IsTeach { get; set; }
    }
}

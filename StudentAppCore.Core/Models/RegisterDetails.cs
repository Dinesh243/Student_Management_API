using System;
using System.Collections.Generic;
using System.Text;

namespace StudentAppCore.Core.Models
{
    public class RegisterDetails
    {
        public int RegId { get; set; }      
        public string RegisterName { get; set; }       
        public string Password { get; set; }
        public bool IsTeacher { get; set; }
    }
}

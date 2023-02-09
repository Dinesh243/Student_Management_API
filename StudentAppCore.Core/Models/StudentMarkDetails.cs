using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentAppCore.Core.Models
{
    public class StudentMarkDetails
    {
        public int StudentId { get; set; }
        public int RegId { get; set; }
        public string StudentName { get; set; }
        public int RollNo { get; set; }
        public int Tamil { get; set; }
        public int English { get; set; }
        public int Maths { get; set; }
        public int Science { get; set; }
        public int Social { get; set; }
        public decimal Average { get; set; }
        public int Total { get; set; }
        public IFormFile Excel { get; set; }
        public byte[] excelPath { get; set; }
        public string excelstr { get; set; }
    }
}

using LinqToExcel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAppCore.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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
            List<StudentMarkDetails> data = new List<StudentMarkDetails>();
            if (details.Excel != null)
            {
                byte[] file;

                if (details != null)
                {
                    if (details.Excel.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            details.Excel.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            string s = Convert.ToBase64String(fileBytes);
                            file = Convert.FromBase64String(s);
                            // act on the Base64 data
                            details.excelPath = file;
                            details.Excel.CopyTo(ms);
                            //details.Excel = null;
                        }
                    }
                    string FileFormat = details.Excel.FileName;
                    if (FileFormat.EndsWith(".xlsx") || FileFormat.EndsWith(".xls"))
                    {
                        string incorrect;

                        if (details.excelPath != null)
                        {
                            //var fileString = Encoding.ASCII.GetString(details.excelPath);

                            string filePath = "D:\\Web Api Crud Operation\\Student_Management\\Student_Management_WebApi\\Student_Management\\wwwroot\\Excel\\Marks" + ".xlsx";

                            string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                            using (FileStream fs = new FileStream(filePath, FileMode.Create))
                            {
                                using (BinaryWriter bs = new BinaryWriter(fs))
                                {
                                    //string s1 = file1 + ".xlsx";
                                    bs.Write(details.excelPath);
                                    bs.Close();
                                }
                            }

                            using (OleDbConnection connection = new OleDbConnection(connectionString))
                            {
                                connection.Open();
                                var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                                var ds = new DataSet();
                                adapter.Fill(ds, "ExcelTable");
                                DataTable dtable = ds.Tables["ExcelTable"];
                                string sheetName = "Sheet1";
                                var excelFile = new ExcelQueryFactory(filePath);
                                var studentMarks = from a in excelFile.Worksheet<StudentMarkDetails>(sheetName) select a;
                                foreach (var a in studentMarks)
                                {
                                    StudentMarkDetails marks = new StudentMarkDetails();
                                    marks.StudentName = a.StudentName;
                                    if (marks.StudentName == null || marks.StudentName == "" || marks.StudentName.All(char.IsDigit))
                                    {
                                        data = null;
                                        incorrect = "Student name is incorrect . Student Roll No"+marks.RollNo;
                                        break;
                                    }
                                    marks.RollNo = a.RollNo;
                                    if (marks.RollNo == 0 || marks.RollNo.ToString() == "" || marks.RollNo.ToString().All(char.IsLetter))
                                    {
                                        data = null;
                                        incorrect = "Roll No incorect";
                                        break;
                                    }
                                    marks.Tamil = a.Tamil;
                                    if (marks.Tamil < 0 || marks.Tamil.ToString() == "" || marks.Tamil > 100)
                                    {
                                        data = null;
                                        incorrect = "Tamil Marks incorect";
                                        break;
                                    }
                                    marks.English = a.English;
                                    if (marks.English < 0 || marks.English.ToString() == "" || marks.English > 100)
                                    {
                                        incorrect = "English Marks incorect";
                                        break;
                                    }
                                    marks.Maths = a.Maths;
                                    if (marks.Maths < 0 || marks.Maths.ToString() == "" || marks.Maths > 100)
                                    {
                                        data = null;
                                        incorrect = "Maths Marks incorect";
                                        break;
                                    }
                                    marks.Science = a.Science;
                                    if (marks.Science < 0 || marks.Science.ToString() == "" || marks.Science > 100)
                                    {
                                        data = null;
                                        incorrect = "Science Marks incorect";
                                        break;
                                    }
                                    marks.Social = a.Social;
                                    if (marks.Social < 0 || marks.Social.ToString() == "" || marks.Social > 100)
                                    {
                                        data = null;
                                        incorrect = "Social Marks incorect";
                                        break;
                                    }
                                    marks.Total = a.Tamil + a.English + a.Maths + a.Science + a.Social;
                                    marks.Average = (marks.Total / 5);
                                    OleDbCommand cmd = new OleDbCommand();
                                    cmd.Connection = connection;
                                    cmd.CommandText = "UPDATE [Sheet1$] SET Total=@total,Average=@avarege WHERE [RollNo]=@roll;";
                                    cmd.Parameters.AddWithValue("@total", marks.Total);
                                    cmd.Parameters.AddWithValue("@avarege", marks.Average);
                                    cmd.Parameters.AddWithValue("@roll", marks.RollNo);
                                    cmd.Connection = connection;
                                    //connection.Open();
                                    cmd.ExecuteNonQuery();
                                    data.Add(marks);
                                }
                                connection.Close();
                                if ( data.Count>0)
                                {
                                    using (var client = new HttpClient())
                                    {
                                        details.Excel = null;
                                        client.BaseAddress = new Uri("http://localhost:53377/StudentApi/SaveandUpdateMarks?StdId=");
                                        var result = client.PostAsJsonAsync<List<StudentMarkDetails>>(client.BaseAddress, data).Result;
                                        if (result.IsSuccessStatusCode)
                                        {
                                            return RedirectToAction("ListStudentMarks");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToAction("GetStudentMark");
        }
        #endregion
        public IActionResult UpdateMarks(StudentMarkDetails markDetails)
        {
            if (markDetails != null)
            {
                using (var client = new HttpClient())
                {
                    markDetails.Excel = null;
                    client.BaseAddress = new Uri("http://localhost:53377/StudentApi/UpdateMarks?StdId=");
                    var result = client.PostAsJsonAsync<StudentMarkDetails>(client.BaseAddress, markDetails).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ListStudentMarks");
                    }
                }
            }
            return View();
        }

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
        public IActionResult ExcelUpload()
        {
            return View();
        }
    }

}

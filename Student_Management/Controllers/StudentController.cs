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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Student_Management.Controllers
{
    public class StudentController : Controller
    {
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
            if (details != null)
            {
                //DecPass(details.Password);
                //string pass = DecPass(details.Password);
                //details.Password = pass;
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
            }
            return RedirectToAction("Login");
        }
        //public string DecPass(string pass)
        //{
        //    UTF8Encoding encoder = new System.Text.UTF8Encoding();
        //    Decoder utf8Decode = encoder.GetDecoder();
        //    byte[] decode_byte =  Encoding.ASCII.GetBytes(pass);
        //    int Count = utf8Decode.GetCharCount(decode_byte, 0, decode_byte.Length);
        //    char[] decoded_char = new char[Count];
        //    utf8Decode.GetChars(decode_byte, 0, decode_byte.Length, decoded_char, 0);
        //    string result = new String(decoded_char);
        //    return result;
        //}
        #endregion

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

        #region New Student Regitration
        public IActionResult NewStudent()
        {
            return View();
        }
        [HttpPost]
        public IActionResult NewStudent(NewStudentRegitration newStudent)
        {
            if (newStudent != null)
            {
                if (newStudent.IsTeach=="Teacher")
                {
                    newStudent.IsTeacher = true;
                }
                else
                {
                    newStudent.IsTeacher = false;
                }
                
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:53377/StudentApi/NewStudent");
                    var result = client.PostAsJsonAsync<NewStudentRegitration>(client.BaseAddress, newStudent).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Login");
                    }
                }
            }
            return View();
        }
        

        //public string EncryptPassword(string encPass)
        //{
        //    byte[] encDataByte = new byte[encPass.Length];
        //    encDataByte =Encoding.UTF8.GetBytes(encPass);
        //    string encodedData = Convert.ToBase64String(encDataByte);
        //    return encodedData;
        //}

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

        #region Uploading Excel Sheet 
        [HttpPost]
        public IActionResult SaveandEditMarks(StudentMarkDetails details)
        {
            List<StudentMarkDetails> data = new List<StudentMarkDetails>();
            if (details.Excel != null)
            {
                if (details.Excel.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        details.Excel.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);
                        byte[] file = Convert.FromBase64String(s);
                        // act on the Base64 data
                        details.excelPath = file;
                        //details.Excel.CopyTo(ms);
                        //details.Excel = null;
                    }
                }
                string FileFormat = details.Excel.FileName;
                if (FileFormat.EndsWith(".xlsx") || FileFormat.EndsWith(".xls"))
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
                                TempData["ErrorMsg"] = "Student name is incorrect . Student Roll No" ;
                                data = null;
                                break;
                            }
                            marks.RollNo = a.RollNo;
                            if (marks.RollNo == 0 || marks.RollNo.ToString() == "" || marks.RollNo.ToString().All(char.IsLetter))
                            {
                                TempData["ErrorMsg"] = "Roll No incorect"+ marks.StudentName;
                                data = null;
                                break;
                            }
                            marks.Tamil = a.Tamil;
                            if (marks.Tamil < 0 || marks.Tamil.ToString() == "" || marks.Tamil > 100)
                            {
                                TempData["ErrorMsg"] = "Tamil Marks incorect" + marks.RollNo + "check This Roll no in Excel Sheer";
                                data = null;
                                break;
                            }
                            marks.English = a.English;
                            if (marks.English < 0 || marks.English.ToString() == "" || marks.English > 100)
                            {
                                TempData["ErrorMsg"] = "English Marks incorect" + marks.RollNo + "check This Roll no in Excel Sheer";
                                data = null;
                                break;
                            }
                            marks.Maths = a.Maths;
                            if (marks.Maths < 0 || marks.Maths.ToString() == "" || marks.Maths > 100)
                            {
                                TempData["ErrorMsg"] = "Maths Marks incorect" + marks.RollNo + "check This Roll no in Excel Sheer";
                                data = null;
                                break;
                            }
                            marks.Science = a.Science;
                            if (marks.Science < 0 || marks.Science.ToString() == "" || marks.Science > 100)
                            {
                                TempData["ErrorMsg"] = "Science Marks incorect" + marks.RollNo + "check This Roll no in Excel Sheer";
                                data = null;
                                break;
                            }
                            marks.Social = a.Social;
                            if (marks.Social < 0 || marks.Social.ToString() == "" || marks.Social > 100)
                            {
                                TempData["ErrorMsg"] = "Social Marks incorect" + marks.RollNo+"check This Roll no in Excel Sheer";
                                data = null;
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
                            //cmd.Connection = connection;
                            //connection.Open();
                            cmd.ExecuteNonQuery();
                            data.Add(marks);
                        }
                        connection.Close();
                        if (data != null && data.Count > 0)
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
                        else
                        {
                            return RedirectToAction("ExcelUpload");
                        }
                    }
                }
                else
                {
                    TempData["ErrorMsg"] = "Please Upload correct Excel File";
                    return RedirectToAction("ExcelUpload");
                }

            }
            TempData["ErrorMsg"] = "Please Upload Excel File";
            return RedirectToAction("ExcelUpload");
        }
        #endregion

        #region UpdateMarks
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

        #region ExcelUpload GetMethod
        public IActionResult ExcelUpload()
        {
            return View();
        }
        #endregion

    }

}

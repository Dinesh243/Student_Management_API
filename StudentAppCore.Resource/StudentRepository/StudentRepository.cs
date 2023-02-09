using Grpc.Core;
using LinqToExcel;
using Microsoft.AspNetCore.Http;
using StudentAppCore.Core.IRepositry;
using StudentAppCore.Core.Models;
using StudentAppCore.Entity.StudentManagement.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;

namespace StudentAppCore.Resource.StudentRepository
{
    public class StudentRepository : IRepository
    {
        #region List STudent Marks
        public List<StudentMarkDetails> ListMarks()
        {
            List<StudentMarkDetails> Marks = new List<StudentMarkDetails>();
            using (var Entity = new Student_ManagementContext())
            {
                var Dbdetails = Entity.Student_Mark_List.Where(x => !x.Is_Deleted).ToList();
                if (Dbdetails.Count > 0)
                {
                    foreach (var item in Dbdetails)
                    {
                        StudentMarkDetails studentMark = new StudentMarkDetails();
                        studentMark.RegId = item.Reg_Id;
                        studentMark.StudentId = item.Student_Id;
                        studentMark.StudentName = item.Student_Name;
                        studentMark.RollNo = item.Roll_No;
                        studentMark.Tamil = item.Tamil;
                        studentMark.English = item.English;
                        studentMark.Maths = item.Maths;
                        studentMark.Science = item.Science;
                        studentMark.Social = item.Social;
                        studentMark.Total = item.Total;
                        studentMark.Average = item.Average;
                        Marks.Add(studentMark);
                    }
                }
            }
            return Marks;
        }
        #endregion

        #region Login
        public RegisterDetails LogInDetails(RegisterDetails details)
        {
            var login = new RegisterDetails();
            if (details != null)
            {
                using (var Entity = new Student_ManagementContext())
                {
                    var DBdetails = Entity.Student_Registration.Where(x => x.Register_Name == details.RegisterName && x.Password == details.Password && !x.Is_Delated).SingleOrDefault();
                    if (DBdetails != null)
                    {
                        login.RegId = DBdetails.Reg_Id;
                        login.IsTeacher = DBdetails.Is_Teacher;
                    }
                }
            }
            return login;
        }
        #endregion

        #region Save and Edit the Student Marks
        public void AddandUpdateMarks(StudentMarkDetails marks)
        {
            if (marks != null)
            {
                using (var Entity = new Student_ManagementContext())
                {
                    Student_Mark_List Addmarks = null;
                    bool isRecordExist = false;
                    Addmarks = Entity.Student_Mark_List.Where(x => x.Student_Id == marks.StudentId && !x.Is_Deleted).SingleOrDefault();
                    if (Addmarks != null)
                    {
                        isRecordExist = true;
                    }
                    else
                    {
                        Addmarks = new Student_Mark_List();
                    }
                    Addmarks.Reg_Id = 2;
                    Addmarks.Student_Name = marks.StudentName;
                    Addmarks.Roll_No = marks.RollNo;
                    Addmarks.Tamil = marks.Tamil;
                    Addmarks.English = marks.English;
                    Addmarks.Maths = marks.Maths;
                    Addmarks.Science = marks.Science;
                    Addmarks.Social = marks.Social;
                    Addmarks.Total = (marks.Tamil + marks.English + marks.Maths + marks.Science + marks.Social);
                    Addmarks.Average = ((Addmarks.Total) / 5);
                    Addmarks.Updated_Time_Stamp = DateTime.Now;
                    if (isRecordExist == false)
                    {
                        Addmarks.Created_Time_Stamp = DateTime.Now;
                        Addmarks.Is_Deleted = false;
                        Entity.Student_Mark_List.Add(Addmarks);
                    }
                    Entity.SaveChanges();
                    ExcelUpload(marks.excelPath, Addmarks.Student_Id);
                }
            }
        }
        #endregion
        public void ExcelUpload(byte[] file,int Save)
        {
            string incorrectName, incorrectRollno, incorrectTamilmarks, incorrectEnglishmarks, incorrectMathsmarks, incorrectSciencemarks, incorrectSocialmarks, incorrectTotalmarks, incorrectAveragemarks;
            List<StudentMarkDetails> data = new List<StudentMarkDetails>();
            if (file != null)
            {
                var fileString = Encoding.ASCII.GetString(file);
                
                string filePath = "D:\\Web Api Crud Operation\\Student_Management\\Student_Management_WebApi\\Student_Management\\wwwroot\\Excel\\Marks" +Save+ ".xlsx";
                
                string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    using (BinaryWriter bs = new BinaryWriter(fs))
                    {
                        //string s1 = file1 + ".xlsx";
                        bs.Write(file);
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
                        if (marks.StudentName == null || marks.StudentName=="" || marks.StudentName.All(char.IsLetter))
                        {
                           incorrectName = "Student name is incorrect";
                            break;
                        }
                        marks.RollNo = a.RollNo;
                        if (marks.RollNo==0 || marks.RollNo.ToString() == "" || marks.RollNo.ToString().All(char.IsDigit))
                        {
                            incorrectRollno = "Roll No incorect";
                            break;
                        }
                        marks.Tamil = a.Tamil;
                        if (marks.Tamil < 0 || marks.Tamil.ToString() == "" || marks.Tamil>100)
                        {
                            incorrectTamilmarks = "Tamil Marks incorect";
                            break;
                        }
                        marks.English = a.English;
                        if (marks.English < 0 || marks.English.ToString() == "" || marks.English > 100 )
                        {
                            incorrectEnglishmarks = "English Marks incorect";
                            break;
                        }
                        marks.Maths = a.Maths;
                        if (marks.Maths < 0 || marks.Maths.ToString() == "" || marks.Maths > 100)
                        {
                            incorrectMathsmarks = "Maths Marks incorect";
                            break;
                        }
                        marks.Science = a.Science;
                        if (marks.Science < 0 || marks.Science.ToString() == "" || marks.Science > 100)
                        {
                            incorrectSciencemarks = "Science Marks incorect";
                            break;
                        }
                        marks.Social = a.Social;
                        if (marks.Social < 0 || marks.Social.ToString() == "" || marks.Social > 100)
                        {
                            incorrectSocialmarks = "Social Marks incorect";
                            break;
                        }
                        marks.Total = a.Total;
                        if (marks.Total < 0 || marks.Total.ToString() == "" || marks.Total > 500)
                        {
                            incorrectTotalmarks = "Total Marks incorect";
                            break;
                        }
                        marks.Average = a.Average;
                        if (marks.Average < 0 || marks.Tamil.ToString() == "" || marks.Average > 101  )
                        {
                            incorrectAveragemarks = "Average Marks incorect";
                            break;
                        }
                        data.Add(marks);
                    }
                }

            }
        }

        #region Send Student Marks to Edit Page
        public StudentMarkDetails GivenDataToEdit(int StdId)
        {
            StudentMarkDetails Data = new StudentMarkDetails();
            if (StdId > 0)
            {
                using (var Entity = new Student_ManagementContext())
                {
                    var DBdata = Entity.Student_Mark_List.Where(x => x.Student_Id == StdId && !x.Is_Deleted).SingleOrDefault();
                    if (DBdata != null)
                    {
                        Data.RegId = DBdata.Reg_Id;
                        Data.StudentId = DBdata.Student_Id;
                        Data.StudentName = DBdata.Student_Name;
                        Data.RollNo = DBdata.Roll_No;
                        Data.Tamil = DBdata.Tamil;
                        Data.English = DBdata.English;
                        Data.Maths = DBdata.Maths;
                        Data.Science = DBdata.Science;
                        Data.Social = DBdata.Social;
                        
                    }
                }

            }
            return Data;
        }
        #endregion

        #region Delete the Student Marks
        public bool DeleteStudentMarks(int StdId)
        {
            bool IsDeleted = false;

            if (StdId > 0)
            {
                using (var Entity = new Student_ManagementContext())
                {
                    var DBdata = Entity.Student_Mark_List.Where(x => x.Student_Id == StdId && !x.Is_Deleted).SingleOrDefault();
                    if (DBdata != null)
                    {
                        DBdata.Is_Deleted = true;
                        Entity.SaveChanges();
                        IsDeleted = true;
                    }
                }
            }
            return IsDeleted;
        }

        #endregion

        #region Show the student Marks
        public List<StudentMarkDetails> ViewStudentMarks(int RegId)
        {
            List<StudentMarkDetails> Marks = new List<StudentMarkDetails>();
            using (var Entity = new Student_ManagementContext())
            {
                var Dbdetails = Entity.Student_Mark_List.Where(x =>x.Reg_Id== RegId && !x.Is_Deleted).ToList();
                if (Dbdetails.Count > 0)
                {
                    foreach (var item in Dbdetails)
                    {
                        StudentMarkDetails studentMark = new StudentMarkDetails();
                        studentMark.RegId = item.Reg_Id;
                        studentMark.StudentId = item.Student_Id;
                        studentMark.StudentName = item.Student_Name;
                        studentMark.RollNo = item.Roll_No;
                        studentMark.Tamil = item.Tamil;
                        studentMark.English = item.English;
                        studentMark.Maths = item.Maths;
                        studentMark.Science = item.Science;
                        studentMark.Social = item.Social;
                        studentMark.Total = item.Total;
                        studentMark.Average = item.Average;
                        Marks.Add(studentMark);
                    }
                }
            }
            return Marks;
        }
#endregion
    }
}

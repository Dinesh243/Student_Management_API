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
using System.Security.Cryptography;
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
                    var DBdetails = Entity.Student_Registration.Where(x => x.Register_Name == details.RegisterName && !x.Is_Delated).SingleOrDefault();
                    if (DBdetails != null)
                    {
                        bool pass = VerifyPassword(details.Password, DBdetails.Salt, DBdetails.Hash);
                        if (pass)
                        {
                            login.RegId = DBdetails.Reg_Id;
                            login.IsTeacher = DBdetails.Is_Teacher;
                        }
                        login = null;
                    }

                }
            }
            return login;
        }
        #endregion

        #region 
        public bool VerifyPassword(string password, byte[] salt, byte[] hash)
        {
           
            byte[] newHash;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 1000))
            {
                newHash = deriveBytes.GetBytes(8);
            }
            if (newHash.Length != hash.Length)
            {
                return false;
            }
            for (int i = 0; i < newHash.Length; i++)
            {
                if (newHash[i] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }

        #region NewStudent
        public NewStudentRegitration NewStudent(NewStudentRegitration newStudent)
        {
            try
            {
                string salt = GenerateSalt();
                string hash = GenerateHash(newStudent.Password, salt);
                //string hash = GenerateHash(newStudent.Password, salt);
                NewStudentRegitration Obj = new NewStudentRegitration();
                if (newStudent != null)
                {
                    using (var Entity = new Student_ManagementContext())
                    {
                        Student_Registration AddStudent = new Student_Registration();
                       var AddStudents = Entity.Student_Registration.Where(x => x.Register_Name == newStudent.RegisterName && !x.Is_Delated).FirstOrDefault();
                        if (AddStudents == null)
                        {
                            AddStudent.Register_Name = newStudent.RegisterName;
                            AddStudent.Salt = salt;
                            AddStudent.Hash = hash;
                            AddStudent.Is_Teacher = newStudent.IsTeacher;
                            AddStudent.Updated_Time_Stamp = DateTime.Now;
                            AddStudent.Created_Time_Stamp = DateTime.Now;
                            AddStudent.Is_Delated = false;
                            Entity.Student_Registration.Add(AddStudent);
                            Entity.SaveChanges();
                            Obj.IsTeacher = AddStudent.Is_Teacher;
                            Obj.RegId = AddStudent.Reg_Id;
                        }
                    }
                }
                return Obj;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
        public string GenerateSalt()
        {
            byte[] salt = new byte[8];
            using (var random = new RNGCryptoServiceProvider())
            {
                  random.GetBytes(salt);
            }
            string randomSalt = Encoding.Default.GetString(salt);
            return randomSalt;
        }
        //private string CreatePasswordHash(string pwd, string salt)
        //{
        //    string saltAndPwd = String.Concat(pwd, salt);
        //    string hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPwd, "sha1");
        //    return hashedPwd;
        //}
        public string GenerateHash(string password, string salt)
        {
            byte[] saltBytes = Encoding.Default.GetBytes(salt);
            byte[] hash;
            //byte[] saltBytes = salt ;

            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 1000))
            {
                hash = rfc2898DeriveBytes.GetBytes(8);
            }
            return Encoding.Default.GetString(hash);
        }
        #endregion

        #region Save and Edit the Student Marks
        public void AddandUpdateMarks(List<StudentMarkDetails> marks)
        {
            if (marks != null)
            {
                using (var Entity = new Student_ManagementContext())
                {
                    foreach(var Item in marks)
                    {
                        Student_Mark_List Addmarks = new Student_Mark_List();
                        bool isRecordExist = false;
                        Addmarks = Entity.Student_Mark_List.Where(x => x.Student_Id == Item.StudentId && !x.Is_Deleted).SingleOrDefault();
                        if (Addmarks != null)
                        {
                            isRecordExist = true;
                        }
                        else
                        {
                            Addmarks = new Student_Mark_List();
                        }
                        Addmarks.Reg_Id = 2;
                        Addmarks.Student_Name = Item.StudentName;
                        Addmarks.Roll_No = Item.RollNo;
                        Addmarks.Tamil = Item.Tamil;
                        Addmarks.English = Item.English;
                        Addmarks.Maths = Item.Maths;
                        Addmarks.Science = Item.Science;
                        Addmarks.Social = Item.Social;
                        Addmarks.Total = Item.Total;
                        Addmarks.Average = Item.Average;
                        Addmarks.Updated_Time_Stamp = DateTime.Now;
                        if (isRecordExist == false)
                        {
                            Addmarks.Created_Time_Stamp = DateTime.Now;
                            Addmarks.Is_Deleted = false;
                            Entity.Student_Mark_List.Add(Addmarks);
                        }
                        Entity.SaveChanges();
                    }
                }
            }
        }
        #endregion

        #region Edit and Update student marks manually
        public void UpdateMarks(StudentMarkDetails marks)
        {
            if (marks != null)
            {
                using (var Entity = new Student_ManagementContext())
                {

                    Student_Mark_List Addmarks = new Student_Mark_List();
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
                    Addmarks.Average = (Addmarks.Total / 5);
                    Addmarks.Updated_Time_Stamp = DateTime.Now;
                    if (isRecordExist == false)
                    {
                        Addmarks.Created_Time_Stamp = DateTime.Now;
                        Addmarks.Is_Deleted = false;
                        Entity.Student_Mark_List.Add(Addmarks);
                    }
                    Entity.SaveChanges();
                }
            }
        }
        #endregion

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

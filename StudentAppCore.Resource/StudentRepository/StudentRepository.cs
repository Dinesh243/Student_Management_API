using StudentAppCore.Core.IRepositry;
using StudentAppCore.Core.Models;
using StudentAppCore.Entity.StudentManagement.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StudentAppCore.Resource.StudentRepository
{
    public class StudentRepository : IRepository
    {
        public List<StudentMarkDetails> ListMarks()
        {
            List<StudentMarkDetails> Marks = new List<StudentMarkDetails>();
            using (var Entity = new Student_ManagementContext())
            {
                var Dbdetails = Entity.Student_Mark_List.Where(x => !x.Is_Deleted).ToList();
                if(Dbdetails.Count>0)
                {
                    foreach(var item in Dbdetails)
                    {
                        StudentMarkDetails studentMark = new StudentMarkDetails();
                        studentMark.RegId = item.Reg_Id;
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
            return Marks.ToList();
        }
        #region Login
        public RegisterDetails LogInDetails(RegisterDetails details)
        {
            var login = new RegisterDetails();
            if (details!=null)
            {
                using (var Entity = new Student_ManagementContext())
                {
                    var DBdetails = Entity.Student_Registration.Where(x => x.Register_Name == login.RegisterName && x.Password == login.Password && !x.Is_Delated).SingleOrDefault();
                    if(DBdetails!=null)
                    {
                        login.RegId = DBdetails.Reg_Id;
                        login.IsTeacher = DBdetails.Is_Teacher;
                        return login;
                    }
                }
            }
            return login;
        }
        #endregion

        #region Save and Edit the Student Marks
        public void AddandUpdateMarks(StudentMarkDetails marks)
        {
            if(marks!=null)
            {
                using(var Entity = new Student_ManagementContext())
                {
                    Student_Mark_List Addmarks = null;
                    bool isRecordExist = false;
                    Addmarks = Entity.Student_Mark_List.Where(x => x.Student_Id == marks.StudentId && !x.Is_Deleted).SingleOrDefault();
                    if(Addmarks!=null)
                    {
                        isRecordExist = true;
                    }
                    else
                    {
                        Addmarks = new Student_Mark_List();
                    }
                    Addmarks.Reg_Id = marks.RegId;
                    Addmarks.Student_Name = marks.StudentName;
                    Addmarks.Roll_No = marks.RollNo;
                    Addmarks.Tamil = marks.Tamil;
                    Addmarks.English = marks.English;
                    Addmarks.Maths = marks.Maths;
                    Addmarks.Science = marks.Science;
                    Addmarks.Social = marks.Social;
                    Addmarks.Total = (marks.Tamil + marks.English + marks.Maths + marks.Science + marks.Social);
                    Addmarks.Average = ((Addmarks.Total)/5);
                    if (isRecordExist==false)
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

        #region Delete the Student Marks


        #endregion
    }
}

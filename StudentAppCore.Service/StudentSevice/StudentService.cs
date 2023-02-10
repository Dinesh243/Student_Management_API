using StudentAppCore.Core.IRepositry;
using StudentAppCore.Core.IService;
using StudentAppCore.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentAppCore.Service.StudentSevice
{
    public class StudentService : Iservice
    {
        #region Declration
        private IRepository _repository;
        public StudentService(IRepository repository)
        {
            _repository = repository;
        }
        #endregion

        #region List All Student Marks
        public List<StudentMarkDetails> ListMarks()
        {
            return _repository.ListMarks();
        }
        #endregion

        #region LogIn 
        public RegisterDetails LogInDetails(RegisterDetails details)
        {
            return _repository.LogInDetails(details);
        }
        #endregion

        #region Save and Edit Student Marks
        public void AddandUpdateMarks(List<StudentMarkDetails> marks)
        {
            _repository.AddandUpdateMarks(marks);
        }
        public void UpdateMarks(StudentMarkDetails marks)
        {
            _repository.UpdateMarks(marks);
        }
        #endregion

        #region Delete the Student Mark
        public bool DeleteStudentMarks(int StdId)
        {
            return _repository.DeleteStudentMarks(StdId);
        }
        #endregion

        #region Send Student Mark to Edit page
        public StudentMarkDetails GivenDataToEdit(int regId)
        {
            return _repository.GivenDataToEdit(regId);
        }
        #endregion

        #region ViewStudentMarks
        public List<StudentMarkDetails> ViewStudentMarks(int RegId)
        {
            return _repository.ViewStudentMarks(RegId);
        }
        #endregion

    }
}

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
        private IRepository _repository;
        public StudentService(IRepository repository)
        {
            _repository = repository;
        }
        public List<StudentMarkDetails> ListMarks()
        {
            return _repository.ListMarks();
        }
        public RegisterDetails LogInDetails(RegisterDetails details)
        {
            return _repository.LogInDetails(details);
        }
        public void AddandUpdateMarks(StudentMarkDetails marks)
        {
            _repository.AddandUpdateMarks(marks);
        }
    }
}

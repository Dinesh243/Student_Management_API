using StudentAppCore.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentAppCore.Core.IRepositry
{
    public interface IRepository
    {
        List<StudentMarkDetails> ListMarks();
        RegisterDetails LogInDetails(RegisterDetails details);
        void AddandUpdateMarks(StudentMarkDetails marks);
    }
}

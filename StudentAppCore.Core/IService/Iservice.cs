using StudentAppCore.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentAppCore.Core.IService
{
    public interface Iservice
    {
        List<StudentMarkDetails> ListMarks();
        RegisterDetails LogInDetails(RegisterDetails details);
        void AddandUpdateMarks(StudentMarkDetails marks);
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StudentAppCore.Core.IRepositry;
using StudentAppCore.Core.IService;
using StudentAppCore.Resource.StudentRepository;
using StudentAppCore.Service.StudentSevice;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentAppCore.Utility
{
    public class DIResolver
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<Iservice, StudentService>();

            services.AddScoped<IRepository, StudentRepository>();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StudentManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentMarksApiController : ControllerBase
    {
        // GET: api/<StudentMarksApiController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<StudentMarksApiController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<StudentMarksApiController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<StudentMarksApiController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<StudentMarksApiController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        [HttpPut]
        public IActionResult GetResult()
        {
            return Ok();
        }
        [HttpGet]
        public IActionResult Listtables()
        {
            return Ok();
        }
     
    }
}

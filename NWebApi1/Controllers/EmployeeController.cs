using MassTransit;
using Message.Contract;
using Microsoft.AspNetCore.Mvc;

namespace NWebApi1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        public EmployeeController(IPublishEndpoint endpoint) => _publishEndpoint = endpoint;

        [HttpPost(Name = "PostEmpData")]
        public async Task<IActionResult> SubmitEmployeeData([FromBody] Employee employee)
        {
            var message = new Employee
            {
                Id = DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second,
                Name = employee.Name,
                Age = employee.Age,
                Department = employee.Department
            };


            await _publishEndpoint.Publish(message);
            return Ok(new { message = "Employee event published" });
        }
    }
}

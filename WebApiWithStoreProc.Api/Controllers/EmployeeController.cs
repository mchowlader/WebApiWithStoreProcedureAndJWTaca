using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using WebApiWithStoreProc.Api.Model;
using WebApiWithStoreProc.Api.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiWithStoreProc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }


        // GET api/<EmployeeController>/5
        [HttpGet("{id}")]
        public List<Employee> Get(int id)
        {
            var result = _service.GetByEmployee(id);
            return result;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Employee), 200)]
        public IActionResult Post(Employee emp)
        {
           var result = _service.AddEmployee(emp);
           var response = new PayloadResponse<Employee>
            {
                Message = result != null ? result.Message : null,
                Payload = result.Data,
                PayloadType = "Save Customer",
                Success = result != null ? result.Success : false
            };
            var created_obj_id = response.Payload != null ? response.Payload.Id : 0;
            return Created(response.RequestURL + "/" + created_obj_id, response );
        }

        [HttpPut]
        [Route("{id:int}")]
        public IActionResult Put(Employee emp, int id)
        {
            var result = _service.APREST_UpdateEmployee(emp, id);

            var response = new PayloadResponse<Employee>
            {
                Message = result != null ? result.Message : null,
                Payload = result.Data,
                PayloadType = "Update Customer",
                Success = result != null ? result.Success : false
            };
            return Ok(response);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            return default;
        }

    }
}

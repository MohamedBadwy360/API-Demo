using APITask.Core.Interfaces;
using APITask.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APITask.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public EmployeeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("{id:int}", Name = "GetEmployeeById")]
        public IActionResult GetById(int id)
        {
            Employee employee = unitOfWork.Employees.GetById(id);
            if (employee == null)
            {
                return BadRequest("Employee isn't found.");
            }
            return Ok(employee);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(unitOfWork.Employees.GetAll());
        }

        [HttpPost]
        public IActionResult Add(Employee employee)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Employees.Add(employee);
                unitOfWork.Complete();
                string url = Url.Link("GetEmployeeById", new Employee { Id = employee.Id });
                return Created(url, employee);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut]
        public IActionResult Update(Employee employee)
        {
            if (ModelState.IsValid)
            {
                Employee oldEmployee = unitOfWork.Employees.GetById(employee.Id);
                if (oldEmployee == null)
                {
                    return BadRequest("Employee is not found");
                }
                else
                {
                    oldEmployee.Name = employee.Name;
                    oldEmployee.Address = employee.Address;
                    oldEmployee.Salary = employee.Salary;
                    unitOfWork.Complete();
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            Employee employee = unitOfWork.Employees.GetById(id);
            if (employee == null)
            {
                return BadRequest("Employee is not found");
            }
            else
            {
                unitOfWork.Employees.Delete(employee);
                unitOfWork.Complete();
                return StatusCode(StatusCodes.Status204NoContent);
            }
        }
    }
}

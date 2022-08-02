using EmployeeWebApi.Models;
using EmployeeWebApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllEmployees()
        //{
        //    var employees = await _context.Employees.ToListAsync();
        //    return Ok(employees);
        //}


        // GET ALL EMPLOYEES WITH PAGINING

        [HttpGet("page/{page}")]
        public async Task<IActionResult> GetAllEmployees(int page)
        {
            if (_context.Employees == null)
                return NotFound();

            var pageResults = 3f;
            var pageCount = Math.Ceiling(_context.Employees.Count() / pageResults);

            var employees = await _context.Employees
                .Skip((page - 1) * (int)pageResults)
                .Take((int)pageResults)
                .ToListAsync();

            var response = new EmployeeResponse
            {
                Employees = employees,
                CurrentPage = page,
                Pages = (int)pageCount
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("getId/{id}")]
        [ActionName("GetEmployee")]
        public async Task<IActionResult> GetEmployee(Guid id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (employee != null)
            {
                return Ok(employee);
            }
            return NotFound($"Employee with Id: {id} was not found");
        }


        [HttpGet]
        [Route("{search}")]
        public async Task<ActionResult<IEnumerable<Employee>>> Search(string username)
        {
            IQueryable<Employee> query = _context.Employees;
            if (!string.IsNullOrEmpty(username))
            {
                query = query.Where(e => e.Username.Contains(username));
            }

            var result = await query.ToListAsync();
            if (result.Any())
            {
                return Ok(result);
            }
            return BadRequest("Error");

        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            var employeeExistUsername = _context.Employees.Any(x => x.Username == employee.Username);
            var employeeExistEmail = _context.Employees.Any(x => x.Email == employee.Email);
            var employeeExistPhoneNumber = _context.Employees.Any(x => x.PhoneNumber == employee.PhoneNumber);

            if (employeeExistUsername)
            {
                return BadRequest("Username already exist");
            }
            else if (employeeExistEmail)
            {
                return BadRequest("Email already exist");

            }
            else if (employeeExistPhoneNumber)
            {
                return BadRequest("PhoneNumber already exist");
            }
            else
            {
                employee.Id = Guid.NewGuid();
                employee.CreatedAt = DateTime.Now;
                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(AddEmployee), employee.Id, employee);
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            var existingEmployee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (existingEmployee != null)
            {
                _context.Remove(existingEmployee);
                await _context.SaveChangesAsync();
                return Ok("Deleted Success");
            }
            return NotFound($"Employee with Id: {id} was not found");
        }


        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> EditEmployee(Guid id, [FromBody] Employee employee)
        {
            var existingEmployee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (existingEmployee != null)
            {
                employee.Id = id;
                existingEmployee.Username = employee.Username;
                existingEmployee.Email = employee.Email;
                existingEmployee.Password = employee.Password;
                existingEmployee.Birthday = employee.Birthday;
                existingEmployee.Address = employee.Address;
                existingEmployee.PhoneNumber = employee.PhoneNumber;
                existingEmployee.UpdatedAt = DateTime.Now;
                employee.UpdatedAt = existingEmployee.UpdatedAt;
                await _context.SaveChangesAsync();
                return Ok(existingEmployee);
            }
            return NotFound($"Employee with Id: {id} was not found");
        }

    }
}


using EmployeeWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public UserInfoController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(UserInfo userInfo)
        {
            if(userInfo != null && userInfo.Username != null && userInfo.Password != null)
            {
                var user = await GetUser(userInfo.Username, userInfo.Password);
                if(user != null)
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id", user.Id.ToString()),
                        new Claim("Username", user.Username),
                        new Claim("Password", user.Password)
                    };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                            _configuration["Jwt:Issuer"],
                            _configuration["Jwt:Audiece"],
                            claims,
                            expires: DateTime.Now.AddMinutes(20),
                            signingCredentials: signIn
                        );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid Credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

       
        [HttpGet]
        public async Task<Employee> GetUser(string username, string password)
        {
            return await _context.Employees.FirstOrDefaultAsync(x => x.Username == username && x.Password == password);
        }

        [HttpPost]
        [Route("register")]

        public async Task<IActionResult> Register([FromBody] Employee employee)
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

            return CreatedAtAction(nameof(Register), employee.Id, employee);
        }

    }
}

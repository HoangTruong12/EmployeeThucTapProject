using EmployeeWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> LoginUser(Employee user)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("https://localhost:44311/api/UserInfo/login", stringContent))
                {
                    string token = await response.Content.ReadAsStringAsync();
                    
                    if (token == "Invalid Credentials")
                    {
                        TempData["Message"] = "Tài khoản hoặc mật khẩu không đúng";
                        return Redirect("~/Home/Index");
                    }
                    HttpContext.Session.SetString("JWToken", token);
                    HttpContext.Session.SetString("username", user.Username);
                }
                return Redirect("~/Employee/Index");
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Employee employee)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    StringContent stringContent = new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json");
                    using (var response = await client.PostAsync("https://localhost:44311/api/UserInfo/register", stringContent))
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        if (result == "Username already exist")
                        {
                            TempData["Message"] = "Username đã tồn tại";
                        }
                        if (result == "Email already exist")
                        {
                            TempData["Message"] = "Email đã tồn tại";
                        }
                        if (result == "PhoneNumber already exist")
                        {
                            TempData["Message"] = "Số điện thoại đã tồn tại";
                        }

                        if (response.IsSuccessStatusCode)
                        {
                            return Redirect("~/Home/Index");
                        }
                    }
                }
            }
            ModelState.AddModelError(string.Empty, "Lỗi thông tin nhập liệu");

            // return Redirect("~/Home/Index");
            return View(employee);
            
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            //Response.Cookies.Delete(".AspNetCore.Session");
            return Redirect("~/Home/Index");
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

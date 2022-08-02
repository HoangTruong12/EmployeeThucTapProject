using EmployeeWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace EmployeeWebApp.Controllers
{
    public class EmployeeController : Controller
    {
        public static string baseUrl = "https://localhost:44311/api/Employee/";
        public async Task<IActionResult> Index()
        {
            var employees = await GetEmployees();
            return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string username)
        {
            var accessToken = HttpContext.Session.GetString("JWToken");
            var url = baseUrl + "search?username=" + username;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<List<Employee>>(jsonStr).ToList();
            return View(res);
        }


        [HttpGet]
        public async Task<List<Employee>> GetEmployees()
        {
            var accessToken = HttpContext.Session.GetString("JWToken");
            var url = baseUrl + "page/";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string jsonStr = await client.GetStringAsync(url);

            var res = JsonConvert.DeserializeObject<List<Employee>>(jsonStr).ToList();

            return res;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var accessToken = HttpContext.Session.GetString("JWToken");
                    string url = baseUrl;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var stringContent = new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(url, stringContent);

                    var result = response.Content.ReadAsStringAsync().Result;

                    if(result == "Username already exist")
                    {
                        TempData["Message"] = "Username đã tồn tại";
                        //return Redirect("~/Employee/Create");
                    }
                    if(result == "Email already exist")
                    {
                        TempData["Message"] = "Email đã tồn tại";
                    }
                    if (result == "PhoneNumber already exist")
                    {
                        TempData["Message"] = "Số điện thoại đã tồn tại";
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "Lỗi thông tin nhập liệu");
            
            return View(employee);

        }


        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accessToken = HttpContext.Session.GetString("JWToken");
            var url = baseUrl + "getId/" +id;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<Employee>(jsonStr);

            if (res == null)
            {
                return NotFound();
            }
            return View(res);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid? id, Employee employee)
        {
            if (ModelState.IsValid)
            {
                if (id != employee.Id)
                {
                    return NotFound();
                }
                using (var client = new HttpClient())
                {
                    var accessToken = HttpContext.Session.GetString("JWToken");
                    string url = baseUrl + id;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var stringContent = new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(url, stringContent);

                    var result = response.Content.ReadAsByteArrayAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }


            ModelState.AddModelError(string.Empty, "Lỗi thông tin nhập liệu");


            return View(employee);

        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accessToken = HttpContext.Session.GetString("JWToken");
            var url = baseUrl + "getId/" + id;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<Employee>(jsonStr);

            if (res == null)
            {
                return NotFound();
            }
            return View(res);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(Guid id)
        {
            var accessToken = HttpContext.Session.GetString("JWToken");
            var url = baseUrl + id;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            await client.DeleteAsync(url);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accessToken = HttpContext.Session.GetString("JWToken");
            var url = baseUrl + "getId/" + id;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<Employee>(jsonStr);

            if (res == null)
            {
                return NotFound();
            }
            return View(res);
        }
    }
}

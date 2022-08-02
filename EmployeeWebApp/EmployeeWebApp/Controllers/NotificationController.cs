using EmployeeWebApp.Hubs;
using EmployeeWebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWebApp.Controllers
{
    public class NotificationController : Controller
    {
        public static string baseUrl = "https://localhost:44311/api/Notification/";
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Notification notification)
        {
            using(var client = new HttpClient())
            {
                await _hubContext.Clients.All.SendAsync("sendToUser", notification.Title, notification.Content);
                notification.Sender = HttpContext.Session.GetString("username");
                string recipent = 0.ToString();
                notification.Recipent = recipent;

                var stringContent = new StringContent(JsonConvert.SerializeObject(notification), Encoding.UTF8, "application/json");
                string url = baseUrl;
                HttpResponseMessage res = await client.PostAsync(url, stringContent);

                if (res.IsSuccessStatusCode)
                {
                    Console.Write("thanh cong");
                }
                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
                return RedirectToAction("Index", "Employee");
            }
           
        }
    }
}

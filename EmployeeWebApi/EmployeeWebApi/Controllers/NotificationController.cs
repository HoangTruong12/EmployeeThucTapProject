using EmployeeWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // lay tat ca thong bao
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetAllNotifications()
        {
            var notifications = await _context.Notifications.ToListAsync();
            return Ok(notifications);
        }

        // lay thong bao theo id notification
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Notification>> GetNotificationById(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if(notification == null)
            {
                return NotFound();
            }
            return notification;
        }


        // hien thong bao
        [HttpGet]
        [Route("recipent/{id}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationByRecipent(string recipentId)
        {
            var id = _context.Notifications.Where(x => x.Recipent == recipentId || x.Recipent == 0.ToString()).ToList();
            return id;
        }

        [HttpGet]
        [Route("getUsernameSender/{id}")]
        public async Task<ActionResult<Employee>> GetUsernameSender(string sender)
        {
            if (sender == null)
            {
                return NotFound();
            }
            else
            {
                //lấy thêm username sender
                var username = _context.Employees.FirstOrDefault(x => x.Username == sender);
                if (username == null)
                {
                    return NotFound();
                }
                return Ok(username);
            }

        }


        [HttpPost]
        public async Task<ActionResult<Notification>> AddNotification([FromBody] Notification notification)
        {
            notification.Id = Guid.NewGuid();
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetNotificationById", new { id = notification.Id }, notification);
        }
    }
}

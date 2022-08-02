using System;

namespace EmployeeWebApp.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Sender { get; set; }
        public string Recipent { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeeWebApi.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Sender { get; set; }
        public string Recipent { get; set; }
        public Employee Employee { get; set; }

    }
}

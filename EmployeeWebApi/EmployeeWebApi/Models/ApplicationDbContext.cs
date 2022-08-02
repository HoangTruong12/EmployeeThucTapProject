using Microsoft.EntityFrameworkCore;

namespace EmployeeWebApi.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
    }
}

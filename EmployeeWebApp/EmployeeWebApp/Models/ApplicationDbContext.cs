using Microsoft.EntityFrameworkCore;

namespace EmployeeWebApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Employee> Employees { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EmployeeWebApi.Models
{
    public class EmployeeCsvResult : FileResult
    {
        private readonly IEnumerable<Employee> _employeeData;
        public EmployeeCsvResult(IEnumerable<Employee> employeeData, string fileDownloadName) : base("text/csv")
        {
            _employeeData = employeeData;
            FileDownloadName = fileDownloadName;
        }

        public async override Task ExecuteResultAsync (ActionContext context)
        {
            var response = context.HttpContext.Response;
            context.HttpContext.Response.Headers.Add("Content-Disposition", new[] { "attachment; filename = " + FileDownloadName });

            using (var streamWriter = new StreamWriter(response.Body))
            {
                await streamWriter.WriteLineAsync($"Username, Email, Password");

                foreach (var p in _employeeData)
                {
                    await streamWriter.WriteLineAsync(
                        $"{p.Username}, {p.Email}, {p.Password}"
                    );
                    await streamWriter.FlushAsync();
                }
                await streamWriter.FlushAsync();
            }
            
        }
    }
}

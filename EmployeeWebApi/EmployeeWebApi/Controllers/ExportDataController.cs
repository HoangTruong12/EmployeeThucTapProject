using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EmployeeWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportDataController : ControllerBase
    {
        [HttpGet("[action]")]
        public IActionResult ExportEmployeeData()
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=LAPTOP-08JCD86K\HVT;Initial Catalog=WebApiEmployee;User ID=sa;Password=123"))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("select * from dbo.Employees", connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);

                        string csvData = TransformTableToCsv(ds.Tables[0]);

                        var fileBytes = Encoding.UTF8.GetBytes(csvData);
                        return File(fileBytes, "text/csv", "EmployeeData.csv");
                    }
                }
            }
        }

        private string TransformTableToCsv(DataTable dataTable)
        {
            StringBuilder csvBuilder = new StringBuilder();
            IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>()
                .Select(x => x.ColumnName);
            csvBuilder.AppendLine(string.Join(',', columnNames));
            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray
                    .Select(x => string.Concat("\"", x.ToString().Replace("\"", "\"\""), "\""));
                csvBuilder.AppendLine(string.Join(',', fields));
            }
            return csvBuilder.ToString();
        }
    }
}

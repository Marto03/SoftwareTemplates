using ExpenseExporterApp.Models;
using System.Text;

namespace ExpenseExporterApp.Export
{
    public class CsvExpenseExporter : ExpenseExporterTemplate
    {
        protected override string Serialize(IEnumerable<Expense> expenses, IEnumerable<Employee> employees)
        {
            var employeeDict = employees.ToDictionary(e => e.Id, e => e);
            var sb = new StringBuilder();

            sb.AppendLine("ExpenseId,EmployeeName,Position,Description,Amount,Date");

            foreach (var exp in expenses)
            {
                var emp = employeeDict[exp.EmployeeId];
                sb.AppendLine($"{exp.Id},{Escape(emp.FullName)},{Escape(emp.Position)},{Escape(exp.Description)},{exp.Amount},{exp.Date:yyyy-MM-dd}");
            }

            return sb.ToString();
        }

        private string Escape(string input)
        {
            if (input.Contains(","))
                return $"\"{input}\"";
            return input;
        }
    }
}

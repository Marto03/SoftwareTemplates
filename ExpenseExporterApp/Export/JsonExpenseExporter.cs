using ExpenseExporterApp.Models;
using System.Text.Json;

namespace ExpenseExporterApp.Export
{
    public class JsonExpenseExporter : ExpenseExporterTemplate
    {
        protected override string Serialize(IEnumerable<Expense> expenses, IEnumerable<Employee> employees)
        {
            var employeeDict = employees.ToDictionary(e => e.Id, e => e);

            var list = expenses.Select(exp => new
            {
                expenseId = exp.Id,
                employeeId = exp.EmployeeId,
                employeeName = employeeDict[exp.EmployeeId].FullName,
                position = employeeDict[exp.EmployeeId].Position,
                description = exp.Description,
                amount = exp.Amount,
                date = exp.Date
            });

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            return JsonSerializer.Serialize(list, options);
        }
    }
}

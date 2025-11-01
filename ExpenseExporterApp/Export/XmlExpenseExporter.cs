using ExpenseExporterApp.Models;
using System.Xml.Linq;

namespace ExpenseExporterApp.Export
{
    public class XmlExpenseExporter : ExpenseExporterTemplate
    {
        protected override string Serialize(IEnumerable<Expense> expenses, IEnumerable<Employee> employees)
        {
            var employeeDict = employees.ToDictionary(e => e.Id, e => e);

            var root = new XElement("expenses",
                from exp in expenses
                let emp = employeeDict[exp.EmployeeId]
                select new XElement("expense",
                    new XAttribute("id", exp.Id),
                    new XElement("employeeName", emp.FullName),
                    new XElement("position", emp.Position),
                    new XElement("description", exp.Description),
                    new XElement("amount", exp.Amount),
                    new XElement("date", exp.Date.ToString("yyyy-MM-dd"))
                )
            );

            return root.ToString();
        }
    }
}

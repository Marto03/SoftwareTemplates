using ExpenseExporterApp.Models;
using ExpenseExporterApp.Validation;

namespace ExpenseExporterApp.Export
{
    public abstract class ExpenseExporterTemplate
    {
        public ExportResult Export(
            IEnumerable<Employee> employees,
            IEnumerable<Expense> expenses,
            string outputPath)
        {
            var validExpenses = new List<Expense>();
            var errors = new List<string>();

            // 1. Validation
            foreach (var expense in expenses)
            {
                var employee = FindEmployee(employees, expense.EmployeeId);
                if (employee == null)
                {
                    errors.Add($"No employee with Id={expense.EmployeeId} for expense {expense.Id}.");
                    continue;
                }

                var strategy = ValidationStrategyFactory.CreateFor(employee);
                if (strategy.IsValid(employee, expense, out var error))
                {
                    validExpenses.Add(expense);
                }
                else if (!string.IsNullOrEmpty(error))
                {
                    errors.Add(error!);
                }
            }

            // 2. Serialization
            var serialized = Serialize(validExpenses, employees);

            // 3. Write to file
            File.WriteAllText(outputPath, serialized);

            return new ExportResult
            {
                OutputPath = outputPath,
                Errors = errors
            };
        }

        protected abstract string Serialize(IEnumerable<Expense> expenses, IEnumerable<Employee> employees);

        private Employee? FindEmployee(IEnumerable<Employee> employees, int id)
        {
            foreach (var e in employees)
            {
                if (e.Id == id) return e;
            }
            return null;
        }
    }

    public class ExportResult
    {
        public string OutputPath { get; set; } = "";
        public List<string> Errors { get; set; } = new();
        public bool HasErrors => Errors.Count > 0;
    }
}

using ExpenseExporterApp.Models;
using ExpenseExporterApp.Validation;

namespace ExpenseExporterApp.Export
{
    /// <summary>
    /// Abstraction for all expense exporters.
    /// Implementations (XML/JSON/CSV) focus only on the serialization logic.
    /// </summary>
    public interface IExpenseExporter
    {
        /// <summary>
        /// Exports the supplied expenses for the supplied employees to the given output path.
        /// </summary>
        ExportResult Export(IEnumerable<Employee> employees, IEnumerable<Expense> expenses, string outputPath);
    }

    /// <summary>
    /// TEMPLATE METHOD PATTERN:
    /// ------------------------
    /// This abstract class defines the skeleton of the export algorithm inside <see cref="Export"/>:
    ///   1. Validate expenses (Strategy pattern employed for each employee's rule).
    ///   2. Serialize valid expenses (deferred to subclass via abstract <see cref="Serialize"/> method).
    ///   3. Write serialized output to file.
    /// Only step 2 varies; steps 1 & 3 are invariant and implemented here.
    /// Subclasses: <see cref="XmlExpenseExporter"/>, <see cref="JsonExpenseExporter"/>, <see cref="CsvExpenseExporter"/>
    /// </summary>
    public abstract class ExpenseExporterTemplate : IExpenseExporter
    {
        /// <summary>
        /// Executes the template algorithm. Calls a validation strategy per expense (Strategy pattern),
        /// then invokes the subclass-specific serialization (Template Method variant step).
        /// </summary>
        public ExportResult Export(
            IEnumerable<Employee> employees,
            IEnumerable<Expense> expenses,
            string outputPath)
        {
            var validExpenses = new List<Expense>();
            var errors = new List<string>();

            // 1. VALIDATION (Strategy pattern usage)
            // Each employee has a ValidationMode; we ask ValidationStrategyFactory to give us the proper IValidationStrategy instance.
            foreach (var expense in expenses)
            {
                var employee = FindEmployee(employees, expense.EmployeeId);
                if (employee == null)
                {
                    errors.Add($"No employee with Id={expense.EmployeeId} for expense {expense.Id}.");
                    continue;
                }

                // FACTORY PATTERN -> produces concrete strategy object based on employee's ValidationMode
                var strategy = ValidationStrategyFactory.CreateFor(employee);
                // STRATEGY PATTERN -> polymorphic IsValid call encapsulates rule differences
                if (strategy.IsValid(employee, expense, out var error))
                {
                    validExpenses.Add(expense);
                }
                else if (!string.IsNullOrEmpty(error))
                {
                    errors.Add(error!);
                }
            }

            // 2. SERIALIZATION (Template Method deferred / variant step)
            var serialized = Serialize(validExpenses, employees);

            // 3. WRITE OUTPUT (invariant step)
            File.WriteAllText(outputPath, serialized);

            return new ExportResult
            {
                OutputPath = outputPath,
                Errors = errors
            };
        }

        /// <summary>
        /// Variant step of Template Method. Each subclass provides its own serialization format.
        /// </summary>
        protected abstract string Serialize(IEnumerable<Expense> expenses, IEnumerable<Employee> employees);

        // Helper method (private; not part of the pattern) to locate employee.
        private Employee? FindEmployee(IEnumerable<Employee> employees, int id)
        {
            foreach (var e in employees)
            {
                if (e.Id == id) return e;
            }
            return null;
        }
    }

    /// <summary>
    /// Result of an export operation including any validation errors encountered.
    /// </summary>
    public class ExportResult
    {
        public string OutputPath { get; set; } = "";
        public List<string> Errors { get; set; } = new();
        public bool HasErrors => Errors.Count > 0;
    }
}

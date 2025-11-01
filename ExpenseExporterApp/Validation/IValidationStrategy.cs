using ExpenseExporterApp.Models;

namespace ExpenseExporterApp.Validation
{
    public interface IValidationStrategy
    {
        bool IsValid(Employee employee, Expense expense, out string? error);
    }
}

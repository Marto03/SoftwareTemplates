using ExpenseExporterApp.Models;

namespace ExpenseExporterApp.Validation
{
    public class PercentOfSalaryValidation : IValidationStrategy
    {
        private readonly decimal _percent;

        public PercentOfSalaryValidation(decimal percent)
        {
            _percent = percent;
        }

        public bool IsValid(Employee employee, Expense expense, out string? error)
        {
            var limit = employee.Salary * _percent;
            if (expense.Amount <= limit)
            {
                error = null;
                return true;
            }

            error = $"Expense {expense.Amount:F2} exceeds {_percent:P0} of salary ({limit:F2}) for {employee.FullName}.";
            return false;
        }
    }
}

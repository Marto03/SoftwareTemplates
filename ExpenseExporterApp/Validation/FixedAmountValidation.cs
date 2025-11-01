using ExpenseExporterApp.Models;

namespace ExpenseExporterApp.Validation
{
    public class FixedAmountValidation : IValidationStrategy
    {
        private readonly decimal _maxAmount;

        public FixedAmountValidation(decimal maxAmount)
        {
            _maxAmount = maxAmount;
        }

        public bool IsValid(Employee employee, Expense expense, out string? error)
        {
            if (expense.Amount <= _maxAmount)
            {
                error = null;
                return true;
            }

            error = $"Expense {expense.Amount:F2} exceeds fixed limit {_maxAmount:F2} for {employee.FullName}.";
            return false;
        }
    }
}

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

            error = $"Разходът {expense.Amount:F2} надвишава фиксирания лимит {_maxAmount:F2} за {employee.FullName}.";
            return false;
        }
    }
}

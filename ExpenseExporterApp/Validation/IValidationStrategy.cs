using ExpenseExporterApp.Models;

namespace ExpenseExporterApp.Validation
{
    /// <summary>
    /// STRATEGY PATTERN:
    /// -----------------
    /// Defines a common interface for validating an expense for a given employee.
    /// Concrete strategies encapsulate different validation rules:
    ///   <see cref="FixedAmountValidation"/> - compares against a fixed monetary limit.
    ///   <see cref="PercentOfSalaryValidation"/> - limit = employee salary * percent.
    ///   <see cref="CustomFormulaValidation"/> - parses and evaluates a user formula (AMOUNT <= expression).
    /// Context selection of strategy is performed by <see cref="ValidationStrategyFactory"/>.
    /// </summary>
    public interface IValidationStrategy
    {
        /// <summary>
        /// Returns true if the expense is valid for the employee according to the strategy's rule.
        /// Outputs an error description when invalid.
        /// </summary>
        bool IsValid(Employee employee, Expense expense, out string? error);
    }
}

using ExpenseExporterApp.Models;

namespace ExpenseExporterApp.Validation
{
    /// <summary>
    /// FACTORY PATTERN (for strategies):
    /// ----------------------------------
    /// Produces an appropriate validation strategy object based on an employee's <see cref="ValidationMode"/>.
    /// This encapsulates object creation logic, keeping calling code (e.g. ExporterTemplate, AddExpenseForm) simple.
    /// If a new validation mode is added, extend the switch here.
    /// </summary>
    public static class ValidationStrategyFactory
    {
        /// <summary>
        /// Returns a concrete <see cref="IValidationStrategy"/> suited to the employee's configured validation mode.
        /// </summary>
        public static IValidationStrategy CreateFor(Employee employee)
        {
            return employee.ValidationMode switch
            {
                ValidationMode.FixedAmount => new FixedAmountValidation(employee.MaxAllowedAmount),
                ValidationMode.PercentOfSalary => new PercentOfSalaryValidation(employee.MaxPercentOfSalary),
                ValidationMode.CustomFormula when !string.IsNullOrWhiteSpace(employee.CustomFormula)
                    => new CustomFormulaValidation(employee.CustomFormula!),
                _ => new FixedAmountValidation(0) // fallback that will always fail validation
            };
        }
    }
}

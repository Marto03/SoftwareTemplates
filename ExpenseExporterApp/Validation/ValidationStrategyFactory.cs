using ExpenseExporterApp.Models;

namespace ExpenseExporterApp.Validation
{
    public static class ValidationStrategyFactory
    {
        public static IValidationStrategy CreateFor(Employee employee)
        {
            return employee.ValidationMode switch
            {
                ValidationMode.FixedAmount => new FixedAmountValidation(employee.MaxAllowedAmount),
                ValidationMode.PercentOfSalary => new PercentOfSalaryValidation(employee.MaxPercentOfSalary),
                ValidationMode.CustomFormula when !string.IsNullOrWhiteSpace(employee.CustomFormula)
                    => new CustomFormulaValidation(employee.CustomFormula!),
                _ => new FixedAmountValidation(0) // всичко ще фейлва
            };
        }
    }
}

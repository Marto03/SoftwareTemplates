using ExpenseExporterApp.Models;
using System.Globalization;


namespace ExpenseExporterApp.Validation
{
    /// <summary>
    /// За да не вкарваме скриптове, ще направим много прост парсър, който разбира само такива формули:
    /// AMOUNT <= 0.5 * SALARY
    /// AMOUNT <= 200
    /// интервали не са задължителни
    /// </summary>
    public class CustomFormulaValidation : IValidationStrategy
    {
        private readonly string _formula;

        public CustomFormulaValidation(string formula)
        {
            _formula = formula;
        }

        public bool IsValid(Employee employee, Expense expense, out string? error)
        {
            // поддържаме само "AMOUNT <= <expr>"
            // където <expr> може да е:
            //  - число: 200
            //  - коефициент * SALARY: 0.4 * SALARY
            //  - SALARY * коефициент: SALARY * 0.4

            var normalized = _formula.Replace(" ", "", StringComparison.OrdinalIgnoreCase)
                                     .ToUpperInvariant();

            const string left = "AMOUNT<=";
            if (!normalized.StartsWith(left))
            {
                error = $"Неподдържана формула: {_formula}";
                return false;
            }

            var rightExpr = normalized.Substring(left.Length);

            decimal limit;

            if (rightExpr.Contains("SALARY"))
            {
                // примерно 0.4*SALARY или SALARY*0.4
                if (rightExpr.StartsWith("SALARY*"))
                {
                    var coefStr = rightExpr.Substring("SALARY*".Length);
                    if (!decimal.TryParse(coefStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var coef))
                    {
                        error = $"Неподдържана формула: {_formula}";
                        return false;
                    }
                    limit = employee.Salary * coef;
                }
                else if (rightExpr.EndsWith("*SALARY"))
                {
                    var coefStr = rightExpr.Substring(0, rightExpr.Length - "*SALARY".Length);
                    if (!decimal.TryParse(coefStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var coef))
                    {
                        error = $"Неподдържана формула: {_formula}";
                        return false;
                    }
                    limit = employee.Salary * coef;
                }
                else
                {
                    error = $"Неподдържана формула: {_formula}";
                    return false;
                }
            }
            else
            {
                // само число
                if (!decimal.TryParse(rightExpr, NumberStyles.Any, CultureInfo.InvariantCulture, out limit))
                {
                    error = $"Неподдържана формула: {_formula}";
                    return false;
                }
            }

            if (expense.Amount <= limit)
            {
                error = null;
                return true;
            }

            error = $"Разходът {expense.Amount:F2} надвишава формулата \"{_formula}\" (лимит {limit:F2}) за {employee.FullName}.";
            return false;
        }
    }
}

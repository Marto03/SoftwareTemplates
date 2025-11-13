using ExpenseExporterApp.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ExpenseExporterApp.Validation
{
    /// <summary>
    /// STRATEGY IMPLEMENTATION: Custom formula validation.
    /// Parses user-defined constraint formulas enforcing: AMOUNT = expression.
    /// Supported forms (case-insensitive, spaces ignored):
    ///   AMOUNT = 200
    ///   AMOUNT = 0.5 * SALARY / SALARY * 0.5
    ///   AMOUNT = 50% * SALARY / SALARY * 50%
    ///   AMOUNT = SALARY
    /// Percent values (e.g. 40%) converted to decimal (0.4).
    /// </summary>
    public class CustomFormulaValidation : IValidationStrategy
    {
        private readonly string _formula;
        private static readonly Regex PercentRegex = new("^(?<num>\\d+(?:[\\.,]\\d+)?)%$", RegexOptions.Compiled);

        public CustomFormulaValidation(string formula)
        {
            _formula = formula;
        }

        public bool IsValid(Employee employee, Expense expense, out string? error)
        {
            var raw = _formula.Trim();
            if (string.IsNullOrWhiteSpace(raw))
            {
                error = "Unsupported formula: (empty)";
                return false;
            }

            // Expect pattern AMOUNT <= <expression>
            var idx = raw.IndexOf("<=", StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
            {
                error = $"Unsupported formula: {_formula}";
                return false;
            }
            var left = raw.Substring(0, idx).Trim();
            var rightOriginal = raw.Substring(idx + 2).Trim();
            if (!left.Equals("AMOUNT", StringComparison.OrdinalIgnoreCase))
            {
                error = $"Unsupported formula: {_formula}. Left side must be AMOUNT.";
                return false;
            }

            // Normalize multiplication spacing.
            var right = Regex.Replace(rightOriginal, @"\s*\*\s*", "*").Trim();

            decimal limit;
            if (right.Equals("SALARY", StringComparison.OrdinalIgnoreCase))
            {
                limit = employee.Salary; // limit equals full salary
            }
            else if (right.Contains('*'))
            {
                var parts = right.Split('*');
                if (parts.Length != 2)
                {
                    error = $"Unsupported formula: {_formula}";
                    return false;
                }
                var pA = parts[0].Trim();
                var pB = parts[1].Trim();
                bool aSalary = pA.Equals("SALARY", StringComparison.OrdinalIgnoreCase);
                bool bSalary = pB.Equals("SALARY", StringComparison.OrdinalIgnoreCase);
                if (aSalary == bSalary) // must be exactly one SALARY operand
                {
                    error = $"Unsupported formula: {_formula}. Provide one SALARY operand and one numeric/percent operand.";
                    return false;
                }
                var coefStr = aSalary ? pB : pA;
                if (!TryParseNumberOrPercent(coefStr, out var coef))
                {
                    error = $"Unsupported formula: {_formula}. Could not parse coefficient '{coefStr}'.";
                    return false;
                }
                limit = employee.Salary * coef;
            }
            else
            {
                if (!TryParseNumberOrPercent(right, out limit))
                {
                    error = $"Unsupported formula: {_formula}. Could not parse numeric value '{right}'.";
                    return false;
                }
            }

            if (expense.Amount <= limit)
            {
                error = null;
                return true;
            }

            error = $"Expense {expense.Amount:F2} exceeds formula \"{_formula}\" (limit {limit:F2}) for {employee.FullName}.";
            return false;
        }

        private static bool TryParseNumberOrPercent(string input, out decimal value)
        {
            input = input.Trim();
            var m = PercentRegex.Match(input);
            if (m.Success)
            {
                var numPart = m.Groups["num"].Value.Replace(',', '.');
                if (decimal.TryParse(numPart, NumberStyles.Any, CultureInfo.InvariantCulture, out var pct))
                {
                    value = pct / 100m; // convert percent to fraction
                    return true;
                }
            }
            var normalized = input.Replace(',', '.');
            if (decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var num))
            {
                value = num;
                return true;
            }
            value = 0;
            return false;
        }
    }
}

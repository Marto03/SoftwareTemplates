namespace ExpenseExporterApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Position { get; set; } = "";
        public decimal Salary { get; set; }

        // как да валидираме разходите му
        public ValidationMode ValidationMode { get; set; }

        // за Fixed
        public decimal MaxAllowedAmount { get; set; }

        // за PercentOfSalary (примерно 0.3m == 30%)
        public decimal MaxPercentOfSalary { get; set; }

        // за Custom
        // пример: "AMOUNT <= SALARY * 0.4"
        public string? CustomFormula { get; set; }
    }
}

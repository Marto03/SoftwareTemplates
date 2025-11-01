namespace ExpenseExporterApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Position { get; set; } = "";
        public decimal Salary { get; set; }

        // Expense validation mode
        public ValidationMode ValidationMode { get; set; }

        // For FixedAmount
        public decimal MaxAllowedAmount { get; set; }

        // For PercentOfSalary (e.g. 0.3m == 30%)
        public decimal MaxPercentOfSalary { get; set; }

        // For CustomFormula
        // Example: "AMOUNT <= SALARY * 0.4"
        public string? CustomFormula { get; set; }
    }
}

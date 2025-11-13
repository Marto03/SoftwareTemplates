using System;

namespace ExpenseExporterApp.Export
{
    /// <summary>
    /// FACTORY PATTERN:
    /// ----------------
    /// Centralizes creation of concrete exporter implementations based on a format key.
    /// Client code (e.g. UI) calls ExporterFactory.Create("JSON") instead of directly instantiating classes.
    /// This decouples selection logic from consumers and makes adding new formats easier (extend switch).
    /// </summary>
    public static class ExporterFactory
    {
        /// <summary>
        /// Returns a concrete <see cref="IExpenseExporter"/> matching the requested format.
        /// Supported: XML, JSON, CSV. Unknown values default to JSON.
        /// </summary>
        public static (ExpenseExporterTemplate exporter, string defaultFileName) Create(string format)
        {
            return format.ToUpperInvariant() switch
            {
                "XML" => (new XmlExpenseExporter(), "expenses.xml"),
                "JSON" => (new JsonExpenseExporter(), "expenses.json"),
                "CSV" => (new CsvExpenseExporter(), "expenses.csv"),
                _ => (new JsonExpenseExporter(), "expenses.txt")
            };
        }
    }
}

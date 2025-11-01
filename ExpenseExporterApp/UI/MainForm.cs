using ExpenseExporterApp.Export;
using ExpenseExporterApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.DataFormats;

namespace ExpenseExporterApp.UI
{
    public partial class MainForm : Form
    {
        private readonly BindingList<Employee> _employees = new();
        private readonly BindingList<Expense> _expenses = new();

        public MainForm()
        {
            InitializeComponent();
            dgvEmployees.AutoGenerateColumns = true;
            dgvExpenses.AutoGenerateColumns = true;

            dgvEmployees.DataSource = _employees;
            dgvExpenses.DataSource = _expenses;

            cbFormat.Items.AddRange(new[] { "XML", "JSON", "CSV" });
            cbFormat.SelectedIndex = 0;

            SeedData();
        }

        private void SeedData()
        {
            _employees.Add(new Employee
            {
                Id = 1,
                FullName = "Иван Иванов",
                Position = "Мениджър",
                Salary = 4000,
                ValidationMode = ValidationMode.PercentOfSalary,
                MaxPercentOfSalary = 0.3m
            });

            _employees.Add(new Employee
            {
                Id = 2,
                FullName = "Петър Петров",
                Position = "Разработчик",
                Salary = 2500,
                ValidationMode = ValidationMode.FixedAmount,
                MaxAllowedAmount = 300
            });

            _employees.Add(new Employee
            {
                Id = 3,
                FullName = "Мария Георгиева",
                Position = "Стажант",
                Salary = 1200,
                ValidationMode = ValidationMode.CustomFormula,
                CustomFormula = "AMOUNT <= 0.4 * SALARY"
            });

            _expenses.Add(new Expense
            {
                Id = 1,
                EmployeeId = 1,
                Description = "Командировка Варна",
                Amount = 1000,
                Date = DateTime.Today
            });

            _expenses.Add(new Expense
            {
                Id = 2,
                EmployeeId = 2,
                Description = "Лиценз за софтуер",
                Amount = 250,
                Date = DateTime.Today.AddDays(-1)
            });

            _expenses.Add(new Expense
            {
                Id = 3,
                EmployeeId = 3,
                Description = "Транспорт",
                Amount = 700, // това сигурно ще фейлне
                Date = DateTime.Today.AddDays(-2)
            });
        }

        private void btnAddExpense_Click(object sender, EventArgs e)
        {
            var frm = new AddExpenseForm(_employees);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                // връща готов Expense
                _expenses.Add(frm.CreatedExpense);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExpenseExporterTemplate exporter = cbFormat.SelectedItem switch
            {
                "XML" => new XmlExpenseExporter(),
                "JSON" => new JsonExpenseExporter(),
                "CSV" => new CsvExpenseExporter(),
                _ => new JsonExpenseExporter()
            };

            using var sfd = new SaveFileDialog();
            sfd.Filter = "All files|*.*";
            sfd.FileName = cbFormat.SelectedItem switch
            {
                "XML" => "expenses.xml",
                "JSON" => "expenses.json",
                "CSV" => "expenses.csv",
                _ => "expenses.txt"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var result = exporter.Export(_employees, _expenses, sfd.FileName);

                var message = $"Експортът е записан в:\n{sfd.FileName}";
                if (result.HasErrors)
                {
                    message += "\n\nНевалидни разходи:\n" + string.Join("\n", result.Errors);
                }

                MessageBox.Show(message, "Експорт", MessageBoxButtons.OK,
                    result.HasErrors ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
        }
    }
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvEmployees;
        private System.Windows.Forms.DataGridView dgvExpenses;
        private System.Windows.Forms.Button btnAddExpense;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ComboBox cbFormat;
        private System.Windows.Forms.Label lblEmployees;
        private System.Windows.Forms.Label lblExpenses;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvEmployees = new System.Windows.Forms.DataGridView();
            this.dgvExpenses = new System.Windows.Forms.DataGridView();
            this.btnAddExpense = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.cbFormat = new System.Windows.Forms.ComboBox();
            this.lblEmployees = new System.Windows.Forms.Label();
            this.lblExpenses = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpenses)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvEmployees
            // 
            this.dgvEmployees.Location = new System.Drawing.Point(12, 29);
            this.dgvEmployees.Name = "dgvEmployees";
            this.dgvEmployees.Size = new System.Drawing.Size(380, 200);
            this.dgvEmployees.TabIndex = 0;
            // 
            // dgvExpenses
            // 
            this.dgvExpenses.Location = new System.Drawing.Point(12, 258);
            this.dgvExpenses.Name = "dgvExpenses";
            this.dgvExpenses.Size = new System.Drawing.Size(560, 200);
            this.dgvExpenses.TabIndex = 1;
            // 
            // btnAddExpense
            // 
            this.btnAddExpense.Location = new System.Drawing.Point(590, 258);
            this.btnAddExpense.Name = "btnAddExpense";
            this.btnAddExpense.Size = new System.Drawing.Size(139, 34);
            this.btnAddExpense.TabIndex = 2;
            this.btnAddExpense.Text = "Добави разход";
            this.btnAddExpense.UseVisualStyleBackColor = true;
            this.btnAddExpense.Click += new System.EventHandler(this.btnAddExpense_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(590, 424);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(139, 34);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "Експорт";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // cbFormat
            // 
            this.cbFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFormat.Location = new System.Drawing.Point(590, 382);
            this.cbFormat.Name = "cbFormat";
            this.cbFormat.Size = new System.Drawing.Size(139, 23);
            this.cbFormat.TabIndex = 4;
            // 
            // lblEmployees
            // 
            this.lblEmployees.AutoSize = true;
            this.lblEmployees.Location = new System.Drawing.Point(12, 9);
            this.lblEmployees.Name = "lblEmployees";
            this.lblEmployees.Size = new System.Drawing.Size(74, 15);
            this.lblEmployees.TabIndex = 5;
            this.lblEmployees.Text = "Служители:";
            // 
            // lblExpenses
            // 
            this.lblExpenses.AutoSize = true;
            this.lblExpenses.Location = new System.Drawing.Point(12, 240);
            this.lblExpenses.Name = "lblExpenses";
            this.lblExpenses.Size = new System.Drawing.Size(55, 15);
            this.lblExpenses.TabIndex = 6;
            this.lblExpenses.Text = "Разходи:";
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(741, 470);
            this.Controls.Add(this.lblExpenses);
            this.Controls.Add(this.lblEmployees);
            this.Controls.Add(this.cbFormat);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnAddExpense);
            this.Controls.Add(this.dgvExpenses);
            this.Controls.Add(this.dgvEmployees);
            this.Name = "MainForm";
            this.Text = "Expense Exporter";
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpenses)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

using ExpenseExporterApp.Export;
using ExpenseExporterApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
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

            // Data binding
            dgvEmployees.AutoGenerateColumns = true;
            dgvExpenses.AutoGenerateColumns = true;
            dgvEmployees.DataSource = _employees;
            dgvExpenses.DataSource = _expenses;

            // Export formats
            cbFormat.Items.AddRange(new[] { "XML", "JSON", "CSV" });
            cbFormat.SelectedIndex = 0;

            // Styling improvements
            ApplyStyling();

            SeedData();

            // Enable editing on double-click
            dgvExpenses.CellDoubleClick += DgvExpenses_CellDoubleClick;
        }

        private void DgvExpenses_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var expense = _expenses[e.RowIndex];
            using var frm = new AddExpenseForm(_employees, expense);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                dgvExpenses.Refresh();
            }
        }

        private void ApplyStyling()
        {
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            BackColor = Color.FromArgb(245, 247, 250);

            foreach (var grid in new[] { dgvEmployees, dgvExpenses })
            {
                grid.ReadOnly = true;
                grid.AllowUserToAddRows = false;
                grid.AllowUserToDeleteRows = false;
                grid.AllowUserToResizeRows = false;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.MultiSelect = false;
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                grid.BackgroundColor = Color.White;
                grid.BorderStyle = BorderStyle.None;
                grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                grid.RowHeadersVisible = false;
                grid.EnableHeadersVisualStyles = false;
                grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 235, 240);
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F);
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250);
                grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 225, 240);
                grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            }

            foreach (var btn in new[] { btnAddExpense, btnAddEmployee, btnExport, btnEditEmployee })
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Height = 38;
            }

            btnAddExpense.BackColor = Color.FromArgb(52, 152, 219);
            btnAddExpense.ForeColor = Color.White;
            btnAddEmployee.BackColor = Color.FromArgb(155, 89, 182);
            btnAddEmployee.ForeColor = Color.White;
            btnExport.BackColor = Color.FromArgb(39, 174, 96);
            btnExport.ForeColor = Color.White;
            btnEditEmployee.BackColor = Color.FromArgb(241, 196, 15);
            btnEditEmployee.ForeColor = Color.White;

            cbFormat.DropDownStyle = ComboBoxStyle.DropDownList;

            lblEmployees.Font = new Font("Segoe UI Semibold", 11F);
            lblExpenses.Font = new Font("Segoe UI Semibold", 11F);
        }

        private void SeedData()
        {
            _employees.Add(new Employee
            {
                Id = 1,
                FullName = "Ivan Ivanov",
                Position = "Manager",
                Salary = 4000,
                ValidationMode = ValidationMode.PercentOfSalary,
                MaxPercentOfSalary = 0.3m
            });

            _employees.Add(new Employee
            {
                Id = 2,
                FullName = "Petar Petrov",
                Position = "Developer",
                Salary = 2500,
                ValidationMode = ValidationMode.FixedAmount,
                MaxAllowedAmount = 300
            });

            _employees.Add(new Employee
            {
                Id = 3,
                FullName = "Maria Georgieva",
                Position = "Intern",
                Salary = 1200,
                ValidationMode = ValidationMode.CustomFormula,
                CustomFormula = "AMOUNT <= 0.4 * SALARY"
            });

            _expenses.Add(new Expense
            {
                Id = 1,
                EmployeeId = 1,
                Description = "Business trip Varna",
                Amount = 1000,
                Date = DateTime.Today
            });

            _expenses.Add(new Expense
            {
                Id = 2,
                EmployeeId = 2,
                Description = "Software license",
                Amount = 250,
                Date = DateTime.Today.AddDays(-1)
            });

            _expenses.Add(new Expense
            {
                Id = 3,
                EmployeeId = 3,
                Description = "Transport",
                Amount = 700,
                Date = DateTime.Today.AddDays(-2)
            });
        }

        private void btnAddExpense_Click(object sender, EventArgs e)
        {
            using var frm = new AddExpenseForm(_employees);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                _expenses.Add(frm.CreatedExpense);
            }
        }

        private void btnAddEmployee_Click(object sender, EventArgs e)
        {
            using var frm = new AddEmployeeForm();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                frm.CreatedEmployee.Id = _employees.Count == 0 ? 1 : _employees.Max(emp => emp.Id) + 1;
                _employees.Add(frm.CreatedEmployee);
            }
        }

        private void btnEditEmployee_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.CurrentRow == null) return;
            if (dgvEmployees.CurrentRow.DataBoundItem is not Employee emp) return;
            using var frm = new AddEmployeeForm(emp);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                dgvEmployees.Refresh();
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // Use factory instead of switch for exporter creation and for defaultFileName (Factory pattern)
            var (exporter, defaultFileName) = ExporterFactory.Create((string)cbFormat.SelectedItem!);

            using var sfd = new SaveFileDialog
            {
                Filter = "All files|*.*",
                FileName = defaultFileName
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var result = exporter.Export(_employees, _expenses, sfd.FileName);

                var message = $"Export saved to:\n{sfd.FileName}";
                if (result.HasErrors)
                {
                    message += "\n\nInvalid expenses:\n" + string.Join("\n", result.Errors);
                }

                MessageBox.Show(message, "Export", MessageBoxButtons.OK,
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
        private System.Windows.Forms.Button btnAddEmployee;
        private System.Windows.Forms.Button btnEditEmployee;
        private System.Windows.Forms.ComboBox cbFormat;
        private System.Windows.Forms.Label lblEmployees;
        private System.Windows.Forms.Label lblExpenses;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.FlowLayoutPanel panelActions;
        private System.Windows.Forms.Label lblFormat;

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
            this.btnAddEmployee = new System.Windows.Forms.Button();
            this.btnEditEmployee = new System.Windows.Forms.Button();
            this.cbFormat = new System.Windows.Forms.ComboBox();
            this.lblEmployees = new System.Windows.Forms.Label();
            this.lblExpenses = new System.Windows.Forms.Label();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.panelActions = new System.Windows.Forms.FlowLayoutPanel();
            this.lblFormat = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpenses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitMain.SplitterDistance = 230;
            this.splitMain.Panel1.Padding = new Padding(10, 10, 10, 5);
            this.splitMain.Panel2.Padding = new Padding(10, 5, 10, 10);
            // 
            // Panel1 (Employees)
            // 
            this.lblEmployees.Dock = DockStyle.Top;
            this.lblEmployees.Text = "Employees";
            this.lblEmployees.Font = new Font("Segoe UI Semibold", 11F);
            this.lblEmployees.Height = 24;
            this.dgvEmployees.Dock = DockStyle.Fill;
            // 
            // Panel2 (Expenses)
            // 
            this.lblExpenses.Dock = DockStyle.Top;
            this.lblExpenses.Text = "Expenses";
            this.lblExpenses.Font = new Font("Segoe UI Semibold", 11F);
            this.lblExpenses.Height = 24;
            this.dgvExpenses.Dock = DockStyle.Fill;
            // Add controls to split panels
            this.splitMain.Panel1.Controls.Add(this.dgvEmployees);
            this.splitMain.Panel1.Controls.Add(this.lblEmployees);
            this.splitMain.Panel2.Controls.Add(this.dgvExpenses);
            this.splitMain.Panel2.Controls.Add(this.lblExpenses);
            // 
            // panelActions
            // 
            this.panelActions.Dock = DockStyle.Right;
            this.panelActions.Width = 200;
            this.panelActions.Padding = new Padding(12);
            this.panelActions.FlowDirection = FlowDirection.TopDown;
            this.panelActions.WrapContents = false;
            this.panelActions.BackColor = Color.FromArgb(235, 239, 243);
            this.panelActions.Controls.Add(this.lblFormat);
            this.panelActions.Controls.Add(this.cbFormat);
            this.panelActions.Controls.Add(this.btnAddExpense);
            this.panelActions.Controls.Add(this.btnAddEmployee);
            this.panelActions.Controls.Add(this.btnEditEmployee);
            this.panelActions.Controls.Add(this.btnExport);
            // 
            // lblFormat
            // 
            this.lblFormat.AutoSize = true;
            this.lblFormat.Text = "Export format:";
            this.lblFormat.Margin = new Padding(3, 3, 3, 0);
            // 
            // cbFormat
            // 
            this.cbFormat.Width = 160;
            this.cbFormat.Margin = new Padding(3, 3, 3, 12);
            // 
            // btnAddExpense
            // 
            this.btnAddExpense.Text = "Add expense";
            this.btnAddExpense.Width = 160;
            this.btnAddExpense.Margin = new Padding(3, 0, 3, 10);
            this.btnAddExpense.Click += new EventHandler(this.btnAddExpense_Click);
            // 
            // btnAddEmployee
            // 
            this.btnAddEmployee.Text = "Add employee";
            this.btnAddEmployee.Width = 160;
            this.btnAddEmployee.Margin = new Padding(3, 0, 3, 10);
            this.btnAddEmployee.Click += new EventHandler(this.btnAddEmployee_Click);
            // 
            // btnEditEmployee
            // 
            this.btnEditEmployee.Text = "Edit employee";
            this.btnEditEmployee.Width = 160;
            this.btnEditEmployee.Margin = new Padding(3, 0, 3, 10);
            this.btnEditEmployee.Click += new EventHandler(this.btnEditEmployee_Click);
            // 
            // btnExport
            // 
            this.btnExport.Text = "Export";
            this.btnExport.Width = 160;
            this.btnExport.Margin = new Padding(3, 0, 3, 10);
            this.btnExport.Click += new EventHandler(this.btnExport_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(950, 600);
            this.MinimumSize = new System.Drawing.Size(750, 500);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.panelActions);
            this.Name = "MainForm";
            this.Text = "Expense Exporter";
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmployees)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpenses)).EndInit();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.panelActions.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}

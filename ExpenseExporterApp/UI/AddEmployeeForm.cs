using ExpenseExporterApp.Models;
using System;
using System.Windows.Forms;
using System.Drawing;

namespace ExpenseExporterApp.UI
{
    public partial class AddEmployeeForm : Form
    {
        public Employee CreatedEmployee { get; private set; } = new Employee();
        private readonly Employee? _editingEmployee;
        private const string FormulaExample = "AMOUNT <= 0.4 * SALARY"; // placeholder example

        public AddEmployeeForm(Employee? editingEmployee = null)
        {
            _editingEmployee = editingEmployee;
            if (_editingEmployee != null)
            {
                CreatedEmployee = _editingEmployee; // editing existing
            }
            InitializeComponent();
            cbValidationMode.Items.AddRange(new object[] { ValidationMode.FixedAmount, ValidationMode.PercentOfSalary, ValidationMode.CustomFormula });
            cbValidationMode.SelectedIndex = 0;
            UpdateModeFields();
            LoadEditingDataIfNeeded();
        }

        private void LoadEditingDataIfNeeded()
        {
            if (_editingEmployee == null) return;
            Text = "Edit Employee";
            btnOk.Text = "Save";
            txtFullName.Text = _editingEmployee.FullName;
            txtPosition.Text = _editingEmployee.Position;
            txtSalary.Text = _editingEmployee.Salary.ToString();
            cbValidationMode.SelectedItem = _editingEmployee.ValidationMode;
            UpdateModeFields();
            if (_editingEmployee.ValidationMode == ValidationMode.FixedAmount)
            {
                txtFixed.Text = _editingEmployee.MaxAllowedAmount.ToString();
            }
            else if (_editingEmployee.ValidationMode == ValidationMode.PercentOfSalary)
            {
                txtPercent.Text = _editingEmployee.MaxPercentOfSalary.ToString();
            }
            else if (_editingEmployee.ValidationMode == ValidationMode.CustomFormula)
            {
                txtFormula.Text = _editingEmployee.CustomFormula ?? string.Empty;
            }
        }

        private void cbValidationMode_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateModeFields();
        }

        private void UpdateModeFields()
        {
            var mode = (ValidationMode)cbValidationMode.SelectedItem!;
            pnlFixed.Visible = mode == ValidationMode.FixedAmount;
            pnlPercent.Visible = mode == ValidationMode.PercentOfSalary;
            pnlFormula.Visible = mode == ValidationMode.CustomFormula;
            if (mode == ValidationMode.CustomFormula && string.IsNullOrWhiteSpace(txtFormula.Text) && _editingEmployee == null)
            {
                // show example placeholder
                txtFormula.PlaceholderText = FormulaExample;
            }
        }

        private void btnOk_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Full name is required.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPosition.Text))
            {
                MessageBox.Show("Position is required.");
                return;
            }
            if (!decimal.TryParse(txtSalary.Text, out var salary) || salary <= 0)
            {
                MessageBox.Show("Salary must be a positive number.");
                return;
            }
            var mode = (ValidationMode)cbValidationMode.SelectedItem!;
            decimal fixedAmount = 0m;
            decimal percent = 0m;
            string? formula = null;
            if (mode == ValidationMode.FixedAmount)
            {
                if (!decimal.TryParse(txtFixed.Text, out fixedAmount) || fixedAmount < 0)
                {
                    MessageBox.Show("Fixed amount must be a non-negative number.");
                    return;
                }
            }
            else if (mode == ValidationMode.PercentOfSalary)
            {
                if (!decimal.TryParse(txtPercent.Text, out percent) || percent <= 0 || percent > 1)
                {
                    MessageBox.Show("Percent must be between 0 and 1 (e.g. 0.3 for 30%).");
                    return;
                }
            }
            else if (mode == ValidationMode.CustomFormula)
            {
                formula = txtFormula.Text.Trim();
                if (string.IsNullOrWhiteSpace(formula))
                {
                    MessageBox.Show("Formula is required. Example: " + FormulaExample);
                    return;
                }
            }

            if (_editingEmployee != null)
            {
                // mutate existing
                _editingEmployee.FullName = txtFullName.Text.Trim();
                _editingEmployee.Position = txtPosition.Text.Trim();
                _editingEmployee.Salary = salary;
                _editingEmployee.ValidationMode = mode;
                _editingEmployee.MaxAllowedAmount = fixedAmount;
                _editingEmployee.MaxPercentOfSalary = percent;
                _editingEmployee.CustomFormula = formula;
                CreatedEmployee = _editingEmployee;
            }
            else
            {
                CreatedEmployee = new Employee
                {
                    FullName = txtFullName.Text.Trim(),
                    Position = txtPosition.Text.Trim(),
                    Salary = salary,
                    ValidationMode = mode,
                    MaxAllowedAmount = fixedAmount,
                    MaxPercentOfSalary = percent,
                    CustomFormula = formula
                };
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }

    partial class AddEmployeeForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtFullName;
        private TextBox txtPosition;
        private TextBox txtSalary;
        private ComboBox cbValidationMode;
        private Panel pnlFixed;
        private Panel pnlPercent;
        private Panel pnlFormula;
        private TextBox txtFixed;
        private TextBox txtPercent;
        private TextBox txtFormula;
        private Button btnOk;
        private Button btnCancel;
        private Label lblFullName;
        private Label lblPosition;
        private Label lblSalary;
        private Label lblMode;
        private Label lblFixed;
        private Label lblPercent;
        private Label lblFormula;
        private Label lblFormulaHint; // hint label

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtFullName = new TextBox();
            this.txtPosition = new TextBox();
            this.txtSalary = new TextBox();
            this.cbValidationMode = new ComboBox();
            this.pnlFixed = new Panel();
            this.pnlPercent = new Panel();
            this.pnlFormula = new Panel();
            this.txtFixed = new TextBox();
            this.txtPercent = new TextBox();
            this.txtFormula = new TextBox();
            this.btnOk = new Button();
            this.btnCancel = new Button();
            this.lblFullName = new Label();
            this.lblPosition = new Label();
            this.lblSalary = new Label();
            this.lblMode = new Label();
            this.lblFixed = new Label();
            this.lblPercent = new Label();
            this.lblFormula = new Label();
            this.lblFormulaHint = new Label();
            this.SuspendLayout();
            // Labels
            this.lblFullName.Text = "Full Name:"; this.lblFullName.Location = new Point(12, 15); this.lblFullName.AutoSize = true;
            this.lblPosition.Text = "Position:"; this.lblPosition.Location = new Point(12, 53); this.lblPosition.AutoSize = true;
            this.lblSalary.Text = "Salary:"; this.lblSalary.Location = new Point(12, 91); this.lblSalary.AutoSize = true;
            this.lblMode.Text = "Validation Mode:"; this.lblMode.Location = new Point(12, 129); this.lblMode.AutoSize = true;
            // TextBoxes main
            this.txtFullName.Location = new Point(130, 12); this.txtFullName.Width = 240;
            this.txtPosition.Location = new Point(130, 50); this.txtPosition.Width = 240;
            this.txtSalary.Location = new Point(130, 88); this.txtSalary.Width = 120;
            // ComboBox mode
            this.cbValidationMode.Location = new Point(130, 126); this.cbValidationMode.Width = 180; this.cbValidationMode.DropDownStyle = ComboBoxStyle.DropDownList; this.cbValidationMode.SelectedIndexChanged += cbValidationMode_SelectedIndexChanged;
            // Panels
            this.pnlFixed.Location = new Point(12, 165); this.pnlFixed.Size = new Size(360, 40);
            this.pnlPercent.Location = new Point(12, 165); this.pnlPercent.Size = new Size(360, 40);
            this.pnlFormula.Location = new Point(12, 165); this.pnlFormula.Size = new Size(360, 85);
            // Fixed panel controls
            this.lblFixed.Text = "Max Amount:"; this.lblFixed.Location = new Point(0, 12); this.lblFixed.AutoSize = true; this.txtFixed.Location = new Point(130, 9); this.txtFixed.Width = 120; this.pnlFixed.Controls.Add(this.lblFixed); this.pnlFixed.Controls.Add(this.txtFixed);
            // Percent panel controls
            this.lblPercent.Text = "Percent (0-1):"; this.lblPercent.Location = new Point(0, 12); this.lblPercent.AutoSize = true; this.txtPercent.Location = new Point(130, 9); this.txtPercent.Width = 120; this.pnlPercent.Controls.Add(this.lblPercent); this.pnlPercent.Controls.Add(this.txtPercent);
            // Formula panel controls
            this.lblFormula.Text = "Formula:"; this.lblFormula.Location = new Point(0, 12); this.lblFormula.AutoSize = true; this.txtFormula.Location = new Point(130, 9); this.txtFormula.Width = 210; this.lblFormulaHint.Text = "Example: " + FormulaExample; this.lblFormulaHint.Location = new Point(130, 40); this.lblFormulaHint.AutoSize = true; this.lblFormulaHint.ForeColor = Color.DimGray; this.pnlFormula.Controls.Add(this.lblFormula); this.pnlFormula.Controls.Add(this.txtFormula); this.pnlFormula.Controls.Add(this.lblFormulaHint);
            // Buttons
            this.btnOk.Text = "Add"; this.btnOk.Location = new Point(214, 270); this.btnOk.Size = new Size(75, 30); this.btnOk.Click += btnOk_Click;
            this.btnCancel.Text = "Cancel"; this.btnCancel.Location = new Point(295, 270); this.btnCancel.Size = new Size(75, 30); this.btnCancel.Click += btnCancel_Click;
            // Form
            this.ClientSize = new Size(384, 321);
            this.Controls.Add(this.lblFullName);
            this.Controls.Add(this.lblPosition);
            this.Controls.Add(this.lblSalary);
            this.Controls.Add(this.lblMode);
            this.Controls.Add(this.txtFullName);
            this.Controls.Add(this.txtPosition);
            this.Controls.Add(this.txtSalary);
            this.Controls.Add(this.cbValidationMode);
            this.Controls.Add(this.pnlFixed);
            this.Controls.Add(this.pnlPercent);
            this.Controls.Add(this.pnlFormula);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Add Employee";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

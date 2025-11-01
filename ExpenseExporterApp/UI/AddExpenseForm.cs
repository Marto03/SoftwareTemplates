using ExpenseExporterApp.Models;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseExporterApp.UI
{
    public partial class AddExpenseForm : Form
    {
        private readonly IList<Employee> _employees;
        public Expense CreatedExpense { get; private set; } = new Expense();

        public AddExpenseForm(IList<Employee> employees)
        {
            _employees = employees;
            InitializeComponent();
            cbEmployee.DataSource = _employees;
            cbEmployee.DisplayMember = "FullName";
            cbEmployee.ValueMember = "Id";
            dtpDate.Value = DateTime.Today;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (cbEmployee.SelectedItem is not Employee emp)
            {
                MessageBox.Show("Избери служител.");
                return;
            }

            if (!decimal.TryParse(txtAmount.Text, out var amount))
            {
                MessageBox.Show("Сумата е невалидна.");
                return;
            }

            CreatedExpense = new Expense
            {
                Id = new Random().Next(1000, 9999),
                EmployeeId = emp.Id,
                Description = txtDescription.Text,
                Amount = amount,
                Date = dtpDate.Value.Date
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
    partial class AddExpenseForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox cbEmployee;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label lblEmployee;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.Label lblDate;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.cbEmployee = new System.Windows.Forms.ComboBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.btnOk = new System.Windows.Forms.Button();
            this.lblEmployee = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblAmount = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbEmployee
            // 
            this.cbEmployee.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEmployee.Location = new System.Drawing.Point(118, 12);
            this.cbEmployee.Name = "cbEmployee";
            this.cbEmployee.Size = new System.Drawing.Size(242, 23);
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(118, 50);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(242, 23);
            // 
            // txtAmount
            // 
            this.txtAmount.Location = new System.Drawing.Point(118, 88);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(100, 23);
            // 
            // dtpDate
            // 
            this.dtpDate.Location = new System.Drawing.Point(118, 126);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(200, 23);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(285, 170);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 27);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "Добави";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // labels
            // 
            this.lblEmployee.AutoSize = true;
            this.lblEmployee.Location = new System.Drawing.Point(12, 15);
            this.lblEmployee.Text = "Служител:";
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(12, 53);
            this.lblDescription.Text = "Описание:";
            this.lblAmount.AutoSize = true;
            this.lblAmount.Location = new System.Drawing.Point(12, 91);
            this.lblAmount.Text = "Сума:";
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(12, 132);
            this.lblDate.Text = "Дата:";
            // 
            // AddExpenseForm
            // 
            this.ClientSize = new System.Drawing.Size(378, 209);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.lblAmount);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblEmployee);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.txtAmount);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.cbEmployee);
            this.Name = "AddExpenseForm";
            this.Text = "Нов разход";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

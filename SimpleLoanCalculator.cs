using System;
using System.Windows.Forms;
using Microsoft.VisualBasic;
namespace LoanCalculator
{
    public partial class frmLoanWorksheet : Form
    {
        public frmLoanWorksheet()
        {
            InitializeComponent();
            btnInterest.Enabled = false;
            cbxDueAtMonthEnd.Checked = false;
            refreshControls();
        }
        private void refreshControls()
        {
            txtTradeIn.Text = Loan.TradeIn != 0 ? Loan.TradeIn.ToString() : "";
            txtCompensatingBalance.Text = Loan.CompensatingBalance != 0 ? Loan.CompensatingBalance.ToString() : "";
            txtPresentValue.Text = Loan.LoanPresentValue != 0 ? txtPresentValue.Text = Loan.LoanPresentValue.ToString() : "";
            btnSalePrice.Enabled = (Loan.MonthlyPayment > 0 && Loan.InterestRate > 0 && Loan.NumberPayments > 0);
            btnInterest.Enabled = (Loan.LoanPresentValue > 0 && Loan.SalePrice > 0 && Loan.MonthlyPayment > 0 && Loan.NumberPayments > 0);
            btnPayment.Enabled = (Loan.LoanPresentValue > 0 && Loan.SalePrice > 0 && Loan.InterestRate > 0 && Loan.NumberPayments > 0);
            btnNumPayments.Enabled = (Loan.LoanPresentValue > 0 && Loan.SalePrice > 0 && Loan.MonthlyPayment > 0 && Loan.InterestRate > 0);
            Loan.WhenDue = cbxDueAtMonthEnd.Checked ? DueDate.EndOfPeriod : DueDate.BegOfPeriod;
            Refresh();
        }
        private void txtTradeIn_TextChanged(object sender, EventArgs e)
        {
            Loan.TradeIn = validateTextbox(txtTradeIn);
            Loan.CalculateCompensatingBalance();
            Loan.CalculateAmountFinanced();
            Loan.RoundValues();
            refreshControls();
        }
        private void txtAmountStillOwed_TextChanged(object sender, EventArgs e)
        {
            Loan.AmountStillOwed = validateTextbox(txtAmountStillOwed);
            Loan.CalculateCompensatingBalance();
            Loan.CalculateAmountFinanced();
            Loan.RoundValues();
            refreshControls();

        }
        private void txtDownPayment_TextChanged(object sender, EventArgs e)
        {
            Loan.DownPayment = validateTextbox(txtDownPayment);
            Loan.CalculateCompensatingBalance();
            Loan.CalculateAmountFinanced();
            Loan.RoundValues();
            refreshControls();
        }
        private void txtNumberPayments_TextChanged(object sender, EventArgs e)
        {
            Loan.NumberPayments = validateTextbox(txtNumberPayments);
            refreshControls();
        }
        private void txtSalePrice_TextChanged(object sender, EventArgs e)
        {
            Loan.SalePrice = validateTextbox(txtSalePrice);
            Loan.CalculateAmountFinanced();
            Loan.RoundValues();
            refreshControls();
        }
        private void txtMonthlyPayment_TextChanged(object sender, EventArgs e)
        {
            Loan.MonthlyPayment = validateTextbox(txtMonthlyPayment);
            Loan.RoundValues();
            refreshControls();
        }
        private void txtInterestRate_TextChanged(object sender, EventArgs e)
        {
            Loan.InterestRate = validateTextbox(txtInterestRate);
            Loan.RoundValues();
            refreshControls();
        }
        private void btnSalePrice_Click(object sender, EventArgs e)
        {
            Loan.CalculateSalePrice();
            Loan.RoundValues();
            txtSalePrice.Text = Loan.SalePrice.ToString();  // Change in this value triggers a recalculation of Loan.loanPresentValue
            txtPresentValue.Text = Loan.LoanPresentValue.ToString();
            refreshControls();
        }
        private void btnInterest_Click(object sender, EventArgs e)
        {
            Loan.WhenDue = cbxDueAtMonthEnd.Checked ? DueDate.EndOfPeriod : DueDate.BegOfPeriod;
            Loan.CalculateInterestRate();
            txtInterestRate.Text = Loan.InterestRate.ToString();
            refreshControls();
        }
        private void btnPayment_Click(object sender, EventArgs e)
        {
            Loan.CalculateMonthlyPayment();
            txtMonthlyPayment.Text = Loan.MonthlyPayment.ToString();
            refreshControls();
        }
        private void cbxDueAtMonthEnd_CheckedChanged(object sender, EventArgs e)
        {
            Loan.WhenDue = cbxDueAtMonthEnd.Checked ? DueDate.EndOfPeriod : DueDate.BegOfPeriod;
            if (btnInterest.Enabled && Loan.NumberPayments > 0)
            {
                Loan.CalculateInterestRate();
                txtInterestRate.Text = Loan.InterestRate.ToString();
            }
            refreshControls();
        }
        private static double validateTextbox(TextBox textBoxToValidate)
        {
            double result = !double.TryParse(textBoxToValidate.Text, out result) ? (textBoxToValidate.Text.StartsWith("-") ? result : 0) : result;
            //Console.WriteLine(textBoxToValidate.Name + " " + result.ToString());
            textBoxToValidate.Text = result != 0 ? textBoxToValidate.Text = result.ToString() : textBoxToValidate.Text = "";
            return result;
        }
        private void clearForm()
        {
            txtCompensatingBalance.Text = "";
            txtDownPayment.Text = "";
            txtInterestRate.Text = "";
            txtMonthlyPayment.Text = "";
            txtNumberPayments.Text = "";
            txtPresentValue.Text = "";
            txtSalePrice.Text = "";
            txtTradeIn.Text = "";
            txtAmountStillOwed.Text = "";
            cbxDueAtMonthEnd.Checked = false;
        }
        private void btnNumPayments_Click(object sender, EventArgs e)
        {
            Loan.CalculateNumberPayments();

            txtNumberPayments.Text = Loan.NumberPayments.ToString();
            refreshControls();
        }
        private void btnDone_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            Loan.ClearValues();
            clearForm();
        }
    }
}

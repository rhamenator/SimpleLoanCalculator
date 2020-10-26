using System;
using static SimpleLoanCalculator.Financial;
//using System.Diagnostics;
using System.Windows.Forms;
//using Microsoft.VisualBasic;
namespace SimpleLoanCalculator
{
    public partial class frmLoanWorksheet : Form
    {
        public frmLoanWorksheet()
        {
            InitializeComponent();
            btnInterest.Enabled = false;
            cbxDueAtMonthEnd.Checked = false;
            RefreshControls();
        }
        private void UpdateLoan()
        {
            Loan.TradeIn = double.TryParse(txtTradeIn.Text, out double result) ? result : 0;
            Loan.AmountStillOwed = double.TryParse(txtAmountStillOwed.Text, out result) ? result : 0;
            Loan.DownPayment = double.TryParse(txtDownPayment.Text, out result) ? result : 0;
            Loan.CalculateCompensatingBalance();
            Loan.SalePrice = double.TryParse(txtSalePrice.Text, out result) ? result : 0;
            Loan.CalculateAmountFinanced();
            Loan.NumberPayments = int.TryParse(txtNumberPayments.Text, out int intResult) ? intResult : 0;
            Loan.MonthlyPayment = double.TryParse(txtMonthlyPayment.Text, out result) ? result : 0;
            Loan.InterestRate = double.TryParse(txtInterestRate.Text, out result) ? result : 0;
            Loan.MonthlyPayment = double.TryParse(txtMonthlyPayment.Text, out result) ? result : 0;
        }
        private void RefreshControls()
        {
            UpdateLoan();
            txtCompensatingBalance.Text = Loan.CompensatingBalance != 0 ? Loan.CompensatingBalance.ToString("N2") : "";
            txtAmountFinanced.Text = Loan.AmountFinanced != 0 ? txtAmountFinanced.Text = Loan.AmountFinanced.ToString("N2") : "";
            btnSalePrice.Enabled = Loan.MonthlyPayment > 0 && Loan.InterestRate > 0 && Loan.NumberPayments > 0;
            btnInterest.Enabled = Loan.AmountFinanced > 0 && Loan.SalePrice > 0 && Loan.MonthlyPayment > 0 && Loan.NumberPayments > 0;
            btnPayment.Enabled = Loan.AmountFinanced > 0 && Loan.SalePrice > 0 && Loan.InterestRate > 0 && Loan.NumberPayments > 0;
            btnNumPayments.Enabled = Loan.AmountFinanced > 0 && Loan.SalePrice > 0 && Loan.MonthlyPayment > 0 && Loan.InterestRate > 0;
            Loan.WhenDue = cbxDueAtMonthEnd.Checked ? DueDate.EndOfPeriod : DueDate.BegOfPeriod;
            Refresh();
        }
        private void LeaveEventDecimal2(object sender, EventArgs e)
        {
            if (double.TryParse(((TextBox)sender).Text.Trim(), out double result))
            {
                ((TextBox)sender).Text = result.ToString("N2");
                RefreshControls();
            }
        }
        private void LeaveEventDecimal4(object sender, EventArgs e)
        {
            if (double.TryParse(((TextBox)sender).Text.Trim(), out double result))
            {
                ((TextBox)sender).Text = result.ToString("N4");
                RefreshControls();
            }
        }
        private void LeaveEventInteger(object sender, EventArgs e)
        {
            if (int.TryParse(((TextBox)sender).Text.Trim(), out int result))
            {
                ((TextBox)sender).Text = result.ToString();
                RefreshControls();
            }
        }
        private void KeyPressedDecimal(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                e.Handled = true;
            if ((e.KeyChar == '.') && ((TextBox)sender).Text.IndexOf('.') > -1)
                e.Handled = true;
        }
        private void KeyPressedInteger(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                e.Handled = true;
        }
        //private void Control_KeyUp(object sender, KeyEventArgs e)
        //{
        //    Debug.Print("This control is a " + sender.ToString() + " Key Pressed:" + e.KeyCode.ToString());
        //    if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return) || (e.KeyCode == Keys.Down))
        //    {
        //        this.SelectNextControl((Control)sender, true, true, true, true);
        //        e.Handled = true;
        //    }
        //    if (e.KeyCode == Keys.Up)
        //    {
        //        this.SelectNextControl((Control)sender, false, true, true, true);
        //        e.Handled = true;
        //    }
        //}
#pragma warning disable IDE1006 // Naming Styles
        private void btnSalePrice_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // Naming Styles
        {
            Loan.CalculateSalePrice();
            txtSalePrice.Text = Loan.SalePrice.ToString("N2");
            txtAmountFinanced.Text = Loan.AmountFinanced.ToString("N2");
            RefreshControls();
        }
#pragma warning disable IDE1006 // Naming Styles
        private void btnInterest_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // Naming Styles
        {
            Loan.WhenDue = cbxDueAtMonthEnd.Checked ? DueDate.EndOfPeriod : DueDate.BegOfPeriod;
            Loan.CalculateInterestRate();
            txtInterestRate.Text = Loan.InterestRate.ToString("N4");
            RefreshControls();
        }
#pragma warning disable IDE1006 // Naming Styles
        private void btnPayment_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // Naming Styles
        {
            Loan.CalculateMonthlyPayment();
            txtMonthlyPayment.Text = Loan.MonthlyPayment.ToString("N2");
            RefreshControls();
        }
#pragma warning disable IDE1006 // Naming Styles
        private void cbxDueAtMonthEnd_CheckedChanged(object sender, EventArgs e)
#pragma warning restore IDE1006 // Naming Styles
        {
            Loan.WhenDue = cbxDueAtMonthEnd.Checked ? DueDate.EndOfPeriod : DueDate.BegOfPeriod;
            if (btnInterest.Enabled && Loan.NumberPayments > 0)
            {
                Loan.CalculateInterestRate();
                txtInterestRate.Text = Loan.InterestRate.ToString("N4");
            }
            RefreshControls();
        }
        private void ClearForm()
        {
            txtCompensatingBalance.Text = "";
            txtDownPayment.Text = "";
            txtInterestRate.Text = "";
            txtMonthlyPayment.Text = "";
            txtNumberPayments.Text = "";
            txtAmountFinanced.Text = "";
            txtSalePrice.Text = "";
            txtTradeIn.Text = "";
            txtAmountStillOwed.Text = "";
            cbxDueAtMonthEnd.Checked = false;
            Refresh();
        }
#pragma warning disable IDE1006 // Naming Styles
        private void btnNumPayments_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // Naming Styles
        {
            Loan.CalculateNumberPayments();

            txtNumberPayments.Text = Loan.NumberPayments.ToString();
            RefreshControls();
        }
#pragma warning disable IDE1006 // Naming Styles
        private void btnDone_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // Naming Styles
        {
            Close();
        }
#pragma warning disable IDE1006 // Naming Styles
        private void btnClear_Click(object sender, EventArgs e)
#pragma warning restore IDE1006 // Naming Styles
        {
            Loan.ClearValues();
            ClearForm();
        }
    }
}

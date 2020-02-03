namespace LoanCalculator
{
    partial class frmLoanWorksheet
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblMonthlyPayment = new System.Windows.Forms.Label();
            this.lblNumPayments = new System.Windows.Forms.Label();
            this.lblInterestRate = new System.Windows.Forms.Label();
            this.txtPresentValue = new System.Windows.Forms.TextBox();
            this.lblSalePrice = new System.Windows.Forms.Label();
            this.lblCompensatingBalance = new System.Windows.Forms.Label();
            this.lblLoanPresentValue = new System.Windows.Forms.Label();
            this.txtCompensatingBalance = new System.Windows.Forms.TextBox();
            this.lblAmtOwed = new System.Windows.Forms.Label();
            this.lblDownPayment = new System.Windows.Forms.Label();
            this.lblTradeIn = new System.Windows.Forms.Label();
            this.txtTradeIn = new System.Windows.Forms.TextBox();
            this.txtAmountStillOwed = new System.Windows.Forms.TextBox();
            this.txtDownPayment = new System.Windows.Forms.TextBox();
            this.txtSalePrice = new System.Windows.Forms.TextBox();
            this.btnSalePrice = new System.Windows.Forms.Button();
            this.txtNumberPayments = new System.Windows.Forms.TextBox();
            this.btnNumPayments = new System.Windows.Forms.Button();
            this.txtInterestRate = new System.Windows.Forms.TextBox();
            this.btnInterest = new System.Windows.Forms.Button();
            this.txtMonthlyPayment = new System.Windows.Forms.TextBox();
            this.cbxDueAtMonthEnd = new System.Windows.Forms.CheckBox();
            this.btnPayment = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnDone = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblMonthlyPayment
            // 
            this.lblMonthlyPayment.AutoSize = true;
            this.lblMonthlyPayment.Location = new System.Drawing.Point(36, 226);
            this.lblMonthlyPayment.Name = "lblMonthlyPayment";
            this.lblMonthlyPayment.Size = new System.Drawing.Size(88, 13);
            this.lblMonthlyPayment.TabIndex = 70;
            this.lblMonthlyPayment.Text = "Monthly Payment";
            // 
            // lblNumPayments
            // 
            this.lblNumPayments.AutoSize = true;
            this.lblNumPayments.Location = new System.Drawing.Point(61, 168);
            this.lblNumPayments.Name = "lblNumPayments";
            this.lblNumPayments.Size = new System.Drawing.Size(63, 13);
            this.lblNumPayments.TabIndex = 68;
            this.lblNumPayments.Text = "# Payments";
            // 
            // lblInterestRate
            // 
            this.lblInterestRate.AutoSize = true;
            this.lblInterestRate.Location = new System.Drawing.Point(87, 197);
            this.lblInterestRate.Name = "lblInterestRate";
            this.lblInterestRate.Size = new System.Drawing.Size(37, 13);
            this.lblInterestRate.TabIndex = 69;
            this.lblInterestRate.Text = "APR%";
            // 
            // txtPresentValue
            // 
            this.txtPresentValue.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPresentValue.Location = new System.Drawing.Point(130, 143);
            this.txtPresentValue.Name = "txtPresentValue";
            this.txtPresentValue.ReadOnly = true;
            this.txtPresentValue.Size = new System.Drawing.Size(92, 13);
            this.txtPresentValue.TabIndex = 66;
            this.txtPresentValue.TabStop = false;
            // 
            // lblSalePrice
            // 
            this.lblSalePrice.AutoSize = true;
            this.lblSalePrice.Location = new System.Drawing.Point(69, 117);
            this.lblSalePrice.Name = "lblSalePrice";
            this.lblSalePrice.Size = new System.Drawing.Size(55, 13);
            this.lblSalePrice.TabIndex = 61;
            this.lblSalePrice.Text = "Sale Price";
            // 
            // lblCompensatingBalance
            // 
            this.lblCompensatingBalance.AutoSize = true;
            this.lblCompensatingBalance.Location = new System.Drawing.Point(8, 92);
            this.lblCompensatingBalance.Name = "lblCompensatingBalance";
            this.lblCompensatingBalance.Size = new System.Drawing.Size(116, 13);
            this.lblCompensatingBalance.TabIndex = 60;
            this.lblCompensatingBalance.Text = "Compensating Balance";
            // 
            // lblLoanPresentValue
            // 
            this.lblLoanPresentValue.AutoSize = true;
            this.lblLoanPresentValue.Location = new System.Drawing.Point(34, 143);
            this.lblLoanPresentValue.Name = "lblLoanPresentValue";
            this.lblLoanPresentValue.Size = new System.Drawing.Size(90, 13);
            this.lblLoanPresentValue.TabIndex = 65;
            this.lblLoanPresentValue.Text = "Amount Financed";
            // 
            // txtCompensatingBalance
            // 
            this.txtCompensatingBalance.BackColor = System.Drawing.SystemColors.Control;
            this.txtCompensatingBalance.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCompensatingBalance.Location = new System.Drawing.Point(130, 92);
            this.txtCompensatingBalance.Name = "txtCompensatingBalance";
            this.txtCompensatingBalance.ReadOnly = true;
            this.txtCompensatingBalance.Size = new System.Drawing.Size(92, 13);
            this.txtCompensatingBalance.TabIndex = 63;
            this.txtCompensatingBalance.TabStop = false;
            // 
            // lblAmtOwed
            // 
            this.lblAmtOwed.AutoSize = true;
            this.lblAmtOwed.Location = new System.Drawing.Point(31, 37);
            this.lblAmtOwed.Name = "lblAmtOwed";
            this.lblAmtOwed.Size = new System.Drawing.Size(93, 13);
            this.lblAmtOwed.TabIndex = 67;
            this.lblAmtOwed.Text = "Amount Still Owed";
            this.lblAmtOwed.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblDownPayment
            // 
            this.lblDownPayment.AutoSize = true;
            this.lblDownPayment.Location = new System.Drawing.Point(45, 66);
            this.lblDownPayment.Name = "lblDownPayment";
            this.lblDownPayment.Size = new System.Drawing.Size(79, 13);
            this.lblDownPayment.TabIndex = 64;
            this.lblDownPayment.Text = "Down Payment";
            this.lblDownPayment.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblTradeIn
            // 
            this.lblTradeIn.AutoSize = true;
            this.lblTradeIn.Location = new System.Drawing.Point(77, 8);
            this.lblTradeIn.Name = "lblTradeIn";
            this.lblTradeIn.Size = new System.Drawing.Size(47, 13);
            this.lblTradeIn.TabIndex = 62;
            this.lblTradeIn.Text = "Trade-In";
            this.lblTradeIn.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtTradeIn
            // 
            this.txtTradeIn.Location = new System.Drawing.Point(130, 5);
            this.txtTradeIn.Name = "txtTradeIn";
            this.txtTradeIn.Size = new System.Drawing.Size(92, 20);
            this.txtTradeIn.TabIndex = 46;
            this.txtTradeIn.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressedDecimal);
            this.txtTradeIn.Leave += new System.EventHandler(this.LeaveEventDecimal2);
            // 
            // txtAmountStillOwed
            // 
            this.txtAmountStillOwed.Location = new System.Drawing.Point(130, 34);
            this.txtAmountStillOwed.Name = "txtAmountStillOwed";
            this.txtAmountStillOwed.Size = new System.Drawing.Size(92, 20);
            this.txtAmountStillOwed.TabIndex = 47;
            this.txtAmountStillOwed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressedDecimal);
            this.txtAmountStillOwed.Leave += new System.EventHandler(this.LeaveEventDecimal2);
            // 
            // txtDownPayment
            // 
            this.txtDownPayment.Location = new System.Drawing.Point(130, 63);
            this.txtDownPayment.Name = "txtDownPayment";
            this.txtDownPayment.Size = new System.Drawing.Size(92, 20);
            this.txtDownPayment.TabIndex = 48;
            this.txtDownPayment.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressedDecimal);
            this.txtDownPayment.Leave += new System.EventHandler(this.LeaveEventDecimal2);
            // 
            // txtSalePrice
            // 
            this.txtSalePrice.Location = new System.Drawing.Point(130, 114);
            this.txtSalePrice.Name = "txtSalePrice";
            this.txtSalePrice.Size = new System.Drawing.Size(92, 20);
            this.txtSalePrice.TabIndex = 49;
            this.txtSalePrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressedDecimal);
            this.txtSalePrice.Leave += new System.EventHandler(this.LeaveEventDecimal2);
            // 
            // btnSalePrice
            // 
            this.btnSalePrice.Location = new System.Drawing.Point(228, 112);
            this.btnSalePrice.Name = "btnSalePrice";
            this.btnSalePrice.Size = new System.Drawing.Size(66, 23);
            this.btnSalePrice.TabIndex = 50;
            this.btnSalePrice.Text = "Calculate";
            this.btnSalePrice.UseVisualStyleBackColor = true;
            this.btnSalePrice.Click += new System.EventHandler(this.btnSalePrice_Click);
            // 
            // txtNumberPayments
            // 
            this.txtNumberPayments.Location = new System.Drawing.Point(130, 165);
            this.txtNumberPayments.Name = "txtNumberPayments";
            this.txtNumberPayments.Size = new System.Drawing.Size(92, 20);
            this.txtNumberPayments.TabIndex = 51;
            this.txtNumberPayments.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressedInteger);
            this.txtNumberPayments.Leave += new System.EventHandler(this.LeaveEventInteger);
            // 
            // btnNumPayments
            // 
            this.btnNumPayments.Location = new System.Drawing.Point(228, 163);
            this.btnNumPayments.Name = "btnNumPayments";
            this.btnNumPayments.Size = new System.Drawing.Size(66, 23);
            this.btnNumPayments.TabIndex = 52;
            this.btnNumPayments.Text = "Calculate";
            this.btnNumPayments.UseVisualStyleBackColor = true;
            this.btnNumPayments.Click += new System.EventHandler(this.btnNumPayments_Click);
            // 
            // txtInterestRate
            // 
            this.txtInterestRate.Location = new System.Drawing.Point(130, 194);
            this.txtInterestRate.Name = "txtInterestRate";
            this.txtInterestRate.Size = new System.Drawing.Size(92, 20);
            this.txtInterestRate.TabIndex = 53;
            this.txtInterestRate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressedDecimal);
            this.txtInterestRate.Leave += new System.EventHandler(this.LeaveEventDecimal4);
            // 
            // btnInterest
            // 
            this.btnInterest.Location = new System.Drawing.Point(228, 192);
            this.btnInterest.Name = "btnInterest";
            this.btnInterest.Size = new System.Drawing.Size(66, 23);
            this.btnInterest.TabIndex = 54;
            this.btnInterest.Text = "Calculate";
            this.btnInterest.UseVisualStyleBackColor = true;
            this.btnInterest.Click += new System.EventHandler(this.btnInterest_Click);
            // 
            // txtMonthlyPayment
            // 
            this.txtMonthlyPayment.Location = new System.Drawing.Point(130, 223);
            this.txtMonthlyPayment.Name = "txtMonthlyPayment";
            this.txtMonthlyPayment.Size = new System.Drawing.Size(92, 20);
            this.txtMonthlyPayment.TabIndex = 55;
            this.txtMonthlyPayment.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressedDecimal);
            this.txtMonthlyPayment.Leave += new System.EventHandler(this.LeaveEventDecimal2);
            // 
            // cbxDueAtMonthEnd
            // 
            this.cbxDueAtMonthEnd.AutoSize = true;
            this.cbxDueAtMonthEnd.Location = new System.Drawing.Point(130, 249);
            this.cbxDueAtMonthEnd.Name = "cbxDueAtMonthEnd";
            this.cbxDueAtMonthEnd.Size = new System.Drawing.Size(113, 17);
            this.cbxDueAtMonthEnd.TabIndex = 56;
            this.cbxDueAtMonthEnd.Text = "Due at Month End";
            this.cbxDueAtMonthEnd.UseVisualStyleBackColor = true;
            this.cbxDueAtMonthEnd.CheckedChanged += new System.EventHandler(this.cbxDueAtMonthEnd_CheckedChanged);
            // 
            // btnPayment
            // 
            this.btnPayment.Location = new System.Drawing.Point(228, 221);
            this.btnPayment.Name = "btnPayment";
            this.btnPayment.Size = new System.Drawing.Size(66, 23);
            this.btnPayment.TabIndex = 57;
            this.btnPayment.Text = "Calculate";
            this.btnPayment.UseVisualStyleBackColor = true;
            this.btnPayment.Click += new System.EventHandler(this.btnPayment_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(144, 271);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 58;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnDone
            // 
            this.btnDone.Location = new System.Drawing.Point(219, 271);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(75, 23);
            this.btnDone.TabIndex = 59;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // frmLoanWorksheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 297);
            this.Controls.Add(this.lblMonthlyPayment);
            this.Controls.Add(this.lblNumPayments);
            this.Controls.Add(this.lblInterestRate);
            this.Controls.Add(this.txtPresentValue);
            this.Controls.Add(this.lblSalePrice);
            this.Controls.Add(this.lblCompensatingBalance);
            this.Controls.Add(this.lblLoanPresentValue);
            this.Controls.Add(this.txtCompensatingBalance);
            this.Controls.Add(this.lblAmtOwed);
            this.Controls.Add(this.lblDownPayment);
            this.Controls.Add(this.lblTradeIn);
            this.Controls.Add(this.txtTradeIn);
            this.Controls.Add(this.txtAmountStillOwed);
            this.Controls.Add(this.txtDownPayment);
            this.Controls.Add(this.txtSalePrice);
            this.Controls.Add(this.btnSalePrice);
            this.Controls.Add(this.txtNumberPayments);
            this.Controls.Add(this.btnNumPayments);
            this.Controls.Add(this.txtInterestRate);
            this.Controls.Add(this.btnInterest);
            this.Controls.Add(this.txtMonthlyPayment);
            this.Controls.Add(this.cbxDueAtMonthEnd);
            this.Controls.Add(this.btnPayment);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnDone);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "frmLoanWorksheet";
            this.Text = "Simple Loan Calculator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMonthlyPayment;
        private System.Windows.Forms.Label lblNumPayments;
        private System.Windows.Forms.Label lblInterestRate;
        private System.Windows.Forms.TextBox txtPresentValue;
        private System.Windows.Forms.Label lblSalePrice;
        private System.Windows.Forms.Label lblCompensatingBalance;
        private System.Windows.Forms.Label lblLoanPresentValue;
        private System.Windows.Forms.TextBox txtCompensatingBalance;
        private System.Windows.Forms.Label lblAmtOwed;
        private System.Windows.Forms.Label lblDownPayment;
        private System.Windows.Forms.Label lblTradeIn;
        private System.Windows.Forms.TextBox txtTradeIn;
        private System.Windows.Forms.TextBox txtAmountStillOwed;
        private System.Windows.Forms.TextBox txtDownPayment;
        private System.Windows.Forms.TextBox txtSalePrice;
        private System.Windows.Forms.Button btnSalePrice;
        private System.Windows.Forms.TextBox txtNumberPayments;
        private System.Windows.Forms.Button btnNumPayments;
        private System.Windows.Forms.TextBox txtInterestRate;
        private System.Windows.Forms.Button btnInterest;
        private System.Windows.Forms.TextBox txtMonthlyPayment;
        private System.Windows.Forms.CheckBox cbxDueAtMonthEnd;
        private System.Windows.Forms.Button btnPayment;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnDone;
    }
}
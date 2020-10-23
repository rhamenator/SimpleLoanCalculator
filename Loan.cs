using System;
using Microsoft.VisualBasic;

namespace LoanCalculator
{
    public static class Loan
    { 
        public static double CompensatingBalance { get; set; }
        public static double AmountFinanced { get; set; }
        public static double TradeIn { get; set; }
        public static double AmountStillOwed { get; set; }
        public static double DownPayment { get; set; }
        public static double SalePrice { get; set; }
        public static double MonthlyPayment { get; set; }
        public static double InterestRate { get; set; }
        public static double NumberPayments { get; set; }
        public static DueDate WhenDue { get; set; }

        public static void ClearValues()
        {
            CompensatingBalance = 0;
            AmountFinanced = 0;
            TradeIn = 0;
            AmountStillOwed = 0;
            DownPayment = 0;
            SalePrice = 0;
            MonthlyPayment = 0;
            InterestRate = 0;
            NumberPayments = 0;
        }

        public static void CalculateCompensatingBalance()
        {
            CompensatingBalance = TradeIn - AmountStillOwed + DownPayment;
            //Console.WriteLine("CalculateCompensatingBalance:");
            //Console.WriteLine("TradeIn = " + TradeIn.ToString());
            //Console.WriteLine("AmountStillOwed = " + AmountStillOwed.ToString());
            //Console.WriteLine("DownPayment = " + DownPayment.ToString());
            //Console.WriteLine("CompensatingBalance = " + CompensatingBalance.ToString());
        }
        public static void CalculateAmountFinanced()
        {
            AmountFinanced = SalePrice - CompensatingBalance;
            //Console.WriteLine("CalculateAmountFinanced:");
            //Console.WriteLine("SalePrice = " + SalePrice.ToString());
            //Console.WriteLine("CompensatingBalance = " + CompensatingBalance.ToString());
            //Console.WriteLine("AmountFinanced = " + AmountFinanced.ToString());
        }

        public static void CalculateSalePrice()
        {
            AmountFinanced
                = Financial.PV(InterestRate / 1200, NumberPayments,
                    -MonthlyPayment, 0, WhenDue);

            CompensatingBalance = Math.Round(CompensatingBalance, 2);
            AmountFinanced = Math.Round(AmountFinanced, 2);
            TradeIn = Math.Round(TradeIn, 2);
            AmountStillOwed = Math.Round(AmountStillOwed, 2);
            DownPayment = Math.Round(DownPayment, 2);

            SalePrice = AmountFinanced + CompensatingBalance;
        }

        public static void CalculateInterestRate()
        {
            InterestRate
                = Math.Abs(Financial.Rate(NumberPayments, -MonthlyPayment,
                    AmountFinanced, 0, WhenDue, 0.1) * 1200);

            InterestRate = Math.Round(InterestRate, 6);
        }

        public static void CalculateMonthlyPayment()
        {
            MonthlyPayment
                = Math.Abs(Financial.Pmt(InterestRate / 1200, NumberPayments,
                    AmountFinanced, 0, WhenDue));

            MonthlyPayment = Math.Round(MonthlyPayment, 2);
        }

        public static void CalculateNumberPayments()
        {
            NumberPayments
                = Financial.NPer(InterestRate / 1200, -MonthlyPayment,
                    AmountFinanced, 0, WhenDue);

            NumberPayments = Math.Round(NumberPayments, 0);
        }
    }
}

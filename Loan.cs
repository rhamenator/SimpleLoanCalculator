using System;
using static SimpleLoanCalculator.Financial;
//using Microsoft.VisualBasic;

namespace SimpleLoanCalculator
{
    public static class Loan
    { 
        public static decimal CompensatingBalance { get; set; }
        public static decimal AmountFinanced { get; set; }
        public static decimal TradeIn { get; set; }
        public static decimal AmountStillOwed { get; set; }
        public static decimal DownPayment { get; set; }
        public static decimal SalePrice { get; set; }
        public static decimal MonthlyPayment { get; set; }
        public static decimal InterestRate { get; set; }
        public static decimal NumberPayments { get; set; }
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

            CompensatingBalance = System.Math.Round(CompensatingBalance, 2);
            AmountFinanced = System.Math.Round(AmountFinanced, 2);
            TradeIn = System.Math.Round(TradeIn, 2);
            AmountStillOwed = System.Math.Round(AmountStillOwed, 2);
            DownPayment = System.Math.Round(DownPayment, 2);

            SalePrice = AmountFinanced + CompensatingBalance;
        }

        public static void CalculateInterestRate()
        {
            InterestRate
                = System.Math.Abs(Financial.Rate(NumberPayments, -MonthlyPayment,
                    AmountFinanced, 0, WhenDue, 0.1m) * 1200);

            InterestRate = System.Math.Round(InterestRate, 4);
        }

        public static void CalculateMonthlyPayment()
        {
            MonthlyPayment
                = System.Math.Abs(Financial.Pmt(InterestRate / 1200, NumberPayments,
                    AmountFinanced, 0, WhenDue));

            MonthlyPayment = System.Math.Round(MonthlyPayment, 2);
        }

        public static void CalculateNumberPayments()
        {
            NumberPayments
                = Financial.NPer(InterestRate / 1200, -MonthlyPayment,
                    AmountFinanced, 0, WhenDue);

            NumberPayments = System.Math.Round(NumberPayments, 0);
        }
    }
}

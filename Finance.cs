namespace LoanCalculator
{
    class Finance
    {
        public static double InterestRateGuess(int numberPaymentsPerYear, 
            double principal, 
            double totalFinanceCharges, 
            int totalNumberOfPayments)
        {
            return 2 * numberPaymentsPerYear * totalFinanceCharges / (principal * (totalNumberOfPayments + 1));
        }
    }
}

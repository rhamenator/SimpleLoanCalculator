using System;
using System.Numerics;
using System.Collections;

namespace SimpleLoanCalculator
{
    class Math
    {
        private const int EXP_ITERATIONS = 27;

        public static decimal Factorial(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException("Factorial argument must be greater than 0.");
            BigInteger result = Fact(n);
            if (result > decimal.MaxValue)
                throw new ArgumentOutOfRangeException("Factorial result too large.");
            else return (decimal)result;
        }
        private static BigInteger Fact(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException("Factorial argument must be greater than 0.");
            BigInteger acc = 1;
            for (int i = 1; i <= n; i++)
                acc *= i;
            return acc;
        }
        public static decimal Exp(decimal x)
        {
            int iteration = EXP_ITERATIONS;
            decimal factorial;
            decimal result = 1;
            //Use a Taylor series to calculate e^x
            for (int i = EXP_ITERATIONS; i > 0; i--)
            {
                factorial = Factorial(iteration);
                result += Pow(x, iteration) / factorial;
            }
            return result;
        }
        public static decimal Pow(decimal Base, decimal Exponent)
        {
            if (Base <= 0)
                throw new ArgumentOutOfRangeException("Base must be greater than 0.");
            return Exp(Exponent * Ln(Base));
        }
        public static decimal Pow(decimal Base, int Exponent)
        {
            //if (Base <= 0)
            //    throw new ArgumentOutOfRangeException("Base must be greater than 0.");
            //else 
            if (System.Math.Abs(Exponent) == 1)
                return Base;
            else if (Exponent == 0)
                return 1;
            else if (Exponent < 0)
                return 1 / Pow(Base, System.Math.Abs(Exponent));
            else
            {
                decimal result = 1m;
                BitArray e = new BitArray(BitConverter.GetBytes(Exponent));
                int t = e.Count;

                for (int i = t - 1; i >= 0; --i)
                {
                    result *= result;
                    if (e[i] == true)
                    {
                        result *= Base;
                    }
                }
                return result;
            }
        }

        public static decimal Ln(decimal Power)
        {
            decimal N, P, L, R, A, E;
            E = Exp(1m);
            P = Power;
            N = 0.0m;
            if (Power <= 0m)
                throw new ArgumentOutOfRangeException("Argument for Ln(x) must be greater than 0");

            // Speed up the convergence by calculating the integral
            while (P >= E)
            {
                P /= E;
                N++;
            }
            N += (P / E);
            P = Power;

            do
            {
                A = N;
                L = (P / (Exp(N - 1.0m)));
                R = ((N - 1.0m) * E);
                N = ((L + R) / E);
            } while (N != A);

            return N;
        }

    }
}

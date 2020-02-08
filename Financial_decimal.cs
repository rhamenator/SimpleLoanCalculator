// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using Microsoft.VisualBasic;

namespace Microsoft.VisualBasic
{
    public static class Financial
    {

        // ============================================================================
        // Decimal financial method overloads.
        // ============================================================================

        private const decimal cnL_IT_STEP = 0.00001m;
        private const decimal cnL_IT_EPSILON = 0.0000001m;


        // -------------------------------------------------------------
        // 
        // Name          : DDB
        // Purpose       : Calculates depreciation for a period based on the
        // doubly-declining balance method. Returns the result.
        // It raises an error if parameters are invalid.
        // Derivation    : At each period, 2*balance/nper is subtracted
        // from the balance.  The balance starts at the purchase
        // value, and the total of the payments may not
        // exceed (value - salvage).  The algorithm uses a non-
        // iterative method to calculate the payment.
        // Note that only the integral values of nper and per make any
        // sense.  However, Excel allowed non-integral values, and
        // thus these routines also work with non-integral input.
        // 
        // PMT = 2 * (value / nper) * ( (nper -2) / nper ) ^ (per - 1)
        // 
        // total = value * ( 1 - ( (nper - 2) / nper ) ^ per )
        // 
        // excess = total - (value - salvage)
        // 
        // ddb =  PMT        : if  excess <= 0
        // PMT-excess : if  PMT >= excess > 0
        // 0          : if  excess > PMT
        // Returns       : decimal
        // 
        // -------------------------------------------------------------
        // 
        public static decimal DDB(decimal Cost, decimal Salvage, decimal Life, decimal Period, decimal Factor = 2.0m)
        {
            decimal mRet;
            decimal mTot;
            decimal mExcess;
            decimal mTemp;
            decimal mNTemp;

            // Handle invalid parameters
            if (Factor <= 0.0m || Salvage < 0.0m || Period <= 0.0m || Period > Life)
                throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue1, "Factor"));

            // Handle special (trivial) cases
            if (Cost <= 0.0m)
                return 0.0m;

            if (Life < 2.0m)
                return (Cost - Salvage);

            if (Life == 2.0m && Period > 1.0m)
                return 0.0m;

            if (Life == 2.0m && Period <= 1.0m)
                return (Cost - Salvage);

            if (Period <= 1.0m)
            {
                mRet = Cost * Factor / Life;
                mTemp = Cost - Salvage;
                if (mRet > mTemp)
                    return mTemp;
                else
                    return mRet;
            }

            // Perform the calculation
            mTemp = (Life - Factor) / Life;
            mNTemp = Period - 1.0m;

            // WARSI Using the exponent operator for pow(..) in C code of DDB. Still got
            // to make sure that they (pow and ^) are same for all conditions
            mRet = Factor * Cost / Life * (decimal)Math.Pow((double)mTemp, (double)mNTemp);

            // WARSI Using the exponent operator for pow(..) in C code of DDB. Still got
            // to make sure that they (pow and ^) are same for all conditions
            mTot = Cost * (1 - (decimal)Math.Pow((double)mTemp, (double)Period));
            mExcess = mTot - Cost + Salvage;

            if (mExcess > 0.0m)
                mRet = mRet - mExcess;

            if (mRet >= 0.0m)
                return mRet;
            else
                return 0.0m;
        }



        // -------------------------------------------------------------
        // 
        // Name                      : FV
        // Purpose                   : It is computed as -
        //                                                      (1+rate)^nper - 1
        // fv = -pv * (1 + rate)^nper - PMT * (1+rate * type) * -----------------
        //                                                            rate
        // 
        // fv = -pv - PMT * nper     : if rate == 0
        // 
        // 
        // Returns                   : decimal
        // 
        // -------------------------------------------------------------
        // 
        public static decimal FV(decimal Rate, decimal NPer, decimal Pmt, decimal PV = 0, DueDate Due = DueDate.EndOfPeriod)
        {
            return FV_Internal(Rate, NPer, Pmt, PV, Due);
        }



        private static decimal FV_Internal(decimal Rate, decimal NPer, decimal Pmt, decimal PV = 0, DueDate Due = DueDate.EndOfPeriod)
        {
            decimal mTemp;
            decimal mTemp2;
            decimal mTemp3;

            // Performing calculation
            if (Rate == 0.0m)
                return (-PV - Pmt * NPer);

            if (Due != DueDate.EndOfPeriod)
                mTemp = 1.0m + Rate;
            else
                mTemp = 1.0m;

            mTemp3 = 1.0m + Rate;
            mTemp2 = (decimal)System.Math.Pow((double)mTemp3, (double)NPer);

            // Do divides before multiplies to avoid OverflowExceptions
            return ((-PV) * mTemp2) - ((Pmt / Rate) * mTemp * (mTemp2 - 1.0m));
        }



        // -------------------------------------------------------------
        // 
        // Name       : IPmt
        // Purpose    : This function calculates the interest part of a
        // payment for a given period.  The payment is part of
        // a series of regular payments described by the other
        // arguments.  The value is returned.  The function
        // Raises an expection if params are invalid. This function
        // calls FV and PMT. It calculates value of annuity (FV) at the
        // begining of period for which IPMT is desired. Since interest
        // rate is constant FV*rate would give IPMT.
        // 
        // if type = 1 and per = 1 then IPMT = 0.
        // 
        // if (type = 0 ) IPMT = FV(per-1)*rate
        // else IPMT = FV(per-2)*rate
        // Returns    : decimal
        // 
        // -------------------------------------------------------------
        // 
        public static decimal IPmt(decimal Rate, decimal Per, decimal NPer, decimal PV, decimal FV = 0, DueDate Due = DueDate.EndOfPeriod)
        {
            decimal Pmt;
            decimal mTFv;
            decimal mTemp;

            if (Due != DueDate.EndOfPeriod)
                mTemp = 2.0m;
            else
                mTemp = 1.0m;

            // Type = 0 or non-zero only. Offset to calculate FV
            if ((Per <= 0.0m) || (Per >= NPer + 1))
                throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue1, "Per"));

            if ((Due != DueDate.EndOfPeriod) && (Per == 1.0m))
                return 0.0m;

            // Calculate PMT (i.e. annuity) for given parms. Rqrd for FV
            Pmt = PMT_Internal(Rate, NPer, PV, FV, Due);

            // Calculate FV just before the period on which interest would be applied
            if (Due != DueDate.EndOfPeriod)
                PV = PV + Pmt;

            mTFv = FV_Internal(Rate, (Per - mTemp), Pmt, PV, DueDate.EndOfPeriod);

            return (mTFv * Rate);
        }



        // -------------------------------------------------------------
        // 
        // Name                      : IRR
        // Purpose                   : This function uses an iterative procedure to find
        // the Internal Rate of Return of an investment.  The algorithm
        // basically uses the secant method to find a rate for which the
        // NPV of the cash flow is 0.
        // This function raises an exception if the parameters are invalid.
        // 
        // This routine uses a slightly different version of the secant
        // routine in Rate.  The basic changes are:
        // - uses LDoNPV to get the 'Y-value'
        // - does not allow Rate to go below -1.
        // (if the Rate drops below -1, it is forced above again)
        // - has a decimal condition for termination:
        // NPV = 0 (within L_IT_EPSILON)
        // Rate1 - Rate0  approaches zero (rate is converging)
        // This last does not parallel Excel, but avoids a class of
        // spurious answers.  Otherwise, performance is comparable to
        // Excel's, and accuracy is often better.
        // 
        // Returns                   : decimal
        // 
        // -------------------------------------------------------------
        // 
        public static decimal IRR(ref decimal[] ValueArray, decimal Guess = 0.1m)
        {
            decimal mTemp;
            decimal mRate0;
            decimal mRate1;
            decimal mNPv0;
            decimal mNpv1;
            decimal mNpvEpsilon;
            decimal mTemp1;
            int lIndex;
            int lCVal;
            int lUpper;

            // Compiler assures that rank of ValueArray is always 1, no need to check it.  
            // WARSI Check for error codes returned by UBound. Check if they match with C code
            try   // Needed to catch dynamic arrays which have not been constructed yet.
            {
                lUpper = ValueArray.GetUpperBound(0);
            }
            catch (StackOverflowException ex)
            {
                throw ex;
            }
            catch (OutOfMemoryException ex)
            {
                throw ex;
            }
            catch (Threading.ThreadAbortException ex)
            {
                throw ex;
            }
            catch
            {
                throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue1, "ValueArray"));
            }

            lCVal = lUpper + 1;

            // Function fails for invalid parameters
            if (Guess <= (-1.0m))
                throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue1, "Guess"));

            if (lCVal <= 1)
                throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue1, "ValueArray"));

            // We scale the epsilon depending on cash flow values. It is necessary
            // because even in max accuracy case where diff is in 16th digit it
            // would get scaled up.
            if (ValueArray[0] > 0.0m)
                mTemp = ValueArray[0];
            else
                mTemp = -ValueArray[0];

            for (lIndex = 0; lIndex <= lUpper; lIndex++)
            {
                // Get max of values in cash flow
                if (ValueArray[lIndex] > mTemp)
                    mTemp = ValueArray[lIndex];
                else if ((-ValueArray[lIndex]) > mTemp)
                    mTemp = -ValueArray[lIndex];
            }

            mNpvEpsilon = mTemp * cnL_IT_EPSILON * 0.01m;

            // Set up the initial values for the secant method
            mRate0 = Guess;
            mNPv0 = OptPV2(ref ValueArray, mRate0);

            if (mNPv0 > 0.0m)
                mRate1 = mRate0 + cnL_IT_STEP;
            else
                mRate1 = mRate0 - cnL_IT_STEP;

            if (mRate1 <= (-1.0m))
                throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue1, "Rate"));

            mNpv1 = OptPV2(ref ValueArray, mRate1);

            for (lIndex = 0; lIndex <= 39; lIndex++)
            {
                if (mNpv1 == mNPv0)
                {
                    if (mRate1 > mRate0)
                        mRate0 = mRate0 - cnL_IT_STEP;
                    else
                        mRate0 = mRate0 + cnL_IT_STEP;
                    mNPv0 = OptPV2(ref ValueArray, mRate0);
                    if (mNpv1 == mNPv0)
                        throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue));
                }

                mRate0 = mRate1 - (mRate1 - mRate0) * mNpv1 / (mNpv1 - mNPv0);

                // Secant method of generating next approximation
                if (mRate0 <= (-1.0m))
                    mRate0 = (mRate1 - 1.0m) * 0.5m;

                // Basically give the algorithm a second chance. Helps the
                // algorithm when it starts to diverge to -ve side
                mNPv0 = OptPV2(ref ValueArray, mRate0);
                if (mRate0 > mRate1)
                    mTemp = mRate0 - mRate1;
                else
                    mTemp = mRate1 - mRate0;

                if (mNPv0 > 0.0m)
                    mTemp1 = mNPv0;
                else
                    mTemp1 = -mNPv0;

                // Test : npv - > 0 and rate converges
                if (mTemp1 < mNpvEpsilon && mTemp < cnL_IT_EPSILON)
                    return mRate0;

                // Exchange the values - store the new values in the 1's
                mTemp = mNPv0;
                mNPv0 = mNpv1;
                mNpv1 = mTemp;
                mTemp = mRate0;
                mRate0 = mRate1;
                mRate1 = mTemp;
            }

            throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue));
        }



        // -------------------------------------------------------------
        // 
        // Name                      : MIRR
        // Returns                   : decimal
        // 
        // -------------------------------------------------------------
        // 
        public static decimal MIRR(ref decimal[] ValueArray, decimal FinanceRate, decimal ReinvestRate)
        {
            decimal mNpvPos;
            decimal mNpvNeg;
            decimal mTemp;
            decimal mTemp1;
            decimal mNTemp2;
            int lCVal;
            int lLower;
            int lUpper;

            if (ValueArray.Rank != 1)
                throw new ArgumentException(GetResourceString(ResID.Argument_RankEQOne1, "ValueArray"));

            lLower = 0;
            lUpper = ValueArray.GetUpperBound(0);
            lCVal = lUpper - lLower + 1;

            if (FinanceRate == -1.0m)
                throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue1, "FinanceRate"));

            if (ReinvestRate == -1.0m)
                throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue1, "ReinvestRate"));

            if (lCVal <= 1)
                throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue1, "ValueArray"));

            mNpvNeg = LDoNPV(FinanceRate, ref ValueArray, -1);
            if (mNpvNeg == 0.0m)
                throw new DivideByZeroException(GetResourceString(ResID.Financial_CalcDivByZero));

            mNpvPos = LDoNPV(ReinvestRate, ref ValueArray, 1); // npv of +ve values
            mTemp1 = ReinvestRate + 1.0m;
            mNTemp2 = lCVal;
            
            mTemp = -mNpvPos * (decimal)Math.Pow((double)mTemp1, (double)mNTemp2) / (mNpvNeg * (FinanceRate + 1.0m));

            if (mTemp < 0.0m)
                throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue));

            mTemp1 = 1 / (lCVal - 1.0m);
            return (decimal)Math.Pow((double)mTemp, (double)mTemp1) - 1.0m;
        }



        // -------------------------------------------------------------
        // 
        // Name                      : NPer
        // Purpose                   :
        // 
        // -fv + PMT*(1+rate*type) / rate
        // (1+rate)^nper = ------------------------------
        // pv + PMT*(1+rate*type) / rate
        // 
        // this yields the log expression used in this function.
        // 
        // nper = (-fv - pv) / PMT     : if rate == 0
        // 
        // Returns                   : decimal
        // 
        // -------------------------------------------------------------
        // 
        public static decimal NPer(decimal Rate, decimal Pmt, decimal PV, decimal FV = 0, DueDate Due = DueDate.EndOfPeriod)
        {
            decimal mTemp3;
            decimal mTempFv;
            decimal mTempPv;
            decimal mTemp4;

            // Checking Error Conditions
            if (Rate <= -1.0m)
                throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue1, "Rate"));

            if (Rate == 0.0m)
            {
                if (Pmt == 0.0m)
                    throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue1, "Pmt"));
                return (-(PV + FV) / Pmt);
            }
            else
            {
                if (Due != 0)
                    mTemp3 = Pmt * (1.0m + Rate) / Rate;
                else
                    mTemp3 = Pmt / Rate;
                mTempFv = -FV + mTemp3;
                mTempPv = PV + mTemp3;

                // Make sure the values fit the domain of log()
                if (mTempFv < 0.0m && mTempPv < 0.0m)
                {
                    mTempFv = -1 * mTempFv;
                    mTempPv = -1 * mTempPv;
                }
                else if (mTempFv <= 0.0m || mTempPv <= 0.0m)
                    throw new ArgumentException(GetResourceString(ResID.Financial_CannotCalculateNPer));

                mTemp4 = Rate + 1.0m;
                return (decimal)((Math.Log((double)mTempFv) - Math.Log((double)mTempPv)) / Math.Log((double)mTemp4));
            }
        }



        // -------------------------------------------------------------
        // 
        // Name       : NPV
        // Purpose                   :
        // This function calculates the Net Present Value of a series of
        // payments at a given rate.  It uses LDoNPV to get the value.  No
        // real work is done here, just some error checking.
        // As with the others, this function puts its status in *lpwStatus,
        // and returns the result as a decimal.
        // 
        // Value1       Value2      Value3
        // npv = -------- + ---------- + ---------- + ... for the series...
        // (1+rate)   (1+rate)^2   (1+rate)^3
        // 
        // 
        // Returns    : decimal
        // 
        // -------------------------------------------------------------
        // 
        public static decimal NPV(decimal Rate, ref decimal[] ValueArray)
        {
            int lCVal;
            int lLower;
            int lUpper;

            if ((ValueArray == null))
                throw new ArgumentException(GetResourceString(ResID.Argument_InvalidNullValue1, "ValueArray"));

            if (ValueArray.Rank != 1)
                throw new ArgumentException(GetResourceString(ResID.Argument_RankEQOne1, "ValueArray"));

            lLower = 0;
            lUpper = ValueArray.GetUpperBound(0);
            lCVal = lUpper - lLower + 1;

            if (Rate == (-1.0m))
                throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue1, "Rate"));
            if (lCVal < 1)
                throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue1, "ValueArray"));

            return LDoNPV(Rate, ref ValueArray, 0);
        }



        // -------------------------------------------------------------
        // 
        // Name                      : PMT
        // Purpose                   :
        // This function, together with the four following
        // it (Pv, Fv, NPer and Rate), can calculate
        // a certain value associated with a regular series of
        // equal-sized payments.  This series can be fully described
        // by these values:
        // Pv   - present value
        // Fv   - future value (at end of series)
        // PMT  - the regular payment
        // nPer - the number of 'periods' over which the
        // money is paid
        // Rate - the interest rate per period.
        // (type - payments at beginning (1) or end (0) of
        // the period).
        // Each function can determine one of the values, given the others.
        // 
        // General Function for the above values:
        // 
        // (1+rate)^nper - 1
        // pv * (1+rate)^nper + PMT*(1+rate*type)*----------------- + fv  = 0
        // rate
        // rate == 0  ->  pv + PMT*nper + fv = 0
        // 
        // Thus:
        // (-fv - pv*(1+rate)^nper) * rate
        // PMT = -------------------------------------
        // (1+rate*type) * ( (1+rate)^nper - 1 )
        // 
        // PMT = (-fv - pv) / nper    : if rate == 0
        // 
        // 
        // Returns                   : decimal
        // 
        // -------------------------------------------------------------
        // 
        public static decimal Pmt(decimal Rate, decimal NPer, decimal PV, decimal FV = 0, DueDate Due = DueDate.EndOfPeriod)
        {
            return PMT_Internal(Rate, NPer, PV, FV, Due);
        }

        private static decimal PMT_Internal(decimal Rate, decimal NPer, decimal PV, decimal FV = 0, DueDate Due = DueDate.EndOfPeriod)
        {
            decimal mTemp;
            decimal mTemp2;
            decimal mTemp3;

            // Checking for error conditions
            if (NPer == 0.0m)
                throw new ArgumentException(GetResourceString(ResID.Argument_InvalidValue1, "NPer"));

            if (Rate == 0.0m)
                return ((-FV - PV) / NPer);
            else
            {
                if (Due != 0)
                    mTemp = 1.0m + Rate;
                else
                    mTemp = 1.0m;
                mTemp3 = Rate + 1.0m;
                // WARSI Using the exponent operator for pow(..) in C code of PMT. Still got
                // to make sure that they (pow and ^) are same for all conditions
                mTemp2 = (decimal)Math.Pow((double)mTemp3, (double)NPer);
                return ((-FV - PV * mTemp2) / (mTemp * (mTemp2 - 1.0m)) * Rate);
            }
        }



        // -------------------------------------------------------------
        // 
        // Name                      : PPmt
        // Purpose                   : This function calculates the principal part of a
        // payment for a given period.
        // 
        // Since PMT = IPMT +PPMT therefore
        // PPMT = PMT - IPMT
        // 
        // Returns                   : decimal
        // 
        // -------------------------------------------------------------
        // 
        public static decimal PPmt(decimal Rate, decimal Per, decimal NPer, decimal PV, decimal FV = 0, DueDate Due = DueDate.EndOfPeriod)
        {
            decimal Pmt;
            decimal mIPMT;

            // Checking for error conditions
            if ((Per <= 0.0m) || (Per >= (NPer + 1)))
                throw new ArgumentException(GetResourceString(ResID.PPMT_PerGT0AndLTNPer, "Per"));

            Pmt = PMT_Internal(Rate, NPer, PV, FV, Due);
            mIPMT = IPmt(Rate, Per, NPer, PV, FV, Due);

            return (Pmt - mIPMT);
        }



        // -------------------------------------------------------------
        // 
        // Name       : PV
        // Purpose    :
        // -fv - PMT * (1+rate*type) * ( (1+rate)^nper-1) / rate
        // pv = -----------------------------------------------------
        // (1 + rate) ^ nper
        // 
        // pv = -fv - PMT * nper     : if rate == 0
        // Returns    : decimal
        // 
        // -------------------------------------------------------------
        // 
        public static decimal PV(decimal Rate, decimal NPer, decimal Pmt, decimal FV = 0, DueDate Due = DueDate.EndOfPeriod)
        {
            decimal mTemp;
            decimal mTemp2;
            decimal mTemp3;

            if (Rate == 0.0m)
                return (-FV - Pmt * NPer);
            else
            {
                if (Due != 0)
                    mTemp = 1.0m + Rate;
                else
                    mTemp = 1.0m;
                mTemp3 = 1.0m + Rate;

                // WARSI Using the exponent operator for pow(..) in C code of PV. Still got
                // to make sure that they (pow and ^) are same for all conditions
                mTemp2 = (decimal)Math.Pow((double)mTemp3, (double)NPer);

                // Do divides before multiplies to avoid OverFlowExceptions
                return (-(FV + Pmt * mTemp * ((mTemp2 - 1.0m) / Rate)) / mTemp2);
            }
        }



        // -------------------------------------------------------------
        // 
        // Name       : Rate
        // Purpose    :
        // See PMT, above, for general details.  This
        // function is not as simple as the others.  Due to the
        // nature of the equation that links the 5 values (see
        // Excel manual - PV), it is not practical to solve for
        // rate algebraically.  As a result, this function implements
        // the secant method of approximation.  LEvalRate provides
        // the 'Y-values', for given rates.
        // 
        // Basic secant method:
        // determine Rate0 and Rate1.  Use LEvalRate to get Y0, Y1.
        // 
        // Y0
        // Rate2 = Rate1 - (Rate1 - Rate0) * ---------
        // (Y1 - Y0)
        // 
        // Get Y2 from Rate2, LEvalRate.  move 1->0, 2->1 and repeat.
        // 
        // stop when abs( Yn ) < L_IT_EPSILON
        // 
        // 
        // Returns    : decimal
        // 
        // -------------------------------------------------------------
        // 
        public static decimal Rate(decimal NPer, decimal Pmt, decimal PV, decimal FV = 0, DueDate Due = DueDate.EndOfPeriod, decimal Guess = 0.1m)
        {
            decimal mTemp;
            decimal mRate0;
            decimal mRate1;
            decimal mY0;
            decimal mY1;
            int I;

            // Check for error condition
            if (NPer <= 0.0m)
                throw new ArgumentException(GetResourceString(ResID.Rate_NPerMustBeGTZero));

            mRate0 = Guess;
            mY0 = LEvalRate(mRate0, NPer, Pmt, PV, FV, Due);
            if (mY0 > 0)
                mRate1 = (mRate0 / 2);
            else
                mRate1 = (mRate0 * 2);

            mY1 = LEvalRate(mRate1, NPer, Pmt, PV, FV, Due);

            for (I = 0; I <= 39; I++)
            {
                if (mY1 == mY0)
                {
                    if (mRate1 > mRate0)
                        mRate0 = mRate0 - cnL_IT_STEP;
                    else
                        mRate0 = mRate0 - cnL_IT_STEP * (-1);
                    mY0 = LEvalRate(mRate0, NPer, Pmt, PV, FV, Due);
                    if (mY1 == mY0)
                        throw new ArgumentException(GetResourceString(ResID.Financial_CalcDivByZero));
                }

                mRate0 = mRate1 - (mRate1 - mRate0) * mY1 / (mY1 - mY0);

                // Secant method of generating next approximation
                mY0 = LEvalRate(mRate0, NPer, Pmt, PV, FV, Due);
                if (Math.Abs(mY0) < cnL_IT_EPSILON)
                    return mRate0;

                mTemp = mY0;
                mY0 = mY1;
                mY1 = mTemp;
                mTemp = mRate0;
                mRate0 = mRate1;
                mRate1 = mTemp;
            }

            throw new ArgumentException(GetResourceString(ResID.Financial_CannotCalculateRate));
        }



        // -------------------------------------------------------------
        // 
        // Name                      : SLN
        // Purpose                   : It calculates the depreciation by the straight
        // line method and returns the result. It raises
        // an error if parameters are invalid.
        // 
        // sln = (value - salvage) / nper
        // 
        // Returns                   : decimal
        // 
        // -------------------------------------------------------------
        // 
        public static decimal SLN(decimal Cost, decimal Salvage, decimal Life)
        {
            if (Life == 0.0m)
                throw new ArgumentException(GetResourceString(ResID.Financial_LifeNEZero));

            return (Cost - Salvage) / (Life);
        }



        // -------------------------------------------------------------
        // 
        // Name       : SYD
        // Purpose    : Calculates depreciation for a period by the
        // sum-of-years-digits method. The result is returned.
        // It raises an error if parameters are invalid.
        // 
        // 2
        // syd = (value - salvage) (nper - per + 1) * ------------
        // (nper)(nper + 1)
        // 
        // Derivation : The value of the asset is divided into even parts.
        // The first period gets N, the second gets N-1, the last
        // gets 1.
        // Returns    : decimal
        // 
        // -------------------------------------------------------------
        // 
        public static decimal SYD(decimal Cost, decimal Salvage, decimal Life, decimal Period)
        {
            decimal Result;

            if (Salvage < 0.0m)
                throw new ArgumentException(GetResourceString(ResID.Financial_ArgGEZero1, "Salvage"));
            if (Period > Life)
                throw new ArgumentException(GetResourceString(ResID.Financial_PeriodLELife));
            if (Period <= 0.0m)
                throw new ArgumentException(GetResourceString(ResID.Financial_ArgGTZero1, "Period"));

            // Avoid OverflowExceptions by dividing before multiplying
            Result = (Cost - Salvage) / (Life * (Life + 1));
            return (Result * (Life + 1 - Period) * 2);
        }


        // -------------------------------------------------------------
        // 
        // Name       : LEvalRate
        // Purpose    : A local helper function. Does a useful calculation
        // for Rate.  The function is derived from the General
        // formulation noted above (PMT).
        // Returns    : decimal
        // 
        // -------------------------------------------------------------
        // 
        private static decimal LEvalRate(decimal Rate, decimal NPer, decimal Pmt, decimal PV, decimal mFv, DueDate Due)
        {
            decimal mTemp1;
            decimal mTemp2;
            decimal mTemp3;

            if (Rate == 0.0m)
                return (PV + Pmt * NPer + mFv);
            else
            {
                mTemp3 = Rate + 1.0m;
                // WARSI Using the exponent operator for pow(..) in C code of LEvalRate. Still got
                // to make sure that they (pow and ^) are same for all conditions
                mTemp1 = (decimal)Math.Pow((double)mTemp3, (double)NPer);

                if (Due != 0)
                    mTemp2 = 1 + Rate;
                else
                    mTemp2 = 1.0m;
                return (PV * mTemp1 + Pmt * mTemp2 * (mTemp1 - 1) / Rate + mFv);
            }
        }


        // -------------------------------------------------------------
        // 
        // Name                      : LDoNPV
        // Purpose                   :
        // This function performs NPV calculations for NPV,
        // MIRR, and IRR.  The wNType variable is used to set
        // the type of calculation: 0 -> do all values,
        // 1 -> only values > 0,
        // -1 -> only values < 0.
        // Note the array pointer, lpdblVal, is preceded by the count of
        // the entries.
        // Since this is just an internal-use function, no fancy exports
        // are done.  It assumes that error checking is done by the caller.
        // 
        // Value1      Value2       Value3
        // npv = -------- + ---------- + ---------- + ... for the series...
        // (1+rate)   (1+rate)^2   (1+rate)^3
        // 
        // Returns                   : decimal
        // 
        // -------------------------------------------------------------
        // 

        private static decimal LDoNPV(decimal Rate, ref decimal[] ValueArray, int iWNType)
        {
            bool bSkipPos;
            bool bSkipNeg;

            decimal mTemp2;
            decimal mTotal;
            decimal mTVal;
            int I;
            int lLower;
            int lUpper;

            bSkipPos = iWNType < 0;
            bSkipNeg = iWNType > 0;

            mTemp2 = 1.0m;
            mTotal = 0.0m;

            lLower = 0;
            lUpper = ValueArray.GetUpperBound(0);

            for (I = lLower; I <= lUpper; I++)
            {
                mTVal = ValueArray[I];
                mTemp2 = mTemp2 + mTemp2 * Rate;

                if (!((bSkipPos && mTVal > 0.0m) || (bSkipNeg && mTVal < 0.0m)))
                    mTotal = mTotal + mTVal / mTemp2;
            }

            return mTotal;
        }

        // ------------------------------------------------------------------------------------------------------
        // Optimized version of PV2
        // ------------------------------------------------------------------------------------------------------

        private static decimal OptPV2(ref decimal[] ValueArray, decimal Guess = 0.1m)
        {
            int lUpper, lLower, lIndex;

            lLower = 0;
            lUpper = ValueArray.GetUpperBound(0);

            decimal mTotal = 0.0m;
            decimal divRate = 1.0m + Guess;

            while (lLower <= lUpper && ValueArray[lLower] == 0.0m)
                lLower = lLower + 1;

            for (lIndex = lUpper; lIndex >= lLower; lIndex += -1)
            {
                mTotal = mTotal / divRate;
                mTotal = mTotal + ValueArray[lIndex];
            }
            return mTotal;
        }
    }
}

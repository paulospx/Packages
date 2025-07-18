
namespace ConsoleApp1
{
    internal class DiscountFactorCalculator
    {

        public static double GetDiscountFactor(double[] maturities, double[] discountFactors, double targetMaturity)
        {
            if (maturities == null || discountFactors == null || maturities.Length != discountFactors.Length)
            {
                throw new ArgumentException("Maturities and discount factors arrays must be non-null and of the same length.");
            }

            // Exact match
            for (int i = 0; i < maturities.Length; i++)
            {
                if (Math.Abs(maturities[i] - targetMaturity) < 1e-9) // Floating-point precision comparison
                {
                    return discountFactors[i];
                }
            }

            // Interpolation if no exact match
            for (int i = 1; i < maturities.Length; i++)
            {
                if (maturities[i] > targetMaturity)
                {
                    double t1 = maturities[i - 1];
                    double t2 = maturities[i];
                    double df1 = discountFactors[i - 1];
                    double df2 = discountFactors[i];

                    // Linear interpolation formula
                    return df1 + (df2 - df1) * (targetMaturity - t1) / (t2 - t1);
                }
            }

            // If targetMaturity is outside the range, return closest value
            if (targetMaturity < maturities[0])
            {
                return discountFactors[0];
            }
            else if (targetMaturity > maturities[^1]) // Using ^1 to access the last element
            {
                return discountFactors[^1];
            }

            throw new InvalidOperationException("Unexpected case in discount factor calculation.");
        }

    }
}

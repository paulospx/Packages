namespace ConsoleApp6
{
    internal class YieldCurve
    {
        public string CurveName { get; }
        public int[] MaturityYears { get; }
        public long[] YieldValues { get; } // e.g., yields in basis points or * 10^-4 for decimals

        public YieldCurve(string curveName, int[] maturityYears, long[] yieldValues)
        {
            if (maturityYears.Length != yieldValues.Length)
                throw new ArgumentException("Maturity and yield arrays must have the same length.");

            CurveName = curveName;
            MaturityYears = maturityYears;
            YieldValues = yieldValues;
        }

        // Convert to decimal yields (e.g., 250 → 2.50%)
        private double GetYieldAtYear(double year)
        {
            if (year <= MaturityYears[0])
                return YieldValues[0] / 10000.0;
            if (year >= MaturityYears[^1])
                return YieldValues[^1] / 10000.0;

            // Linear interpolation between known maturities
            for (int i = 0; i < MaturityYears.Length - 1; i++)
            {
                if (year >= MaturityYears[i] && year <= MaturityYears[i + 1])
                {
                    double x0 = MaturityYears[i];
                    double x1 = MaturityYears[i + 1];
                    double y0 = YieldValues[i] / 10000.0;
                    double y1 = YieldValues[i + 1] / 10000.0;

                    return y0 + (y1 - y0) * (year - x0) / (x1 - x0);
                }
            }

            throw new Exception("Interpolation error.");
        }

        // Compute monthly forward rates between each month up to max maturity
        public List<(int Month, double ForwardRate)> CalculateMonthlyForwardRates()
        {
            var forwards = new List<(int, double)>();
            int totalMonths = MaturityYears[^1] * 12;

            for (int month = 1; month <= totalMonths; month++)
            {
                double t1 = (month - 1) / 12.0;
                double t2 = month / 12.0;

                double r1 = GetYieldAtYear(t1);
                double r2 = GetYieldAtYear(t2);

                // Forward rate formula based on continuous compounding approximation
                double fwd = ((r2 * t2) - (r1 * t1)) / (t2 - t1);

                forwards.Add((month, fwd * 100)); // Return as percentage
            }

            return forwards;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

public class CubicHermiteSplineInterpolation
{
    public static Dictionary<double, double> Interpolate(
        Dictionary<double, double> termStructure,
        List<double> retainedMaturities)
    {
        // Sort maturities and term structure for consistency
        retainedMaturities.Sort();
        var sortedTermStructure = termStructure.OrderBy(kv => kv.Key).ToList();

        // Initialize the result
        Dictionary<double, double> interpolatedStructure = new Dictionary<double, double>();

        // Define values only at retained maturities
        foreach (var maturity in retainedMaturities)
        {
            if (termStructure.ContainsKey(maturity))
            {
                interpolatedStructure[maturity] = termStructure[maturity];
            }
        }

        // Interpolate between retained maturities
        for (int i = 0; i < retainedMaturities.Count - 1; i++)
        {
            double x0 = retainedMaturities[i];
            double x1 = retainedMaturities[i + 1];

            if (!termStructure.ContainsKey(x0) || !termStructure.ContainsKey(x1))
                continue;

            double y0 = termStructure[x0];
            double y1 = termStructure[x1];

            // Derivatives (approximated using centered differences)
            double m0 = ApproximateDerivative(sortedTermStructure, x0);
            double m1 = ApproximateDerivative(sortedTermStructure, x1);

            // Interpolate within [x0, x1]
            for (double x = x0; x < x1; x += 0.01) // step size can be adjusted
            {
                double t = (x - x0) / (x1 - x0);
                double h00 = (1 + 2 * t) * Math.Pow(1 - t, 2);
                double h10 = t * Math.Pow(1 - t, 2);
                double h01 = Math.Pow(t, 2) * (3 - 2 * t);
                double h11 = Math.Pow(t, 2) * (t - 1);

                double y = h00 * y0 + h10 * (x1 - x0) * m0 + h01 * y1 + h11 * (x1 - x0) * m1;
                interpolatedStructure[x] = y;
            }
        }

        return interpolatedStructure;
    }

    private static double ApproximateDerivative(List<KeyValuePair<double, double>> sortedStructure, double x)
    {
        int index = sortedStructure.FindIndex(kv => kv.Key == x);

        if (index == -1 || sortedStructure.Count < 2)
            return 0.0;

        if (index == 0) // Forward difference for first point
        {
            double dx = sortedStructure[1].Key - sortedStructure[0].Key;
            double dy = sortedStructure[1].Value - sortedStructure[0].Value;
            return dy / dx;
        }
        else if (index == sortedStructure.Count - 1) // Backward difference for last point
        {
            double dx = sortedStructure[index].Key - sortedStructure[index - 1].Key;
            double dy = sortedStructure[index].Value - sortedStructure[index - 1].Value;
            return dy / dx;
        }
        else // Centered difference for internal points
        {
            double dx1 = sortedStructure[index].Key - sortedStructure[index - 1].Key;
            double dy1 = sortedStructure[index].Value - sortedStructure[index - 1].Value;
            double dx2 = sortedStructure[index + 1].Key - sortedStructure[index].Key;
            double dy2 = sortedStructure[index + 1].Value - sortedStructure[index].Value;

            return (dy1 / dx1 + dy2 / dx2) / 2.0;
        }
    }
}

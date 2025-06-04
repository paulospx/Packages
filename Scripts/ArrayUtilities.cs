using System;

public class ArrayUtilities
{
    public static double CalculateAverage(long[] array, int from, int to)
    {
        // Validate input
        if (array == null || array.Length == 0)
            throw new ArgumentException("The array must not be null or empty.");
        if (from < 0 || to < 0 || from >= array.Length || to >= array.Length)
            throw new ArgumentOutOfRangeException("Indices must be within the bounds of the array.");
        if (from > to)
            throw new ArgumentException("The 'from' index must be less than or equal to the 'to' index.");

        // Calculate the sum of elements in the range
        long sum = 0;
        for (int i = from; i <= to; i++)
        {
            sum += array[i];
        }

        // Calculate and return the average
        return (double)sum / (to - from + 1);
    }

    public static void Main()
    {
        long[] array = { 10, 20, 30, 40, 50 };
        int from = 1;
        int to = 3;

        double average = CalculateAverage(array, from, to);
        Console.WriteLine($"The average of elements from index {from} to {to} is: {average}");
    }
}

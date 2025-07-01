Console.WriteLine("Financial Model");


// Get a Method that reads a table from a CSV file
string filePath = "financial_data.csv";
var csvData = System.IO.File.ReadAllLines(filePath)
    .Select(line => line.Split(','))
    .ToList();
// Display the data
foreach (var row in csvData)
{
    Console.WriteLine(string.Join(", ", row));
}

// Another method that read another csv file names assumptions.csv
string assumptionsFilePath = "assumptions.csv";
var assumptionsData = System.IO.File.ReadAllLines(assumptionsFilePath)
    .Select(line => line.Split(','))
    .ToList();

// Display the assumptions data
foreach (var row in assumptionsData)
{
    Console.WriteLine(string.Join(", ", row));
}

// Do some simple calculations with the data 
// Example: Calculate total, average, median, and standard deviation of revenue from the financial data
var revenues = new List<decimal>();
foreach (var row in csvData.Skip(1)) // Skip header row
{
    if (decimal.TryParse(row[1], out decimal revenue)) // Assuming revenue is in the second column
    {
        revenues.Add(revenue);
    }
}

decimal totalRevenue = revenues.Sum();
decimal averageRevenue = revenues.Count > 0 ? revenues.Average() : 0;
decimal medianRevenue = 0;
decimal stdDevRevenue = 0;

if (revenues.Count > 0)
{
    var sorted = revenues.OrderBy(x => x).ToList();
    int mid = sorted.Count / 2;
    if (sorted.Count % 2 == 0)
        medianRevenue = (sorted[mid - 1] + sorted[mid]) / 2;
    else
        medianRevenue = sorted[mid];

    decimal mean = averageRevenue;
    decimal sumSq = revenues.Sum(x => (x - mean) * (x - mean));
    stdDevRevenue = (decimal)Math.Sqrt((double)(sumSq / revenues.Count));
}

Console.WriteLine($"Total Revenue: {totalRevenue}");
Console.WriteLine($"Average Revenue: {averageRevenue}");
Console.WriteLine($"Median Revenue: {medianRevenue}");
Console.WriteLine($"Standard Deviation of Revenue: {stdDevRevenue}");


// Output the results to a csv file
string outputFilePath = "financial_output.csv";
using (var writer = new System.IO.StreamWriter(outputFilePath))
{
    writer.WriteLine("Total Revenue,Average Revenue,Median Revenue,Standard Deviation of Revenue");
    writer.WriteLine($"{totalRevenue},{averageRevenue},{medianRevenue},{stdDevRevenue}");
}

// See https://aka.ms/new-console-template for more information
using Triceratops;
using System.Text.Json;
using Triceratops.Utilities;
using Bogus;

Console.WriteLine("Triceratops");

Console.WriteLine("===");
Console.WriteLine("Curve Risk Adjustment");
var cra = new CurveRiskAdjustment
{
    Name = "CRA Example",
    Until = DateTime.Now.AddYears(5),
    Currency = "EUR",
    Type = "Curve Risk Adjustment",
    Value = 0.01
};

var craJson = JsonSerializer.Serialize(cra, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(craJson);


Console.WriteLine("===");
Console.WriteLine("Input Instrument");
var inputInstrument = new InputInstrument
{
    Name = "Swap",
    Description = "Descripion of swap",
    Value = 0.001
};
var inputInstrumentJson = JsonSerializer.Serialize(inputInstrument, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(inputInstrumentJson);

/*
{
    Id = 1,
    Type = "Swap",
    Notional = 1000000,
    Currency = "EUR",
    MaturityDate = new DateOnly(2030, 12, 31),
    FixedRate = 0.025,
    FloatingRateIndex = "EURIBOR 6M",
    Spread = 0.001,
    PaymentFrequency = "6M",
    DayCountConvention = "30/360"
};
*/

Console.WriteLine("===");
Console.WriteLine("Interpolation Method");
var interpolationMethod = new InterpolationMethod
{
    Name = "Information",
    LiquidPoints = "1,2,3,4,5,6,7",
    RateType = "spot",
    Type = "Inter Type",
    Ufr = 0.01
};
var interpolationMethodJson = JsonSerializer.Serialize(interpolationMethod, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(interpolationMethodJson);

Console.WriteLine("===");
Console.WriteLine("Output");
var output = new Output
{
    Frequency = "Monthly",
    Maturity = 150,
    Type = "csv",
    OutputType = "CSV File"
};
var outputJson = JsonSerializer.Serialize(output, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(outputJson);

Console.WriteLine("===");
Console.WriteLine("Shock");
var shock = new Shock
{
    Name = "Shock Name",
    Description = "Shock Description",
    From = DateTime.Now,
    Until = DateTime.Now.AddYears(1),
    Size = 0.0001,
    Type = "Shock Type"
};
var shockJson = JsonSerializer.Serialize(shock, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(shockJson);



var faker = new Faker();
var result = new List<CurveRiskAdjustment>();
for(var i = 0; i < 100; i++)
{
    var testCra = new Faker<CurveRiskAdjustment>()
        .RuleFor(u => u.CurveName, f => f.Finance.AccountName())
        .RuleFor(u => u.From, f => f.Date.Past())
        .RuleFor(u => u.Until, f => f.Date.Future())
        .RuleFor(u => u.Name, f => f.Company.CompanyName())
        .RuleFor(u => u.Description, f => f.Lorem.Sentence())
        .RuleFor(u => u.Currency, f => faker.Finance.Currency().Code)
        .RuleFor(u => u.Value, f => faker.Random.Double(0.0002, 0.0023))
        .RuleFor(u => u.Type, f => faker.Finance.TransactionType());
    result.Add(testCra);
}

Console.WriteLine("===");
Console.WriteLine("CSV");
var csv = ExporterManager.ToCsv(result);
Console.Write(csv);

Console.WriteLine("===");
var listCras = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(listCras);


var listOutputs = new List<Output>();
for (int i = 0; i < 100; i++)
{
    var fakeOutput = new Faker<Output>()
        .RuleFor(o => o.Maturity, i)
        .RuleFor(o => o.Frequency, f => f.Date.Weekday())
        .RuleFor(o => o.Type, f => f.Lorem.Word())
        .RuleFor(o => o.OutputType, f => f.Lorem.Word());

    listOutputs.Add(fakeOutput);

}

foreach (var outputItem in listOutputs)
{
    Console.WriteLine("Output Item:");
    var outputItemJson = JsonSerializer.Serialize(outputItem, new JsonSerializerOptions { WriteIndented = true });
    Console.WriteLine(outputItemJson);
}
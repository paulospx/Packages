using ConsoleApp6;
using Microsoft.Data.SqlClient;
using System.Data;

Console.WriteLine("Testing Yield Curves");

var maturities = new int[] { 1, 2, 3, 5, 10, 20 };
var yields = new long[] { 350, 23, 390, 420, 480,230 }; // in basis points (3.50%, etc.)

var curve = new YieldCurve("USD_SWAP", maturities, yields);
var forwardRates = curve.CalculateMonthlyForwardRates();

Console.WriteLine($"Monthly forward rates for curve: {curve.CurveName}");
foreach (var (month, rate) in forwardRates)
{
    Console.WriteLine($"Month {month,3}: {rate:F3}%");
}

Console.WriteLine("=== End of Test ===");


var table = IngestDatabase.ReadCsvToDataTable("C:\\Temp\\LargeData.csv", hasHeader: true);
table.WriteToSqlServer(
    connectionString: "Server=localhost;Database=TestDB;User Id=sa;Password=Your_password123;",
    destinationTableName: "dbo.LargeData",
    batchSize: 5000,
    timeoutSeconds: 600,
    options: SqlBulkCopyOptions.TableLock);




DataTable dt = new DataTable();
dt.Columns.Add("Id", typeof(int));           // will be INT NOT NULL
dt.Columns.Add("Name", typeof(string));      // NVARCHAR(4000) NULL
dt.Columns.Add("Price", typeof(decimal));    // DECIMAL(18,6)
dt.Columns.Add("IsActive", typeof(bool));    // BIT
dt.Columns.Add("CreatedAt", typeof(DateTime));

dt.Rows.Add(1, "Laptop", 999.99m, true, DateTime.Now);

string connString = "Server=localhost;Database=TestDB;Trusted_Connection=True;";

SqlTableCreator.CreateTableFromDataTable(
    dataTable: dt,
    connectionString: connString,
    tableName: "dbo.Products",
    dropIfExists: true,
    maxStringLength: -1  // Use NVARCHAR(MAX) for strings
);


// First create table
SqlTableCreator.CreateTableFromDataTable(dt, connString, "dbo.Imports", dropIfExists: true);

// Then bulk insert
dt.WriteToSqlServer(connString, "dbo.Imports");  // from previous method
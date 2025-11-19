using System.Data;
using Microsoft.Data.SqlClient;

// Ensure that you have a reference to the System.Data.SqlClient assembly in your project.
// If you are using .NET Core or .NET 5+, you may need to install the NuGet package:
// Install-Package System.Data.SqlClient

public static class SqlServerExtensions
{
    /// <summary>
    /// Writes the contents of a DataTable to a SQL Server table using SqlBulkCopy (very fast).
    /// </summary>
    /// <param name="dataTable">The DataTable to insert</param>
    /// <param name="connectionString">SQL Server connection string</param>
    /// <param name="destinationTableName">Target table name (schema optional, e.g. dbo.Users)</param>
    /// <param name="batchSize">Optional: Number of rows per batch (default 10,000)</param>
    /// <param name="timeoutSeconds">Optional: Command timeout in seconds (default 300 = 5 min)</param>
    /// <param name="options">Optional: SqlBulkCopyOptions (e.g. KeepIdentity, TableLock)</param>
    public static void WriteToSqlServer(
        this DataTable dataTable,
        string connectionString,
        string destinationTableName,
        int batchSize = 10000,
        int timeoutSeconds = 300,
        SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
    {
        if (dataTable == null) throw new ArgumentNullException(nameof(dataTable));
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Connection string is required", nameof(connectionString));
        if (string.IsNullOrWhiteSpace(destinationTableName)) throw new ArgumentException("Destination table name is required", nameof(destinationTableName));

        // Optional: Auto-map columns by name (case-insensitive)
        using (var bulkCopy = new SqlBulkCopy(connectionString, options))
        {
            bulkCopy.DestinationTableName = destinationTableName;
            bulkCopy.BatchSize = batchSize;
            bulkCopy.BulkCopyTimeout = timeoutSeconds;

            // Map DataTable columns to database columns (by name)
            foreach (DataColumn column in dataTable.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            try
            {
                bulkCopy.WriteToServer(dataTable);
                Console.WriteLine($"Successfully inserted {dataTable.Rows.Count} rows into {destinationTableName}");
            }
            catch (SqlException ex)
            {
                throw new Exception($"SQL Error during bulk insert into {destinationTableName}: {ex.Message}", ex);
            }
        }
    }
}
// using System.Data.SqlClient;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

public static class SqlTableCreator
{
    /// <summary>
    /// Creates a SQL Server table based on the DataTable schema.
    /// If the table already exists, it will be dropped and recreated (optional).
    /// </summary>
    public static void CreateTableFromDataTable(
        DataTable dataTable,
        string connectionString,
        string tableName,
        bool dropIfExists = false,
        int maxStringLength = 4000)
    {
        var sql = GetCreateTableScript(dataTable, tableName, dropIfExists, maxStringLength);
        ExecuteSql(sql, connectionString);
        var (schema, plainName) = ParseSchemaAndName(tableName);
        Console.WriteLine($"Table [{schema}].[{plainName}] created successfully with {dataTable.Columns.Count} columns.");
    }

    /// <summary>
    /// Returns the CREATE TABLE script for the DataTable (does not execute).
    /// </summary>
    public static string GetCreateTableScript(DataTable dataTable, string tableName, bool includeDrop = false, int maxStringLength = 4000)
    {
        if (dataTable == null) throw new ArgumentNullException(nameof(dataTable));
        if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentException("Table name is required", nameof(tableName));

        var (schema, plainTableName) = ParseSchemaAndName(tableName);
        var sql = new StringBuilder();

        if (includeDrop)
        {
            sql.AppendLine($"IF OBJECT_ID('{schema}.{plainTableName}', 'U') IS NOT NULL DROP TABLE [{schema}].[{plainTableName}];");
        }

        sql.AppendLine($"CREATE TABLE [{schema}].[{plainTableName}] (");

        bool first = true;
        foreach (DataColumn col in dataTable.Columns)
        {
            if (!first) sql.AppendLine(",");
            string sqlType = GetSqlType(col.DataType, maxStringLength);
            string nullSpec = col.AllowDBNull ? "NULL" : "NOT NULL";
            sql.Append($"  [{col.ColumnName}] {sqlType} {nullSpec}");
            first = false;
        }

        if (dataTable.PrimaryKey.Length == 0 && dataTable.Columns.Count > 0)
        {
            var firstCol = dataTable.Columns[0];
            if (firstCol.DataType == typeof(int) || firstCol.DataType == typeof(long) || firstCol.DataType == typeof(Guid))
            {
                sql.AppendLine(",");
                sql.Append($"  CONSTRAINT [PK_{plainTableName}] PRIMARY KEY CLUSTERED ([{firstCol.ColumnName}])");
            }
        }

        sql.AppendLine();
        sql.AppendLine(");");

        return sql.ToString();
    }

    /// <summary>
    /// Executes a non-query SQL against the provided connection string.
    /// </summary>
    public static void ExecuteSql(string sql, string connectionString, int commandTimeoutSeconds = 300)
    {
        if (string.IsNullOrWhiteSpace(sql)) throw new ArgumentException("SQL is required", nameof(sql));
        using var conn = new SqlConnection(connectionString);
        conn.Open();
        using var cmd = new SqlCommand(sql, conn) { CommandTimeout = commandTimeoutSeconds };
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Returns true if the named table exists.
    /// </summary>
    public static bool TableExists(string connectionString, string tableName)
    {
        var (schema, plainName) = ParseSchemaAndName(tableName);
        const string sql = @"
SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @tableName;
";
        using var conn = new SqlConnection(connectionString);
        conn.Open();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@schema", schema);
        cmd.Parameters.AddWithValue("@tableName", plainName);
        var count = (int)cmd.ExecuteScalar()!;
        return count > 0;
    }

    /// <summary>
    /// Creates the table only if it does not already exist.
    /// </summary>
    public static void CreateTableIfNotExists(DataTable dataTable, string connectionString, string tableName, int maxStringLength = 4000)
    {
        if (!TableExists(connectionString, tableName))
        {
            var sql = GetCreateTableScript(dataTable, tableName, includeDrop: false, maxStringLength: maxStringLength);
            ExecuteSql(sql, connectionString);
            var (schema, plain) = ParseSchemaAndName(tableName);
            Console.WriteLine($"Table [{schema}].[{plain}] created.");
        }
        else
        {
            var (schema, plain) = ParseSchemaAndName(tableName);
            Console.WriteLine($"Table [{schema}].[{plain}] already exists. No action taken.");
        }
    }

    /// <summary>
    /// Drops the specified table if it exists.
    /// </summary>
    public static void DropTable(string connectionString, string tableName)
    {
        var (schema, plain) = ParseSchemaAndName(tableName);
        string sql = $"IF OBJECT_ID('{schema}.{plain}', 'U') IS NOT NULL DROP TABLE [{schema}].[{plain}];";
        ExecuteSql(sql, connectionString);
        Console.WriteLine($"Table [{schema}].[{plain}] dropped if it existed.");
    }

    /// <summary>
    /// Performs a SqlBulkCopy from the DataTable into the target table.
    /// </summary>
    public static void BulkInsert(DataTable dataTable, string connectionString, string tableName, int batchSize = 5000, int bulkCopyTimeoutSeconds = 600)
    {
        if (dataTable == null) throw new ArgumentNullException(nameof(dataTable));
        var (schema, plain) = ParseSchemaAndName(tableName);
        using var conn = new SqlConnection(connectionString);
        conn.Open();
        using var bulk = new SqlBulkCopy(conn)
        {
            DestinationTableName = $"[{schema}].[{plain}]",
            BatchSize = batchSize,
            BulkCopyTimeout = bulkCopyTimeoutSeconds
        };

        // Map columns by name if possible
        foreach (DataColumn col in dataTable.Columns)
        {
            bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);
        }

        bulk.WriteToServer(dataTable);
        Console.WriteLine($"Bulk inserted {dataTable.Rows.Count} rows into [{schema}].[{plain}].");
    }

    /// <summary>
    /// Adds missing columns from the DataTable to the existing table.
    /// Only adds columns that do not already exist. Does not modify existing columns.
    /// </summary>
    public static void AddMissingColumns(DataTable dataTable, string connectionString, string tableName, int maxStringLength = 4000)
    {
        if (dataTable == null) throw new ArgumentNullException(nameof(dataTable));
        var (schema, plain) = ParseSchemaAndName(tableName);

        var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        const string colSql = @"
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @tableName;
";
        using (var conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using var cmd = new SqlCommand(colSql, conn);
            cmd.Parameters.AddWithValue("@schema", schema);
            cmd.Parameters.AddWithValue("@tableName", plain);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                existing.Add(rdr.GetString(0));
            }
        }

        var alterStatements = new List<string>();
        foreach (DataColumn col in dataTable.Columns)
        {
            if (!existing.Contains(col.ColumnName))
            {
                string sqlType = GetSqlType(col.DataType, maxStringLength);
                string nullSpec = col.AllowDBNull ? "NULL" : "NOT NULL";
                alterStatements.Add($"ALTER TABLE [{schema}].[{plain}] ADD [{col.ColumnName}] {sqlType} {nullSpec};");
            }
        }

        if (alterStatements.Count == 0)
        {
            Console.WriteLine($"No missing columns to add to [{schema}].[{plain}].");
            return;
        }

        // Execute all ALTER statements in a single batch
        var batchSql = string.Join(Environment.NewLine, alterStatements);
        ExecuteSql(batchSql, connectionString);
        Console.WriteLine($"Added {alterStatements.Count} columns to [{schema}].[{plain}].");
    }

    /// <summary>
    /// Parses a possibly schema-qualified table name into (schema, table).
    /// </summary>
    private static (string schema, string table) ParseSchemaAndName(string tableName)
    {
        string schema = "dbo";
        string plain = tableName;
        if (tableName.Contains("."))
        {
            var parts = tableName.Split('.');
            schema = parts[0].Trim('[', ']', '"');
            plain = parts[1].Trim('[', ']', '"');
        }
        return (schema, plain);
    }

    /// <summary>
    /// Maps CLR type to SQL type (improved mapping). maxStringLength = -1 -> NVARCHAR(MAX).
    /// </summary>
    private static string GetSqlType(Type type, int maxStringLength)
    {
        if (type == null) return "NVARCHAR(MAX)";
        // Handle nullable types
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            type = Nullable.GetUnderlyingType(type)!;

        var name = type.Name.ToLowerInvariant();
        return name switch
        {
            "string" => maxStringLength == -1 ? "NVARCHAR(MAX)" : $"NVARCHAR({(maxStringLength <= 0 ? 4000 : maxStringLength)})",
            "int32" or "int" => "INT",
            "int64" or "long" => "BIGINT",
            "boolean" or "bool" => "BIT",
            "datetime" or "datetime2" => "DATETIME2",
            "datetimeoffset" => "DATETIMEOFFSET",
            "timespan" => "TIME",
            "decimal" => "DECIMAL(18,6)",
            "double" => "FLOAT",
            "single" or "float" => "REAL",
            "guid" => "UNIQUEIDENTIFIER",
            "byte[]" => "VARBINARY(MAX)",
            "byte" => "TINYINT",
            "int16" or "short" => "SMALLINT",
            "char" => "NCHAR(1)",
            "object" => "NVARCHAR(MAX)",
            _ => "NVARCHAR(MAX)"
        };
    }
}
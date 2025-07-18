using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    public static class CsvSqlGenerator
    {
        public static string GenerateCreateTableSql(string csvPath, string tableName = "MyTable", int sampleSize = 100)
        {
            var lines = File.ReadLines(csvPath).Take(sampleSize + 1).ToList();
            if (lines.Count < 2) throw new InvalidOperationException("CSV must have at least one data row");

            var headers = lines[0].Split(',').Select(NormalizeHeader).ToList();
            var rows = lines.Skip(1).Select(l => l.Split(',')).ToList();

            var columnSamples = new Dictionary<string, List<string>>();
            for (int i = 0; i < headers.Count; i++)
            {
                columnSamples[headers[i]] = rows.Select(r =>
                    r.Length > i ? r[i]?.Trim() ?? string.Empty : string.Empty).ToList();
            }

            var sb = new StringBuilder();
            sb.AppendLine($"CREATE TABLE [{tableName}] (");

            foreach (var (colName, samples) in columnSamples)
            {
                var (sqlType, isNullable) = InferSqlType(samples);
                string nullClause = isNullable ? "NULL" : "NOT NULL";
                sb.AppendLine($"    [{colName}] {sqlType} {nullClause},");
            }

            sb.Length -= 3; // remove trailing comma
            sb.AppendLine("\n);");

            return sb.ToString();
        }

        private static (string sqlType, bool isNullable) InferSqlType(List<string> values)
        {
            bool IsAll(Func<string, bool> check) => values.All(v => string.IsNullOrWhiteSpace(v) || check(v));
            bool hasNulls = values.Any(string.IsNullOrWhiteSpace);

            if (IsAll(v => int.TryParse(v, out _))) return ("INT", hasNulls);
            if (IsAll(v => long.TryParse(v, out _))) return ("BIGINT", hasNulls);
            if (IsAll(v => decimal.TryParse(v, out _))) return ("DECIMAL(18,4)", hasNulls);
            if (IsAll(v => double.TryParse(v, out _))) return ("FLOAT", hasNulls);
            if (IsAll(v => bool.TryParse(v, out _))) return ("BIT", hasNulls);
            if (IsAll(v => DateTime.TryParse(v, out _))) return ("DATETIME2", hasNulls);
            if (IsAll(v => Guid.TryParse(v, out _))) return ("UNIQUEIDENTIFIER", hasNulls);

            // Fallback to variable-length string
            int maxLen = values.Where(v => !string.IsNullOrWhiteSpace(v)).DefaultIfEmpty(string.Empty).Max(v => v.Length);
            string strType = maxLen <= 255 ? $"NVARCHAR({Math.Max(maxLen, 50)})" : "NVARCHAR(MAX)";
            return (strType, true);
        }

        private static string NormalizeHeader(string header)
        {
            var cleaned = Regex.Replace(header.Trim(), @"[^\w\d]", "_");
            if (char.IsDigit(cleaned[0])) cleaned = "_" + cleaned;
            return cleaned;
        }
    }
}

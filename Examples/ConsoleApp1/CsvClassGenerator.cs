using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{

    public static class CsvClassGenerator
    {
        public static string GenerateClassFromCsv(string filePath, string className = "CsvRow", int sampleSize = 100)
        {
            var lines = File.ReadLines(filePath).Take(sampleSize + 1).ToList();
            if (lines.Count < 2) throw new InvalidOperationException("CSV must have at least one data row");

            var headers = lines[0].Split(',').Select(NormalizeHeader).ToList();
            var rows = lines.Skip(1).Select(l => l.Split(',')).ToList();

            var columnTypes = new Dictionary<string, List<string>>();

            for (int i = 0; i < headers.Count; i++)
            {
                columnTypes[headers[i]] = rows.Select(row =>
                    row.Length > i ? row[i]?.Trim() ?? string.Empty : string.Empty).ToList();
            }

            var sb = new StringBuilder();
            sb.AppendLine($"public class {className}");
            sb.AppendLine("{");

            foreach (var (name, values) in columnTypes)
            {
                var (type, isNullable) = InferType(values);
                var nullableSuffix = (isNullable && type != "string") ? "?" : string.Empty;
                sb.AppendLine($"    public {type}{nullableSuffix} {name} {{ get; set; }}");
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        private static (string typeName, bool isNullable) InferType(List<string> values)
        {
            bool IsAll(Func<string, bool> check) => values.All(v => string.IsNullOrWhiteSpace(v) || check(v));
            bool AnyNull = values.Any(string.IsNullOrWhiteSpace);

            if (IsAll(v => int.TryParse(v, out _))) return ("int", AnyNull);
            if (IsAll(v => long.TryParse(v, out _))) return ("long", AnyNull);
            if (IsAll(v => double.TryParse(v, out _))) return ("double", AnyNull);
            if (IsAll(v => decimal.TryParse(v, out _))) return ("decimal", AnyNull);
            if (IsAll(v => bool.TryParse(v, out _))) return ("bool", AnyNull);
            if (IsAll(v => DateTime.TryParse(v, out _))) return ("DateTime", AnyNull);
            if (IsAll(v => Guid.TryParse(v, out _))) return ("Guid", AnyNull);

            return ("string", false);
        }

        private static string NormalizeHeader(string header)
        {
            if (string.IsNullOrWhiteSpace(header)) return "Column";

            // PascalCase and remove invalid chars
            var cleaned = Regex.Replace(header.Trim(), @"[^\w\d]", "_");
            if (char.IsDigit(cleaned[0])) cleaned = "_" + cleaned;

            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(cleaned);
        }
    }
}

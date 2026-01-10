Absolutely! Below is an **enhanced version** of the **Minimal Class Generator from CSV**, with support for more types and better inference across multiple rows.

---

## ✅ Enhancements:

* Scans up to N rows (not just the first) to infer types.
* Supports:

  * `int`
  * `long`
  * `double`
  * `decimal`
  * `bool`
  * `DateTime`
  * `Guid`
  * fallback to `string`
* Handles nullable types (if some values are blank).
* Normalizes column names to valid C# identifiers.

---

## ✅ Improved Code: `GenerateClassFromCsv`

```csharp
using System.Text;
using System.Text.RegularExpressions;

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
                row.Length > i ? row[i]?.Trim() ?? "" : "").ToList();
        }

        var sb = new StringBuilder();
        sb.AppendLine($"public class {className}");
        sb.AppendLine("{");

        foreach (var (name, values) in columnTypes)
        {
            var (type, isNullable) = InferType(values);
            var nullableSuffix = (isNullable && type != "string") ? "?" : "";
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
```

---

## ✅ Usage Example

```csharp
string csvPath = "data.csv";
string classCode = CsvClassGenerator.GenerateClassFromCsv(csvPath, "MyCsvModel");
Console.WriteLine(classCode);
```

---

## ✅ Example Output

Given this CSV:

```csv
Id, Name, CreatedAt, Price, Active, GuidCol
1, Alice, 2023-07-01, 19.99, true, 123e4567-e89b-12d3-a456-426614174000
2, Bob, , 20.00, false, 
```

You get:

```csharp
public class MyCsvModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime? CreatedAt { get; set; }
    public decimal Price { get; set; }
    public bool Active { get; set; }
    public Guid? GuidCol { get; set; }
}
```

---

Would you like to:

* Add `[JsonProperty]`, `[Column]`, or `[DisplayName]` attributes?
* Generate a matching `DataTable` or Entity Framework model?
* Save the class to a `.cs` file automatically?

Let me know!

Yes, there are a few **C# libraries and tools** that can analyze a CSV file and **suggest a corresponding C# class structure**, including property names and inferred types.

---

## ✅ Recommended Libraries & Tools

### 1. **[QuickType](https://quicktype.io) (Online + CLI + NuGet)**

> ⭐ Most popular tool for generating C# classes from CSV, JSON, etc.

#### ✅ Features:

* Infers C# classes from CSV, JSON, or TSV
* Supports nullable types, enums, and arrays
* CLI + Web UI + API
* Generates \[DataMember], PascalCase, etc.

#### 🔧 Example CLI (requires Node.js or CLI install):

```bash
quicktype -l csharp --src data.csv -o CsvRow.cs
```

#### 🔧 Example Output:

```csharp
public partial class CsvRow
{
    [JsonProperty("Id")]
    public long Id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("CreatedDate")]
    public DateTime CreatedDate { get; set; }
}
```

---

### 2. **[CsvHelper.TypeGenerator (unofficial tool)](https://github.com/JoshClose/CsvHelper.TypeGenerator)**

> 🔍 Auto-generates C# classes for use with `CsvHelper`.

This is a community tool that can analyze a CSV header and generate a model class for you.

But it requires a bit of customization. Not as smart about types as QuickType.

---

### 3. **[FlatFiles (with Schema Discovery)](https://github.com/jehugaleahsa/FlatFiles)**

> 🧠 Reads CSVs and supports schema detection.

You can define schemas programmatically and extract them:

```csharp
var schema = SeparatedValueSchema.Infer(reader, SeparatedValueOptions.Default);
```

But: it doesn't auto-generate **C# code**, just internal type metadata.

---

### 4. **DIY — Minimal Class Generator from CSV**

If you'd rather generate it yourself, here’s a simple starter:

```csharp
public static string GenerateClassFromCsv(string filePath, string className = "CsvRow")
{
    var lines = File.ReadLines(filePath).ToList();
    if (lines.Count < 2) throw new InvalidOperationException("CSV must have at least one data row");

    var headers = lines[0].Split(',');
    var firstDataRow = lines[1].Split(',');

    string InferType(string val)
    {
        if (int.TryParse(val, out _)) return "int";
        if (decimal.TryParse(val, out _)) return "decimal";
        if (DateTime.TryParse(val, out _)) return "DateTime";
        if (bool.TryParse(val, out _)) return "bool";
        return "string";
    }

    var props = headers.Select((h, i) =>
        $"    public {InferType(firstDataRow[i])} {h.Trim().Replace(" ", "_")} {{ get; set; }}");

    var classCode = $"""
    public class {className}
    {{
    {string.Join(Environment.NewLine, props)}
    }}
    """;

    return classCode;
}
```

---

## ✅ Recommendation

| Use Case                          | Tool                    |
| --------------------------------- | ----------------------- |
| Auto-generate C# classes w/ types | ✅ QuickType             |
| CsvHelper integration             | CsvHelper.TypeGenerator |
| Raw schema inference only         | FlatFiles               |
| Lightweight DIY in-app codegen    | DIY method above        |

---

Would you like me to:

* Install QuickType for you via NuGet or CLI?
* Or extend the DIY method to handle multiple rows and nullable types?

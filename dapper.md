Absolutely! Here's a **complete example** of how to **store and read application settings in a Microsoft SQL Server database using Dapper** in C#.

---

### ✅ Prerequisites

* Install Dapper via NuGet:

  ```bash
  dotnet add package Dapper
  ```
* Ensure you have a valid SQL Server connection string.

---

## 📦 `AppSetting` Model (Same as before)

```csharp
public class AppSetting
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

---

## 🗃️ MSSQL Table Schema

Run this SQL to create the table:

```sql
CREATE TABLE AppSettings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    [Key] NVARCHAR(100) NOT NULL UNIQUE,
    [Value] NVARCHAR(100),
    [Type] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(255),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
```

---

## ⚙️ Dapper Data Access Methods

```csharp
using System.Data;
using System.Data.SqlClient;
using Dapper;

public class AppSettingRepository
{
    private readonly string _connectionString;

    public AppSettingRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<AppSetting?> GetSettingByKeyAsync(string key)
    {
        using var connection = CreateConnection();
        string sql = "SELECT * FROM AppSettings WHERE [Key] = @Key";
        return await connection.QueryFirstOrDefaultAsync<AppSetting>(sql, new { Key = key });
    }

    public async Task<IEnumerable<AppSetting>> GetAllSettingsAsync()
    {
        using var connection = CreateConnection();
        string sql = "SELECT * FROM AppSettings ORDER BY [Key]";
        return await connection.QueryAsync<AppSetting>(sql);
    }

    public async Task InsertSettingAsync(AppSetting setting)
    {
        using var connection = CreateConnection();
        string sql = @"
            INSERT INTO AppSettings ([Key], [Value], [Type], [Description], CreatedAt, UpdatedAt)
            VALUES (@Key, @Value, @Type, @Description, @CreatedAt, @UpdatedAt)";
        await connection.ExecuteAsync(sql, setting);
    }

    public async Task UpdateSettingAsync(AppSetting setting)
    {
        using var connection = CreateConnection();
        string sql = @"
            UPDATE AppSettings
            SET [Value] = @Value,
                [Type] = @Type,
                [Description] = @Description,
                UpdatedAt = @UpdatedAt
            WHERE [Key] = @Key";
        await connection.ExecuteAsync(sql, setting);
    }

    public async Task UpsertSettingAsync(AppSetting setting)
    {
        var existing = await GetSettingByKeyAsync(setting.Key);
        if (existing == null)
        {
            setting.CreatedAt = DateTime.UtcNow;
            setting.UpdatedAt = DateTime.UtcNow;
            await InsertSettingAsync(setting);
        }
        else
        {
            setting.Id = existing.Id;
            setting.CreatedAt = existing.CreatedAt;
            setting.UpdatedAt = DateTime.UtcNow;
            await UpdateSettingAsync(setting);
        }
    }
}
```

---

## ✅ Example Usage

```csharp
var repo = new AppSettingRepository("YourConnectionStringHere");

// Insert or update a setting
await repo.UpsertSettingAsync(new AppSetting
{
    Key = "Theme",
    Value = "Dark",
    Type = "string",
    Description = "UI theme preference"
});

// Read a setting
var theme = await repo.GetSettingByKeyAsync("Theme");
Console.WriteLine($"Theme = {theme?.Value}");
```

---

Would you like to add **caching**, **scoped settings** (like by environment or user), or support for **JSON values** as well?

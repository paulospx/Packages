Here's a complete C# class that reads configuration from `appsettings.json` for your **Bronto** application:

### 1. First, create the configuration model class:

```csharp
// BrontoConfig.cs
using System.ComponentModel.DataAnnotations;

namespace Bronto
{
    public class BrontoConfig
    {
        [Required(ErrorMessage = "ConnectionString is required")]
        public string ConnectionString { get; set; } = string.Empty;

        [Required(ErrorMessage = "TempFilesFolder is required")]
        public string TempFilesFolder { get; set; } = string.Empty;

        [Required(ErrorMessage = "LogFilesFolder is required")]
        public string LogFilesFolder { get; set; } = string.Empty;

        // You can add more properties here as needed
        public string ApplicationName { get; set; } = "Bronto";
        
        public int DefaultTimeoutSeconds { get; set; } = 30;
        
        public bool EnableDetailedLogging { get; set; } = true;
    }
}
```

### 2. Configuration Reader / Service class:

```csharp
// BrontoConfiguration.cs
using Microsoft.Extensions.Configuration;
using System;

namespace Bronto
{
    public class BrontoConfiguration
    {
        private readonly IConfiguration _configuration;
        private readonly BrontoConfig _config;

        public BrontoConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
            _config = LoadConfiguration();
        }

        private BrontoConfig LoadConfiguration()
        {
            var config = new BrontoConfig();
            
            // Bind the "Bronto" section from appsettings.json
            _configuration.GetSection("Bronto").Bind(config);

            // Validate required settings
            ValidateConfiguration(config);

            return config;
        }

        private void ValidateConfiguration(BrontoConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.ConnectionString))
                throw new InvalidOperationException("ConnectionString is not configured in appsettings.json");

            if (string.IsNullOrWhiteSpace(config.TempFilesFolder))
                throw new InvalidOperationException("TempFilesFolder is not configured in appsettings.json");

            if (string.IsNullOrWhiteSpace(config.LogFilesFolder))
                throw new InvalidOperationException("LogFilesFolder is not configured in appsettings.json");

            // Create directories if they don't exist
            EnsureDirectoriesExist(config);
        }

        private void EnsureDirectoriesExist(BrontoConfig config)
        {
            try
            {
                Directory.CreateDirectory(config.TempFilesFolder);
                Directory.CreateDirectory(config.LogFilesFolder);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create required directories: {ex.Message}", ex);
            }
        }

        // Public properties to access config values
        public string ConnectionString => _config.ConnectionString;
        public string TempFilesFolder => _config.TempFilesFolder;
        public string LogFilesFolder => _config.LogFilesFolder;
        public string ApplicationName => _config.ApplicationName;
        public int DefaultTimeoutSeconds => _config.DefaultTimeoutSeconds;
        public bool EnableDetailedLogging => _config.EnableDetailedLogging;

        // Optional: Get the full config object
        public BrontoConfig GetConfig() => _config;
    }
}
```

### 3. Example `appsettings.json`:

```json
{
  "Bronto": {
    "ConnectionString": "Server=localhost;Database=BrontoDb;Trusted_Connection=True;TrustServerCertificate=True;",
    "TempFilesFolder": "C:\\Bronto\\Temp",
    "LogFilesFolder": "C:\\Bronto\\Logs",
    "ApplicationName": "Bronto",
    "DefaultTimeoutSeconds": 60,
    "EnableDetailedLogging": true
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  }
}
```

### 4. How to register it in `Program.cs` (Minimal API / Console / Worker Service):

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<BrontoConfiguration>();
        
        // Or if you prefer to use IOptions pattern:
        // services.Configure<BrontoConfig>(hostContext.Configuration.GetSection("Bronto"));
    });

using var host = builder.Build();
```

### Usage example:

```csharp
public class SomeService
{
    private readonly BrontoConfiguration _config;

    public SomeService(BrontoConfiguration config)
    {
        _config = config;
    }

    public void DoWork()
    {
        string connStr = _config.ConnectionString;
        string tempPath = _config.TempFilesFolder;
        
        Console.WriteLine($"Using temp folder: {tempPath}");
    }
}
```

Would you like me to also show the **IOptions<T>** version (recommended for larger apps) or add support for environment-specific settings (`appsettings.Development.json`)?

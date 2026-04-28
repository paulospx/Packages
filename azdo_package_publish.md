**Here's a complete, ready-to-use PowerShell script** that recursively scans a folder (and all subfolders) for `.nupkg` files and publishes them to an Azure Artifacts NuGet feed.

```powershell
<#
.SYNOPSIS
    Recursively finds all .nupkg files in a folder and publishes them to Azure Artifacts.

.PARAMETER FolderPath
    The root folder to scan for NuGet packages.

.PARAMETER SourceUrl
    The full v3 feed URL (e.g. https://pkgs.dev.azure.com/{org}/{project}/_packaging/{feed}/nuget/v3/index.json)

.PARAMETER ApiKey
    Any non-empty string. Azure Artifacts ignores the value but the parameter is required.
    Default: "AZ"

.PARAMETER SkipSymbols
    If specified, skips .symbols.nupkg files.

.EXAMPLE
    .\Publish-NuGetPackages.ps1 -FolderPath "C:\MyPackages" `
        -SourceUrl "https://pkgs.dev.azure.com/MyOrg/MyProject/_packaging/MyFeed/nuget/v3/index.json"
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$FolderPath,

    [Parameter(Mandatory=$true)]
    [string]$SourceUrl,

    [string]$ApiKey = "AZ",

    [switch]$SkipSymbols
)

# Validate folder exists
if (-not (Test-Path $FolderPath -PathType Container)) {
    Write-Error "Folder not found: $FolderPath"
    exit 1
}

# Find all .nupkg files recursively
$packages = Get-ChildItem -Path $FolderPath -Recurse -Filter "*.nupkg" -File

if ($SkipSymbols) {
    $packages = $packages | Where-Object { $_.Name -notlike "*.symbols.nupkg" }
}

if ($packages.Count -eq 0) {
    Write-Host "No NuGet packages found in $FolderPath" -ForegroundColor Yellow
    exit 0
}

Write-Host "Found $($packages.Count) package(s) to publish." -ForegroundColor Green

$success = 0
$failed = 0

foreach ($pkg in $packages) {
    Write-Host "`nPublishing: $($pkg.FullName)" -ForegroundColor Cyan
    
    $result = dotnet nuget push $pkg.FullName `
        --source $SourceUrl `
        --api-key $ApiKey `
        --skip-duplicate `
        --timeout 300

    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Successfully published $($pkg.Name)" -ForegroundColor Green
        $success++
    } else {
        Write-Host "❌ Failed to publish $($pkg.Name)" -ForegroundColor Red
        $failed++
    }
}

Write-Host "`n=== Publish Summary ===" -ForegroundColor White
Write-Host "Total packages     : $($packages.Count)" -ForegroundColor White
Write-Host "Successfully pushed: $success" -ForegroundColor Green
Write-Host "Failed             : $failed" -ForegroundColor $(if ($failed -gt 0) { "Red" } else { "Green" })
```

### How to use it

1. Save the script as `Publish-NuGetPackages.ps1`
2. Run it like this:

```powershell
.\Publish-NuGetPackages.ps1 -FolderPath "C:\BuildOutput" `
    -SourceUrl "https://pkgs.dev.azure.com/contoso/MyProject/_packaging/MyInternalFeed/nuget/v3/index.json"
```

### Authentication tips

- **Most common & easiest**: Use a **Personal Access Token (PAT)** with **Packaging (Read & write)** scope.  
  You can embed it in the source URL (replace the username/password part) or pre-authenticate with:

```powershell
# One-time setup (stores credentials securely)
dotnet nuget add source $SourceUrl --name MyFeed --username ANY --password YOUR_PAT
```

- Then you can call the script with just the source name instead of the full URL.

### Alternative using `nuget.exe` (if you prefer)

Replace the push line with:

```powershell
nuget push $pkg.FullName -Source $SourceUrl -ApiKey $ApiKey -SkipDuplicate -Timeout 300
```

Would you like a version that also supports:
- Parallel publishing?
- Logging to a file?
- Using a pre-configured source name instead of full URL?
- Handling symbols packages differently?

Just let me know!

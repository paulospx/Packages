# Get some code to download files from a URL
function Get-Files {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true)]
        [string]$Url,

        [Parameter(Mandatory = $true)]
        [string]$Destination
    )

    try {
        # Create destination directory if it doesn't exist
        if (-not (Test-Path -Path $Destination)) {
            New-Item -ItemType Directory -Path $Destination | Out-Null
        }

        # Download the file
        $fileName = Join-Path -Path $Destination -ChildPath (Split-Path -Path $Url -Leaf)
        Invoke-WebRequest -Uri $Url -OutFile $fileName

        Write-Host "File downloaded to: $fileName"
    } catch {
        Write-Error "Failed to download file: $_"
    }
}


# Function that get current times stamp as YYYYMMDD 
function Get-Timestamp {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $false)]
        [int]$DayOffset
    )

    return (Get-Date).AddDays($DayOffset).ToString("yyyyMMdd")
}


# Function to get timestamp as YYYYMM 
function Get-TimestampMonth {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $false)]
        [int]$MonthOffset
    )

    return (Get-Date).AddMonths($MonthOffset).ToString("yyyyMM")
}

# Function to get timestamp as quarter
function Get-TimestampQuarter {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $false)]
        [int]$QuarterOffset
    )

    $date = (Get-Date).AddMonths($QuarterOffset * 3)
    $year = $date.Year
    $quarter = [math]::Ceiling($date.Month / 3)

    return "$year-Q0$quarter".Replace("-", "")
}


# Method to upload a file to an API endpoint adding parameters in the header
# Example usage: Upload-File -FilePath "C:\path\to\file.json" -ApiEndpoint "https://api.example.com/upload"
# Note: Ensure the API endpoint accepts the file format you are uploading.  
function Send-File {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true)]
        [string]$FilePath,

        [Parameter(Mandatory = $true)]
        [string]$ApiEndpoint
    )

    try {
        $fileContent = Get-Content -Path $FilePath -Raw
        $response = Invoke-RestMethod -Uri $ApiEndpoint -Method Post -Body $fileContent -ContentType "application/json"

        Write-Host "File uploaded successfully. Response: $response"
    } catch {
        Write-Error "Failed to upload file: $_"
    }
}

# Example of Send-File function usage
# Send-File -FilePath "C:\path\to\file.json" -ApiEndpoint "https://api.example.com/upload"

# Example usage of the functions
# Uncomment the lines below to test the functions

# $tsm = Get-TimestampMonth
# Write-Host "Current month timestamp: $tsm"

# $tsq = Get-TimestampQuarter
# Write-Host "Current quarter timestamp: $tsq"

# Test Get-Timestamp
# $timestamp = Get-Timestamp -DayOffset -1
# Write-Host "Current timestamp: $timestamp"

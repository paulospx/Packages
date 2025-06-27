# Define the shared folder path and the alternate credentials
$SharedFolderPath = "\\ServerName\SharedFolder"
$Username = "Domain\Username"
$Password = "Password"

# Create a secure string for the password
$SecurePassword = ConvertTo-SecureString -String $Password -AsPlainText -Force

# Create the credential object
$Credential = New-Object System.Management.Automation.PSCredential($Username, $SecurePassword)

# Use the credential to map the shared folder temporarily
New-PSDrive -Name Z -PSProvider FileSystem -Root $SharedFolderPath -Credential $Credential -Persist

# Confirm access to the shared folder
if (Test-Path "Z:\")
{
    Write-Output "Successfully accessed the shared folder."
}
else
{
    Write-Output "Failed to access the shared folder."
}

# Optionally perform operations on the shared folder
# Example: Get all items in the shared folder
Get-ChildItem -Path "Z:\" -Recurse

# Clean up the mapped drive
Remove-PSDrive -Name Z

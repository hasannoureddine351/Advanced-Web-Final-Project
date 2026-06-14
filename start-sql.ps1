# Run this script in PowerShell AS ADMINISTRATOR to start SQL Server Express
Write-Host "Starting SQL Server Express..."
Start-Service -Name 'MSSQL$SQLEXPRESS'
Get-Service -Name 'MSSQL$SQLEXPRESS' | Format-Table Name, Status

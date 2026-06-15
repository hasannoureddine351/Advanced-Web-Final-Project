Start-Service -Name 'MSSQL$SQLEXPRESS'
Get-Service -Name 'MSSQL$SQLEXPRESS' | Format-Table Name, Status

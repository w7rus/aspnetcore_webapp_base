param([string]$migrationName='Default') 
$ProjectPath = Join-Path $PSScriptRoot "..\DAL"
dotnet ef migrations add ${migrationName} --project "${ProjectPath}"
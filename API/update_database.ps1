param([string]$migrationName='Default') 
$ProjectPath = Join-Path $PSScriptRoot "..\DAL"
dotnet ef database update ${migrationName} --project "${ProjectPath}"
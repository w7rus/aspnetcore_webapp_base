$ProjectPath = Join-Path $PSScriptRoot "..\DAL"
dotnet ef migrations remove --project "${ProjectPath}"
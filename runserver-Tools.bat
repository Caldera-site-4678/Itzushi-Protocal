@echo off
dotnet build --configuration Tools
dotnet run --project Content.Itzushi.Server --configuration Tools
pause

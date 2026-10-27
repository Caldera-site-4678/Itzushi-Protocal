@echo off
dotnet build --configuration Tools
dotnet run --project Content.Itzushi.Client --configuration Tools

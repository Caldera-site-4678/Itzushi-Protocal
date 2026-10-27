#!/usr/bin/env bash
dotnet build --configuration Tools
dotnet run --project Content.Itzushi.Server --configuration Tools
read -p "Press enter to continue"

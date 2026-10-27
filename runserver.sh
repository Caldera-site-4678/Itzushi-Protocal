#!/usr/bin/env bash
dotnet build
dotnet run --project Content.Itzushi.Server
read -p "Press enter to continue"

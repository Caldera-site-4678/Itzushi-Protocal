#!/usr/bin/env bash
dotnet build
dotnet run --project Content.Itzushi.Client
read -p "Press enter to continue"

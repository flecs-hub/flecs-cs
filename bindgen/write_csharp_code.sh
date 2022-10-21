#!/bin/bash
DIRECTORY="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

dotnet build "$DIRECTORY/../src/cs/production/Flecs.Bindgen/Flecs.Bindgen.csproj" -p:OutputPath="$DIRECTORY/plugins/Flecs.Bindgen"
c2cs cs
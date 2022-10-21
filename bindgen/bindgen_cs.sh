#!/bin/bash
DIRECTORY="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

dotnet build "$DIRECTORY/../src/cs/production/Flecs.Bindgen/Flecs.Bindgen.csproj" -p:OutputPath="$DIRECTORY/plugins/Flecs.Bindgen"
/Users/lstranks/.dotnet/tools/c2cs cs
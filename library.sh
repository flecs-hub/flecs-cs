#!/bin/bash
DIRECTORY="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"

c2cs library --config "./bindgen/config-library.json"
	
if [[ -z "$1" ]]; then
    read
fi
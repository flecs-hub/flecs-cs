#!/bin/bash
DIRECTORY="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
if [[ -z $SCRIPTS_DIRECTORY ]]; then
    SCRIPTS_DIRECTORY="$DIRECTORY/ext/scripts"
    git clone "https://github.com/bottlenoselabs/scripts" "$SCRIPTS_DIRECTORY" 2> /dev/null 1> /dev/null || git -C "$SCRIPTS_DIRECTORY" pull 1> /dev/null
fi

$SCRIPTS_DIRECTORY/c/library/main.sh \
    $DIRECTORY/src/c/production/flecs \
    $DIRECTORY/build \
    $DIRECTORY/lib \
    "flecs" \
    "flecs" \
    "" \
    "" \
	
if [[ -z "$1" ]]; then
    read
fi
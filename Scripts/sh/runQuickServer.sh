#!/usr/bin/env sh

# make sure to start from script dir
if [ "$(dirname $0)" != "." ]; then
    cd "$(dirname $0)"
fi

cd ../../
sh -e Scripts/sh/pklEval.sh
dotnet run --project Content.Server --no-build

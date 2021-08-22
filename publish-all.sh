#!/bin/bash

script_path="$(dirname "$(readlink -f "${BASH_SOURCE[0]}")")"

set -e # break on error.
cd $script_path # cd to script location.

echo Building in Release mode...
dotnet build -c Release -v q

echo Running all tests...
ls ./test/*.csproj | xargs -L1 -P 0 dotnet test -c Release -v q --no-build

cd ./src

echo Cleaning previous publish output...
rm -rf ./bin/Release/net6/publish

$script_path/publish-frontend.sh

echo Publishing API...
dotnet publish -c Release -v q --no-build
cd ./bin/Release/net6/publish/

echo Copying files...
rsync -ru --progress --exclude="wwwroot" --exclude="logs" ./* jbfp@jbfp.dk:/opt/sequence

cd $script_path

echo Restarting server processes...
ssh -t jbfp@jbfp.dk "sudo systemctl restart sequence" || true

echo Done!

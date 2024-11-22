#! /bin/bash

echo "Searching registration server"
nuget_registration_server=$(curl https://api.nuget.org/v3/index.json | jq -r '.resources[] | select(.["@type"] | startswith("RegistrationsBaseUrl")) | .["@id"]' | head -n 1)

echo "Using $nuget_registration_server"

echo "Searching & building project files"
project_files=$(find . -name "*.csproj")

for project in $project_files; do
    #echo "Processing $project"

    # Extract project name
    project_name=$(basename "$project" .csproj)

    # Extract version
    project_version=$(grep -oPm1 "(?<=<Version>)[^<]+" "$project")
    if [ -z "$project_version" ]; then
        echo "No <Version> tag found in $project, skipping."
        continue
    fi

    #echo "Detected version $project_version for $project_name"

    meta_url="${nuget_registration_server}${project_name,,}/${project_version}.json"

    # Check if version exists on NuGet
    if curl --silent --fail $meta_url > /dev/null ; then
        echo "Version $project_version of $project_name already exists on NuGet."
    else
        echo "Publishing $project_name version $project_version to NuGet."

        pwd=$(pwd)
        project_path=$(dirname $project)
        (cd $project_path; dotnet build --configuration Release; dotnet pack --configuration Release --output $pwd/nupkgs)
    fi
done

dotnet nuget push ./nupkgs/*.nupkg -k $NUGET_API_KEY -s https://api.nuget.org/v3/index.json

rm -f ./nupkgs/*.nupkg
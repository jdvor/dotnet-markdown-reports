#!/usr/bin/env bash

target='publish'
semantic_version='1.0.0'
version_suffix=''
rid='linux-x64'
push=''
archive=''

while [[ "$#" -gt 0 ]]; do
    case $1 in
        -t|--target) target="$2"; shift ;;
        -v|--semantic-version) semantic_version="$2"; shift ;;
        -s|--version-suffix) version_suffix="$2"; shift ;;
        -r|--rid) rid="$2"; shift ;;
        -p|--push) push="$2"; shift ;;
        -z|--archive) archive='true' ;;
        *) echo "Unknown parameter passed: $1"; exit 1 ;;
    esac
    shift
done

publish_dir='./artifacts/publish'
pack_dir='./artifacts/pack'

if [[ "$target" == *clean* ]]; then
    rm -rf ./artifacts
fi

if [[ "$target" == *publish* ]]; then
    rm -rf "$publish_dir"
    dotnet publish src/DotnetMarkdownReports.App/DotnetMarkdownReports.App.csproj \
        -p:RunAnalyzers=false -p:AnalysisMode=None -clp:NoSummary --nologo -v minimal \
        -c Release \
        -r "$rid" \
        -o "$publish_dir" \
        -p:PublishTrimmed=true \
        -p:TrimMode=partial \
        -p:PublishSingleFile=true \
        --self-contained \
        -p:VersionPrefix="$semantic_version" -p:VersionSuffix="$version_suffix"
    if [ -n "$archive" ]; then
        tar -C "$publish_dir" -czf "$publish_dir/mdreport_$semantic_version.tar.gz" mdreport
    fi
    ls -Alh "$publish_dir"
fi

if [[ "$target" == *pack* ]]; then
    rm -rf "$pack_dir"
    dotnet pack src/DotnetMarkdownReports.App/DotnetMarkdownReports.App.csproj \
        -p:RunAnalyzers=false -p:AnalysisMode=None -clp:NoSummary --nologo -v minimal \
        -c Release \
        -o "$pack_dir" \
        -p:VersionPrefix="$semantic_version" -p:VersionSuffix="$version_suffix"

    readarray -d '' packages < <(find "$pack_dir" -type f -name "*.nupkg" -print0)
    if (( ${#packages[@]} == 0 )); then
        echo "No packages created."
        exit 0
    fi
    ls -Alh "$pack_dir"

    if [ -n "$push" ]; then
        dotnet nuget push "$pack_dir/*.nupkg" --api-key "$push" --skip-duplicate
    fi
fi

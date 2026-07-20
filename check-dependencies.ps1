[CmdletBinding()]
param(
    [string]$RepositoryRoot = ".",
    [string]$DependenciesFile = "DEPENDENCIES.md"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Get-DependenciesFromMarkdown {
    param([string]$Path)

    $dependencies = @{}

    foreach ($line in Get-Content $Path) {

        if (-not $line.StartsWith("|")) {
            continue
        }

        if ($line -match '^\|\s*Library\s*\|' -or
            $line -match '^\|\s*-+') {
            continue
        }

        $columns = ($line -split '\|').ForEach({ $_.Trim() })

        if ($columns.Count -lt 4) {
            continue
        }

        $name = $columns[1]
        $version = $columns[2]

        if ([string]::IsNullOrWhiteSpace($name) -or [string]::IsNullOrWhiteSpace($version)) {
            continue
        }

        $dependencies[$name.ToLowerInvariant()] = $version
    }

    return $dependencies
}

function Get-PackageReferences {
    param([string]$RepositoryRoot)

    $result = @()

    foreach ($project in Get-ChildItem $RepositoryRoot -Recurse -Filter *.csproj) {

        [xml]$xml = Get-Content $project.FullName -Raw

        $packageReferences = $xml.SelectNodes(
            "//*[local-name()='PackageReference']"
        )

        foreach ($reference in $packageReferences) {

            $version = $reference.Version

            if (-not $version) {
                $versionNode = $reference.SelectSingleNode("*[local-name()='Version']")
                if ($versionNode) {
                    $version = $versionNode.InnerText
                }
            }

            $result += [PSCustomObject]@{
                Package = $reference.Include
                Version = $version
                Source  = $project.FullName
            }
        }
    }

    foreach ($packagesConfig in Get-ChildItem $RepositoryRoot -Recurse -Filter packages.config) {

        [xml]$xml = Get-Content $packagesConfig.FullName -Raw

        foreach ($package in $xml.packages.package) {
            $result += [PSCustomObject]@{
                Package = $package.id
                Version = $package.version
                Source  = $packagesConfig.FullName
            }
        }
    }

    return $result
}

$expected = Get-DependenciesFromMarkdown $DependenciesFile
$packages = Get-PackageReferences $RepositoryRoot

$issues = @()

foreach ($package in $packages) {

    $key = $package.Package.ToLowerInvariant()

    if (-not $expected.ContainsKey($key)) {

        $issues += [PSCustomObject]@{
            Type     = "Missing"
            Package  = $package.Package
            Expected = ""
            Actual   = $package.Version
            Source   = $package.Source
        }

        continue
    }

    if ($expected[$key] -ne $package.Version) {

        $issues += [PSCustomObject]@{
            Type     = "VersionMismatch"
            Package  = $package.Package
            Expected = $expected[$key]
            Actual   = $package.Version
            Source   = $package.Source
        }
    }
}

if ($issues.Count -eq 0) {
    Write-Host "Dependency check passed." -ForegroundColor Green
    exit 0
}

$issues |
    Sort-Object Type, Package |
    Format-Table Type, Package, Expected, Actual, Source -Wrap -AutoSize

exit 1
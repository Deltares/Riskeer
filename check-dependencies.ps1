[CmdletBinding()]
param(
    [string]$RepositoryRoot = ".",
    [string]$DependenciesFile = "DEPENDENCIES.md",
	[string]$LicensesFolder = "licenses"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function ConvertTo-NormalizedIdentifier {
    param([string]$Value)

    if ([string]::IsNullOrWhiteSpace($Value)) {
        return ""
    }

    return (($Value.ToLowerInvariant() -replace '[^a-z0-9]', ''))
}

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

function Get-MissingLicenseFiles {
    param(
        [hashtable]$Dependencies,
        [string]$LicensesFolder
    )

    $licenseFiles = @{}

    Get-ChildItem $LicensesFolder -Recurse -File | ForEach-Object {
        $licenseFiles[$_.Name.ToLowerInvariant()] = $true
    }

    $issues = @()

    foreach ($packageName in $Dependencies.Keys) {
        $expectedFile = "$packageName.LICENSE"

        if (-not $licenseFiles.ContainsKey($expectedFile.ToLowerInvariant())) {
            $issues += [PSCustomObject]@{
                Type     = "MissingLicense"
                Package  = $packageName
                Expected = $expectedFile
                Actual   = ""
                Source   = $LicensesFolder
            }
        }
    }

    return $issues
}

function Get-UnusedLicenseFiles {
    param(
        [hashtable]$Dependencies,
        [string]$LicensesFolder
    )

    $expectedLicenseFiles = @{}

    foreach ($packageName in $Dependencies.Keys) {
        $expectedLicenseFiles["$packageName.LICENSE".ToLowerInvariant()] = $true
    }

    $issues = @()

    Get-ChildItem $LicensesFolder -Recurse -File | ForEach-Object {
        $licenseFileName = $_.Name.ToLowerInvariant()

        if (-not $expectedLicenseFiles.ContainsKey($licenseFileName)) {
            $issues += [PSCustomObject]@{
                Type     = "UnusedLicense"
                Package  = $_.Name
                Expected = ""
                Actual   = $_.FullName
                Source   = $LicensesFolder
            }
        }
    }

    return $issues
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

function Get-UnusedDependencies {
    param(
        [hashtable]$Dependencies,
        [object[]]$Packages,
        [string]$RepositoryRoot,
        [string]$DependenciesFile
    )

    $usedPackages = @{}
    $usageTokens = @{}

    foreach ($package in $Packages) {
        if (-not [string]::IsNullOrWhiteSpace($package.Package)) {
            $usedPackages[(ConvertTo-NormalizedIdentifier $package.Package)] = $true
        }
    }

    foreach ($project in Get-ChildItem $RepositoryRoot -Recurse -Filter *.csproj) {
        [xml]$xml = Get-Content $project.FullName -Raw

        $referenceNodes = $xml.SelectNodes(
            "//*[local-name()='ProjectReference' or local-name()='Reference' or local-name()='PackageReference' or local-name()='HintPath']"
        )

        foreach ($node in $referenceNodes) {
            $value = ""

            if ($node.LocalName -eq "HintPath") {
                $value = $node.InnerText
            } else {
                $value = $node.Include
            }

            if ([string]::IsNullOrWhiteSpace($value)) {
                continue
            }

            foreach ($token in ($value -split '[^A-Za-z0-9]+')) {
                $normalizedToken = ConvertTo-NormalizedIdentifier $token
                if (-not [string]::IsNullOrWhiteSpace($normalizedToken)) {
                    $usageTokens[$normalizedToken] = $true
                }
            }
        }
    }

    $issues = @()

    foreach ($dependencyName in $Dependencies.Keys) {
        $normalizedDependency = ConvertTo-NormalizedIdentifier $dependencyName

        $isUsed = $usedPackages.ContainsKey($normalizedDependency)

        if (-not $isUsed -and $normalizedDependency.Length -ge 5) {
            foreach ($token in $usageTokens.Keys) {
                if ($token.Contains($normalizedDependency)) {
                    $isUsed = $true
                    break
                }
            }
        }

        if (-not $isUsed) {
            $issues += [PSCustomObject]@{
                Type     = "UnusedDependency"
                Package  = $dependencyName
                Expected = $Dependencies[$dependencyName]
                Actual   = ""
                Source   = $DependenciesFile
            }
        }
    }

    return $issues
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

$issues += Get-UnusedDependencies -Dependencies $expected -Packages $packages -RepositoryRoot $RepositoryRoot -DependenciesFile $DependenciesFile
$issues += Get-MissingLicenseFiles -Dependencies $expected -LicensesFolder $LicensesFolder
$issues += Get-UnusedLicenseFiles -Dependencies $expected -LicensesFolder $LicensesFolder

if ($issues.Count -eq 0) {
    Write-Host "Dependency check passed." -ForegroundColor Green
    exit 0
}

$issues |
    Sort-Object Type, Package |
    Format-Table Type, Package, Expected, Actual, Source -Wrap -AutoSize

exit 1
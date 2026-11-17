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

function Get-ProjectNameFromSourcePath {
    param([string]$SourcePath)

    if ([string]::IsNullOrWhiteSpace($SourcePath)) {
        return ""
    }

    $fileName = [System.IO.Path]::GetFileName($SourcePath)
    $extension = [System.IO.Path]::GetExtension($SourcePath).ToLowerInvariant()

    if ($extension -eq ".csproj" -or $extension -eq ".wixproj") {
        return [System.IO.Path]::GetFileNameWithoutExtension($SourcePath)
    }

    if ($extension -eq ".targets" -or $extension -eq ".props") {
        return $fileName
    }

    if ($fileName.ToLowerInvariant() -eq "packages.config") {
        return [System.IO.Path]::GetFileName([System.IO.Path]::GetDirectoryName($SourcePath))
    }

    return ""
}

function Get-MsBuildFiles {
    param([string]$RepositoryRoot)

    $ignoredFolderPattern = '\\(bin|obj|packages|TestResults)\\'

    return @(Get-ChildItem $RepositoryRoot -Recurse -File -Include *.csproj, *.wixproj, *.targets, *.props |
        Where-Object { $_.FullName -notmatch $ignoredFolderPattern })
}

# Output folders that are not named after the package that provides their content.
$outputFolderAliases = @{
    "System.Data.SQLite" = "Stub.System.Data.SQLite.Core.NetStandard"
}

# Version constants in source code that must match the version in the dependencies file.
$sourceVersionConstants = @(
    [PSCustomObject]@{
        Library  = "Hydra-Ring"
        Path     = "Riskeer\HydraRing\src\Riskeer.HydraRing.Calculation\HydraRingFileConstants.cs"
        Constant = "HydraRingVersionNumber"
    }
)

# Text that must be present in the license file for each license in the dependencies file.
# GNU licenses are identified separately, because their texts refer to each other.
$licenseTextPatterns = @{
    "Apache-2.0"     = 'Apache License'
    "BSD-3-Clause"   = 'Neither the name'
    "MIT"            = 'Permission is hereby granted, free of charge'
    "MS-NET-Library" = 'MICROSOFT \.NET LIBRARY'
    "MS-PL"          = 'Microsoft Public License'
    "MS-RL"          = 'Microsoft Reciprocal License'
    "Proprietary"    = ''
    "Public domain"  = 'public domain'
}

$gnuLicenses = @{
    ""       = "GPL-3.0"
    "lesser" = "LGPL-3.0"
    "affero" = "AGPL-3.0"
}

function Get-DependencyRowsFromMarkdown {
    param([string]$Path)

    $rows = @()

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

        $rows += [PSCustomObject]@{
            Name    = $name
            Version = $version
            License = $columns[3]
        }
    }

    return $rows
}

function Get-DependenciesFromMarkdown {
    param([string]$Path)

    # Keys keep the casing of the dependencies file; lookups are case-insensitive.
    $dependencies = @{}

    foreach ($row in Get-DependencyRowsFromMarkdown $Path) {
        $dependencies[$row.Name] = $row.Version
    }

    return $dependencies
}

function Get-LicenseFilesByName {
    param([string]$LicensesFolder)

    # Case-insensitive lookup of all files per name, so casing and duplicates across subfolders can be verified.
    $licenseFiles = @{}

    Get-ChildItem $LicensesFolder -Recurse -File | ForEach-Object {
        if (-not $licenseFiles.ContainsKey($_.Name)) {
            $licenseFiles[$_.Name] = @()
        }

        $licenseFiles[$_.Name] += $_
    }

    return $licenseFiles
}

function Get-MissingLicenseFiles {
    param(
        [hashtable]$Dependencies,
        [string]$LicensesFolder
    )

    $licenseFiles = Get-LicenseFilesByName $LicensesFolder

    $issues = @()

    foreach ($packageName in $Dependencies.Keys) {
        $expectedFile = "$packageName.LICENSE"

        if (-not $licenseFiles.ContainsKey($expectedFile)) {
            $issues += [PSCustomObject]@{
                Type     = "MissingLicense"
                Package  = $packageName
                Expected = $expectedFile
                Actual   = ""
                Source   = $LicensesFolder
            }
            continue
        }

        $actualFiles = @($licenseFiles[$expectedFile])

        if ($actualFiles.Count -gt 1) {
            $issues += [PSCustomObject]@{
                Type     = "DuplicateLicense"
                Package  = $packageName
                Expected = $expectedFile
                Actual   = (@($actualFiles | ForEach-Object { $_.FullName }) -join ", ")
                Source   = $LicensesFolder
            }
        }

        foreach ($actualFile in $actualFiles) {
            if ($actualFile.Name -cne $expectedFile) {
                $issues += [PSCustomObject]@{
                    Type     = "LicenseFileNameCase"
                    Package  = $packageName
                    Expected = $expectedFile
                    Actual   = $actualFile.Name
                    Source   = $actualFile.FullName
                }
            }
        }
    }

    return $issues
}

function Get-LicenseTextIssues {
    param(
        [object[]]$DependencyRows,
        [string]$LicensesFolder,
        [string]$DependenciesFile
    )

    $licenseFiles = Get-LicenseFilesByName $LicensesFolder

    $issues = @()

    foreach ($row in $DependencyRows) {
        $isGnuLicense = $gnuLicenses.Values -ccontains $row.License

        if (-not $isGnuLicense -and -not $licenseTextPatterns.ContainsKey($row.License)) {
            $issues += [PSCustomObject]@{
                Type     = "UnknownLicense"
                Package  = $row.Name
                Expected = (@($licenseTextPatterns.Keys) + @($gnuLicenses.Values) | Sort-Object) -join ", "
                Actual   = $row.License
                Source   = $DependenciesFile
            }
            continue
        }

        # Missing license files are reported by Get-MissingLicenseFiles.
        $licenseFilesForRow = @()
        if ($licenseFiles.ContainsKey("$($row.Name).LICENSE")) {
            $licenseFilesForRow = @($licenseFiles["$($row.Name).LICENSE"])
        }

        foreach ($licenseFile in $licenseFilesForRow) {
            $licenseText = Get-Content $licenseFile.FullName -Raw

            if ($isGnuLicense) {
                # The first GNU license mentioned identifies the license; the full texts refer to the other GNU licenses.
                # Version 3 must directly follow it, either as the header of the full text ("... LICENSE Version 3")
                # or as in the license notice ("... License as published by the Free Software Foundation, either version 3").
                $actualLicense = "Not a GNU version 3 license"
                $gnuMatch = [regex]::Match(
                    $licenseText,
                    'GNU\s+(?:(?<type>Affero|Lesser)\s+)?General\s+Public\s+License(?<version>[\s,]*(?:as\s+published\s+by\s+the\s+Free\s+Software\s+Foundation,?\s+)?(?:either\s+)?version\s+3\b)?',
                    'IgnoreCase')

                if ($gnuMatch.Success -and $gnuMatch.Groups["version"].Success) {
                    $actualLicense = $gnuLicenses[$gnuMatch.Groups["type"].Value.ToLowerInvariant()]
                }

                if ($actualLicense -cne $row.License) {
                    $issues += [PSCustomObject]@{
                        Type     = "LicenseTextMismatch"
                        Package  = $row.Name
                        Expected = $row.License
                        Actual   = $actualLicense
                        Source   = $licenseFile.FullName
                    }
                }

                continue
            }

            $pattern = $licenseTextPatterns[$row.License]

            if (-not [string]::IsNullOrEmpty($pattern) -and $licenseText -notmatch $pattern) {
                $issues += [PSCustomObject]@{
                    Type     = "LicenseTextMismatch"
                    Package  = $row.Name
                    Expected = $row.License
                    Actual   = "Text '$pattern' not found"
                    Source   = $licenseFile.FullName
                }
            }
        }
    }

    return $issues
}

function Get-OutputFolderIssues {
    param(
        [hashtable]$Dependencies,
        [string]$RepositoryRoot,
        [string]$DependenciesFile
    )

    # Matches versioned output folders like $(SolutionDir)bin\$(Configuration)\Application\Built-in\Managed\Third-party\<Library>-<Version>,
    # $(OutDir)Application\Built-in\... and ...\Application\Standalone\Deltares\<Library>-<Version>.
    $outputFolderPattern = '(?:\\|\))Application\\(?:Built-in\\[^\\&"]+\\[^\\&"]+|Standalone\\[^\\&"]+)\\(?<name>[^\\&"*]+?)-(?<version>\d+(?:\.\d+)+)(?=\\|&quot;|"|$)'

    $issues = @()

    foreach ($project in Get-MsBuildFiles $RepositoryRoot) {
        $content = Get-Content $project.FullName -Raw
        $outputFolders = @([regex]::Matches($content, $outputFolderPattern) |
            ForEach-Object { [PSCustomObject]@{ Name = $_.Groups["name"].Value; Version = $_.Groups["version"].Value } } |
            Sort-Object Name, Version -Unique)

        foreach ($outputFolder in $outputFolders) {
            $folderName = "$($outputFolder.Name)-$($outputFolder.Version)"
            $dependencyName = $outputFolder.Name

            if ($outputFolderAliases.ContainsKey($dependencyName)) {
                $dependencyName = $outputFolderAliases[$dependencyName]
            }

            $matchingDependency = @($Dependencies.Keys | Where-Object { $_ -eq $dependencyName }) | Select-Object -First 1

            if (-not $matchingDependency) {
                $issues += [PSCustomObject]@{
                    Type     = "UnknownOutputFolder"
                    Package  = $outputFolder.Name
                    Expected = "Library in $DependenciesFile"
                    Actual   = $folderName
                    Source   = $project.FullName
                }
                continue
            }

            if (-not $outputFolderAliases.ContainsKey($outputFolder.Name) -and $matchingDependency -cne $outputFolder.Name) {
                $issues += [PSCustomObject]@{
                    Type     = "OutputFolderNameCase"
                    Package  = $matchingDependency
                    Expected = "$matchingDependency-$($outputFolder.Version)"
                    Actual   = $folderName
                    Source   = $project.FullName
                }
            }

            if ($Dependencies[$matchingDependency] -ne $outputFolder.Version) {
                $issues += [PSCustomObject]@{
                    Type     = "OutputFolderVersionMismatch"
                    Package  = $matchingDependency
                    Expected = $Dependencies[$matchingDependency]
                    Actual   = $folderName
                    Source   = $project.FullName
                }
            }
        }
    }

    return $issues
}

function Get-SourceVersionConstantIssues {
    param(
        [hashtable]$Dependencies,
        [string]$RepositoryRoot
    )

    $issues = @()

    foreach ($sourceVersionConstant in $sourceVersionConstants) {
        $path = Join-Path $RepositoryRoot $sourceVersionConstant.Path
        $actualVersion = ""

        if (Test-Path $path) {
            $constantPattern = "const\s+string\s+$([regex]::Escape($sourceVersionConstant.Constant))\s*=\s*""([^""]*)"""
            $constantMatch = [regex]::Match((Get-Content $path -Raw), $constantPattern)

            if ($constantMatch.Success) {
                $actualVersion = $constantMatch.Groups[1].Value
            }
        }

        $expectedVersion = $Dependencies[$sourceVersionConstant.Library]

        if ([string]::IsNullOrEmpty($actualVersion) -or $actualVersion -ne $expectedVersion) {
            $issues += [PSCustomObject]@{
                Type     = "SourceVersionMismatch"
                Package  = $sourceVersionConstant.Library
                Expected = $expectedVersion
                Actual   = "$($sourceVersionConstant.Constant) = $actualVersion"
                Source   = $path
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

    # Hashtable keys are case-insensitive; casing of license file names is checked by Get-MissingLicenseFiles.
    $expectedLicenseFiles = @{}

    foreach ($packageName in $Dependencies.Keys) {
        $expectedLicenseFiles["$packageName.LICENSE"] = $true
    }

    $issues = @()

    Get-ChildItem $LicensesFolder -Recurse -File | ForEach-Object {
        if (-not $expectedLicenseFiles.ContainsKey($_.Name)) {
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

    foreach ($project in Get-MsBuildFiles $RepositoryRoot) {

        [xml]$xml = Get-Content $project.FullName -Raw

        # Versioned MSBuild SDKs (e.g. <Project Sdk="WixToolset.Sdk/5.0.2">) are NuGet packages as well.
        $sdkNodes = $xml.SelectNodes(
            "/*[local-name()='Project' and @Sdk] | //*[local-name()='Sdk' and @Name] | //*[local-name()='Import' and @Sdk]"
        )

        foreach ($sdkNode in $sdkNodes) {
            $sdkValues = @()

            if ($sdkNode.LocalName -eq "Sdk") {
                $sdkVersion = $sdkNode.GetAttribute("Version")
                if (-not [string]::IsNullOrWhiteSpace($sdkVersion)) {
                    $sdkValues += "$($sdkNode.GetAttribute("Name"))/$sdkVersion"
                }
            } else {
                $sdkValues += ($sdkNode.GetAttribute("Sdk") -split ';')
            }

            foreach ($sdkValue in $sdkValues) {
                $sdkParts = $sdkValue.Trim() -split '/'

                if ($sdkParts.Count -ne 2 -or [string]::IsNullOrWhiteSpace($sdkParts[1])) {
                    continue
                }

                $result += [PSCustomObject]@{
                    Package = $sdkParts[0].Trim()
                    Version = $sdkParts[1].Trim()
                    Source  = $project.FullName
                }
            }
        }

        $packageReferences = $xml.SelectNodes(
            "//*[local-name()='PackageReference' and @Include]"
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

function Get-ExpectedProjectLicenseHeader {
    param(
        [string]$ProjectPath,
        [string]$RepositoryRoot
    )

    $normalizedRepositoryRoot = [System.IO.Path]::GetFullPath($RepositoryRoot).TrimEnd('\')
    $normalizedProjectPath = [System.IO.Path]::GetFullPath($ProjectPath)
    $relativePath = $normalizedProjectPath.Substring($normalizedRepositoryRoot.Length).TrimStart('\')
    $topLevelFolder = ($relativePath -split '\\')[0]

    if ($topLevelFolder -eq "Application" -or $topLevelFolder -eq "Riskeer") {
        return "Copying.licenseheader"
    }

    if ($topLevelFolder -eq "Core" -or $topLevelFolder -eq "Migration") {
        return "Copying.Lesser.licenseheader"
    }

    return ""
}

function Get-ProjectLicenseHeaderIssues {
    param([string]$RepositoryRoot)

    $issues = @()

    foreach ($project in Get-ChildItem $RepositoryRoot -Recurse -Filter *.csproj) {
        $expectedLicenseHeader = Get-ExpectedProjectLicenseHeader -ProjectPath $project.FullName -RepositoryRoot $RepositoryRoot

        if ([string]::IsNullOrWhiteSpace($expectedLicenseHeader)) {
            continue
        }

        [xml]$xml = Get-Content $project.FullName -Raw

        $licenseHeaders = @()
        $itemNodes = $xml.SelectNodes(
            "//*[local-name()='None' or local-name()='Content']"
        )

        foreach ($itemNode in $itemNodes) {
            $valuesToInspect = @()

            $includeAttribute = $itemNode.Attributes["Include"]
            if ($includeAttribute -and -not [string]::IsNullOrWhiteSpace($includeAttribute.Value)) {
                $valuesToInspect += $includeAttribute.Value
            }

            $linkNode = $itemNode.SelectSingleNode("*[local-name()='Link']")
            if ($linkNode -and -not [string]::IsNullOrWhiteSpace($linkNode.InnerText)) {
                $valuesToInspect += $linkNode.InnerText
            }

            foreach ($value in $valuesToInspect) {
                if ($value -match '(Copying(?:\.Lesser)?\.licenseheader)') {
                    $licenseHeaders += $Matches[1]
                }
            }
        }

        $licenseHeaders = @($licenseHeaders | Select-Object -Unique)

        if ($licenseHeaders.Count -eq 0) {
            $issues += [PSCustomObject]@{
                Type     = "MissingProjectLicenseHeader"
                Package  = $project.Name
                Expected = $expectedLicenseHeader
                Actual   = ""
                Source   = $project.FullName
            }
            continue
        }

        if ($licenseHeaders -notcontains $expectedLicenseHeader) {
            $issues += [PSCustomObject]@{
                Type     = "WrongProjectLicenseHeader"
                Package  = $project.Name
                Expected = $expectedLicenseHeader
                Actual   = ($licenseHeaders -join ", ")
                Source   = $project.FullName
            }
        }
    }

    return $issues
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

    foreach ($project in Get-MsBuildFiles $RepositoryRoot) {
        [xml]$xml = Get-Content $project.FullName -Raw

        $referenceNodes = $xml.SelectNodes(
            "//*[local-name()='ProjectReference' or local-name()='Reference' or local-name()='PackageReference' or local-name()='HintPath']"
        )

        foreach ($node in $referenceNodes) {
            $value = ""

            if ($node.LocalName -eq "HintPath") {
                $value = $node.InnerText
            } else {
                $value = $node.GetAttribute("Include")
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
            $dependencyParts = @()

            foreach ($part in ($dependencyName -split '[^A-Za-z0-9]+')) {
                $normalizedPart = ConvertTo-NormalizedIdentifier $part
                if ($normalizedPart.Length -lt 4) {
                    continue
                }

                $dependencyParts += $normalizedPart
            }

            $dependencyParts = @($dependencyParts | Select-Object -Unique)

            foreach ($dependencyPart in $dependencyParts) {
                foreach ($token in $usageTokens.Keys) {
                    if ($token.Contains($dependencyPart)) {
                        $isUsed = $true
                        break
                    }
                }

                if ($isUsed) {
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

$dependencyRows = Get-DependencyRowsFromMarkdown $DependenciesFile
$expected = Get-DependenciesFromMarkdown $DependenciesFile
$packages = Get-PackageReferences $RepositoryRoot

$issues = @()

foreach ($package in $packages) {

    $key = $package.Package

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

    # The lookup is case-insensitive; the name must still match the casing in the dependencies file.
    $dependencyName = @($expected.Keys | Where-Object { $_ -eq $key })[0]

    if ($dependencyName -cne $package.Package) {

        $issues += [PSCustomObject]@{
            Type     = "PackageNameCase"
            Package  = $package.Package
            Expected = $dependencyName
            Actual   = $package.Package
            Source   = $package.Source
        }
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
$issues += Get-LicenseTextIssues -DependencyRows $dependencyRows -LicensesFolder $LicensesFolder -DependenciesFile $DependenciesFile
$issues += Get-OutputFolderIssues -Dependencies $expected -RepositoryRoot $RepositoryRoot -DependenciesFile $DependenciesFile
$issues += Get-SourceVersionConstantIssues -Dependencies $expected -RepositoryRoot $RepositoryRoot
$issues += Get-ProjectLicenseHeaderIssues -RepositoryRoot $RepositoryRoot

if ($issues.Count -eq 0) {
    Write-Host "Dependency check passed." -ForegroundColor Green
    exit 0
}

$issues |
    Sort-Object Type, Package |
    Format-Table Type,
                 Package,
                 @{Label = "Project"; Expression = { Get-ProjectNameFromSourcePath -SourcePath $_.Source }},
                 Expected,
                 Actual,
                 Source -Wrap -AutoSize

exit 1

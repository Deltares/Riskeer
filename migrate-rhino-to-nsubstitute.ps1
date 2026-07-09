#!/usr/bin/env pwsh
<#
.SYNOPSIS
Migrate test files from Rhino.Mocks to NSubstitute following the approved rulebook
.PARAMETER FilePath
Path to the C# test file to migrate
#>
param(
    [Parameter(Mandatory=$true)]
    [string]$FilePath
)

# Read the file with UTF8 encoding
$content = Get-Content -Path $FilePath -Encoding UTF8 -Raw

# Skip if already migrated
if ($content -notlike "*Rhino.Mocks*" -and $content -like "*NSubstitute*") {
    Write-Host "Already migrated: $FilePath" -ForegroundColor Green
    return
}

# Check if has NSubstitute already
$hasNSubstitute = $content -like "*using NSubstitute*"

# Phase 1: Replace using statements
$content = $content -replace "using Rhino\.Mocks;", ""
if (-not $hasNSubstitute) {
    # Add NSubstitute using after NUnit if present, otherwise at end of using block
    if ($content -like "*using NUnit.Framework;*") {
        $content = $content -replace "(using NUnit\.Framework;)", "`$1`nusing NSubstitute;"
    } elseif ($content -like "*using Core.Common*") {
        # Find last using Core.Common statement and add after it
        $content = $content -replace "(using Core\.Common[^;]*;)", "`$1`nusing NSubstitute;"
    } else {
        # Add after last using statement
        $lastUsingMatch = [regex]::Matches($content, "using [^;]+;") | Select-Object -Last 1
        if ($lastUsingMatch) {
            $lastUsing = $lastUsingMatch.Value
            $content = $content -replace [regex]::Escape($lastUsing), "$lastUsing`nusing NSubstitute;"
        }
    }
}

# Phase 2: Remove MockRepository initialization  
# Pattern: var mocks = new MockRepository(); with surrounding empty lines
$content = $content -replace "(\s+)var mocks = new MockRepository\(\);\r?\n", "`$1"

# Phase 3: Replace mocks.Stub<T>() -> Substitute.For<T>()
$content = $content -replace "mocks\.Stub<([^>]+)>\(\)", "Substitute.For<`$1>()"

# Phase 4: Replace object.Stub(lambda).Return -> object.Return pattern
# This is trickier - need to handle chained calls
# Pattern: variable.Stub(x => x.Property).Return(value)
$content = $content -replace "(\w+)\.Stub\(([^)]*)\s*=>\s*(.+?)\)\.Return", "`$1.`$3.Returns"

# Phase 5: Remove ReplayAll() calls with surrounding whitespace
$content = $content -replace "\s*mocks\.ReplayAll\(\);\r?\n", "`n"

# Phase 6: Remove VerifyAll() calls with surrounding whitespace
$content = $content -replace "\s*mocks\.VerifyAll\(\);\r?\n", "`n"

# Phase 7: Clean up extra blank lines created by removals
$content = $content -replace "\r?\n\s*\r?\n\s*\r?\n", "`n`n"

# Write back with UTF8 encoding (no BOM)
$utf8NoBOM = New-Object System.Text.UTF8Encoding $false
[System.IO.File]::WriteAllText($FilePath, $content, $utf8NoBOM)

Write-Host "Migrated: $FilePath" -ForegroundColor Cyan

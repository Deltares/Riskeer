#!/usr/bin/env pwsh
<#
.SYNOPSIS
Scan all test files for Rhino.Mocks patterns and generate migration manifest
Single-pass scanner without nested script invocation
.PARAMETER OutputManifest
Path to save the migration manifest (JSON format)
#>
param(
    [Parameter(Mandatory=$false)]
    [string]$OutputManifest = "migration-manifest.json"
)

Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process -Force

# Pattern definitions matching DEPENDENCIES.md rulebook
$patterns = @(
    @{ name = "MockRepository"; regex = "new MockRepository\(\)"; rule = "Remove entirely"; type = "removal" },
    @{ name = "mocks.Stub<T>()"; regex = "mocks\.Stub<[^>]+>\(\)"; rule = "Replace with Substitute.For<T>()"; type = "transform" },
    @{ name = "mocks.StrictMock<T>()"; regex = "mocks\.StrictMock<[^>]+>\(\)"; rule = "Replace with Substitute.For<T>()"; type = "transform" },
    @{ name = "obj.Stub(x => x.Foo).Return(v)"; regex = "\w+\.Stub\([^)]*=>[^)]*\)\.Return\("; rule = "Transform to obj.Foo.Returns(v)"; type = "contextual" },
    @{ name = "obj.Expect(x => x.Foo).Return(v)"; regex = "\w+\.Expect\([^)]*=>[^)]*\)\.Return\("; rule = "Add Received() assertion"; type = "contextual" },
    @{ name = "IgnoreArguments()"; regex = "\.IgnoreArguments\(\)"; rule = "Use Arg.Any<T>()"; type = "contextual" },
    @{ name = "Arg<T>.Is.Anything"; regex = "Arg<[^>]+>\.Is\.Anything"; rule = "Replace with Arg.Any<T>()"; type = "transform" },
    @{ name = "Repeat.Never()"; regex = "Repeat\.Never\(\)"; rule = "Use DidNotReceive()"; type = "contextual" },
    @{ name = "Repeat.Any()"; regex = "Repeat\.Any\(\)"; rule = "Remove verification"; type = "contextual" },
    @{ name = "Repeat.AtLeastOnce()"; regex = "Repeat\.AtLeastOnce\(\)"; rule = "Use Received()"; type = "contextual" },
    @{ name = "Repeat.Once()"; regex = "Repeat\.Once\(\)"; rule = "Use Received(1)"; type = "contextual" },
    @{ name = "Repeat.Twice()"; regex = "Repeat\.Twice\(\)"; rule = "Use Received(2)"; type = "contextual" },
    @{ name = "Repeat.Times(n)"; regex = "Repeat\.Times\(\d+\)"; rule = "Use Received(n)"; type = "contextual" },
    @{ name = ".Throw()"; regex = "\.Throw\([^)]+\)"; rule = "Use Returns or When/Do"; type = "contextual" },
    @{ name = "WhenCalled(inv => ...)"; regex = "\.WhenCalled\([^)]*=>\s*[^)]*\)"; rule = "Use When/Do"; type = "contextual" },
    @{ name = ".Do(Action)"; regex = "\.Do\([^)]+\)"; rule = "Use When/Do or Returns"; type = "contextual" },
    @{ name = "ReplayAll()"; regex = "\.ReplayAll\(\)"; rule = "Remove"; type = "removal" },
    @{ name = "VerifyAll()"; regex = "\.VerifyAll\(\)"; rule = "Use Received()"; type = "contextual" },
    @{ name = "using Rhino.Mocks"; regex = "using Rhino\.Mocks"; rule = "Remove"; type = "removal" }
)

function Find-Patterns {
    param([string]$content)
    $findings = @()
    $lines = $content -split "`n"
    
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $lineNum = $i + 1
        $line = $lines[$i]
        
        foreach ($pattern in $patterns) {
            if ($line -match $pattern.regex) {
                $findings += @{
                    line = $lineNum
                    content = $line.Trim()
                    pattern = $pattern.name
                    type = $pattern.type
                }
            }
        }
    }
    return $findings
}

# Find all test files
$testFiles = @(Get-ChildItem -Path "Riskeer" -Recurse -Filter "*Test.cs" -ErrorAction SilentlyContinue | 
    Where-Object { $_.FullName -like "*\test\*" } |
    Select-Object -ExpandProperty FullName)

Write-Host "Found $($testFiles.Count) test files to scan" -ForegroundColor Cyan

$manifest = @{
    generatedAt = [datetime]::UtcNow.ToString("O")
    totalFiles = $testFiles.Count
    migratedFiles = 0
    pendingFiles = 0
    fileSummary = @()
    patternStats = @{ removals = 0; transforms = 0; contextual = 0 }
}

$fileIndex = 0

foreach ($file in $testFiles) {
    $fileIndex++
    if ($fileIndex % 100 -eq 0) {
        Write-Host "Scanned $fileIndex / $($testFiles.Count) files..." -ForegroundColor Gray
    }
    
    $relPath = $file -replace [regex]::Escape((Get-Location).Path + "\"), ""
    
    try {
        $content = Get-Content -Path $file -Encoding UTF8 -Raw -ErrorAction SilentlyContinue
        if (-not $content) { continue }
        
        # Check if already fully migrated
        if ($content -notmatch "Rhino\.Mocks|\.Stub\(|\.Expect\(|\.IgnoreArguments\(\)|Repeat\.|WhenCalled|ReplayAll|VerifyAll" -and $content -like "*using NSubstitute*") {
            $manifest.migratedFiles++
            continue
        }
        
        # Scan for patterns
        $findings = Find-Patterns -content $content
        
        if ($findings.Count -gt 0) {
            $summary = @{
                removals = @($findings | Where-Object { $_.type -eq "removal" }).Count
                transforms = @($findings | Where-Object { $_.type -eq "transform" }).Count
                contextual = @($findings | Where-Object { $_.type -eq "contextual" }).Count
            }
            
            $manifest.fileSummary += @{
                file = $relPath
                status = "pending"
                patternCount = $findings.Count
                patterns = $findings
                summary = $summary
            }
            
            $manifest.pendingFiles++
            $manifest.patternStats.removals += $summary.removals
            $manifest.patternStats.transforms += $summary.transforms
            $manifest.patternStats.contextual += $summary.contextual
        }
    } catch {
        # Silently skip on error
    }
}

# Save manifest
$utf8NoBOM = New-Object System.Text.UTF8Encoding $false
$json = $manifest | ConvertTo-Json -Depth 5
[System.IO.File]::WriteAllText($OutputManifest, $json, $utf8NoBOM)

Write-Host ""
Write-Host "Migration Manifest Summary:" -ForegroundColor Green
Write-Host "  Total test files: $($manifest.totalFiles)"
Write-Host "  Already migrated: $($manifest.migratedFiles)"
Write-Host "  Pending migration: $($manifest.pendingFiles)"
Write-Host ""
Write-Host "Pattern Frequency:" -ForegroundColor Green
Write-Host "  Removals (easy): $($manifest.patternStats.removals)"
Write-Host "  Transforms (medium): $($manifest.patternStats.transforms)"
Write-Host "  Contextual (requires review): $($manifest.patternStats.contextual)"
Write-Host ""
Write-Host "Manifest saved to: $OutputManifest" -ForegroundColor Cyan

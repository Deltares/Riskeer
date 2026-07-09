#!/usr/bin/env pwsh
<#
.SYNOPSIS
Scan test files for Rhino.Mocks patterns and flag them for migration
Outputs a structured report of patterns found so agents can process them intelligently
.PARAMETER FilePath
Path to the C# test file to scan
.PARAMETER OutputFile
Optional: Path to save JSON report of findings (default: stdout)
#>
param(
    [Parameter(Mandatory=$true)]
    [string]$FilePath,
    [Parameter(Mandatory=$false)]
    [string]$OutputFile
)

function Find-Patterns {
    param([string]$content, [string]$filePath)
    
    $findings = @()
    $lines = $content -split "`n"
    
    # Pattern definitions matching DEPENDENCIES.md rulebook
    $patterns = @(
        @{
            name = "MockRepository"
            regex = "new MockRepository\(\)"
            rule = "Remove entirely; use Substitute.For<T>() inline"
            type = "removal"
        },
        @{
            name = "mocks.Stub<T>()"
            regex = "mocks\.Stub<[^>]+>\(\)"
            rule = "Replace with Substitute.For<T>()"
            type = "transform"
        },
        @{
            name = "mocks.StrictMock<T>()"
            regex = "mocks\.StrictMock<[^>]+>\(\)"
            rule = "Replace with Substitute.For<T>()"
            type = "transform"
        },
        @{
            name = "obj.Stub(x => x.Foo).Return(v)"
            regex = "\w+\.Stub\([^)]*=>[^)]*\)\.Return\("
            rule = "Transform to obj.Foo.Returns(v); requires context review"
            type = "contextual"
        },
        @{
            name = "obj.Expect(x => x.Foo).Return(v)"
            regex = "\w+\.Expect\([^)]*=>[^)]*\)\.Return\("
            rule = "Replace with obj.Foo.Returns(v); add obj.Received().Foo() assertion"
            type = "contextual"
        },
        @{
            name = "IgnoreArguments()"
            regex = "\.IgnoreArguments\(\)"
            rule = "Use Arg.Any<T>() in the lambda instead"
            type = "contextual"
        },
        @{
            name = "Arg<T>.Is.Anything"
            regex = "Arg<[^>]+>\.Is\.Anything"
            rule = "Replace with Arg.Any<T>()"
            type = "transform"
        },
        @{
            name = "Repeat.Never()"
            regex = "Repeat\.Never\(\)"
            rule = "Use obj.DidNotReceive().Foo() assertion"
            type = "contextual"
        },
        @{
            name = "Repeat.Any()"
            regex = "Repeat\.Any\(\)"
            rule = "Remove verification (no equivalent in NSubstitute)"
            type = "contextual"
        },
        @{
            name = "Repeat.AtLeastOnce()"
            regex = "Repeat\.AtLeastOnce\(\)"
            rule = "Replace with obj.Received().Foo() assertion"
            type = "contextual"
        },
        @{
            name = "Repeat.Once()"
            regex = "Repeat\.Once\(\)"
            rule = "Replace with obj.Received().Foo() or obj.Received(1).Foo()"
            type = "contextual"
        },
        @{
            name = "Repeat.Twice()"
            regex = "Repeat\.Twice\(\)"
            rule = "Replace with obj.Received(2).Foo() assertion"
            type = "contextual"
        },
        @{
            name = "Repeat.Times(n)"
            regex = "Repeat\.Times\(\d+\)"
            rule = "Replace with obj.Received(n).Foo() assertion"
            type = "contextual"
        },
        @{
            name = ".Throw() on non-void"
            regex = "\.Throw\([^)]+\)(?!.*void)"
            rule = "Replace with .Returns(_ => throw new Ex())"
            type = "contextual"
        },
        @{
            name = ".Throw() on void"
            regex = "void.*\.Throw\([^)]+\)"
            rule = "Use .When(x => x.VoidFoo()).Do(_ => throw new Ex())"
            type = "contextual"
        },
        @{
            name = "WhenCalled(inv => ...)"
            regex = "\.WhenCalled\([^)]*=>\s*[^)]*\)"
            rule = "Replace with .When(x => x.Foo()).Do(...)"
            type = "contextual"
        },
        @{
            name = ".Do(Action)"
            regex = "\.Do\([^)]+\)"
            rule = "Use .When(x => x.Foo()).Do(...) or .Returns(...)"
            type = "contextual"
        },
        @{
            name = "ReplayAll()"
            regex = "\.ReplayAll\(\)"
            rule = "Remove (not needed in NSubstitute)"
            type = "removal"
        },
        @{
            name = "VerifyAll()"
            regex = "\.VerifyAll\(\)"
            rule = "Remove; replace with explicit Received() assertions"
            type = "contextual"
        },
        @{
            name = "using Rhino.Mocks"
            regex = "using Rhino\.Mocks"
            rule = "Remove; add 'using NSubstitute;' if not present"
            type = "removal"
        }
    )
    
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $lineNum = $i + 1
        $line = $lines[$i]
        
        # Skip if line doesn't contain Rhino references
        if ($line -notmatch "Rhino|mocks|Substitute|Repeat|Arg|IgnoreArguments|WhenCalled|\.Do\(|\.Throw\(|Expect|ReplayAll|VerifyAll") {
            continue
        }
        
        foreach ($pattern in $patterns) {
            if ($line -match $pattern.regex) {
                $findings += @{
                    line = $lineNum
                    content = $line.Trim()
                    pattern = $pattern.name
                    rule = $pattern.rule
                    type = $pattern.type
                }
            }
        }
    }
    
    return $findings
}

# Main logic
$content = Get-Content -Path $FilePath -Encoding UTF8 -Raw

# Skip if already fully migrated
if ($content -notmatch "Rhino\.Mocks|\.Stub\(|\.Expect\(|\.IgnoreArguments\(\)|Repeat\.|WhenCalled|ReplayAll|VerifyAll" -and $content -like "*using NSubstitute*") {
    Write-Host "Already migrated: $FilePath" -ForegroundColor Green
    return
}

$findings = Find-Patterns -content $content -filePath $FilePath

if ($findings.Count -eq 0) {
    Write-Host "No Rhino.Mocks patterns found: $FilePath" -ForegroundColor Green
    return
}

# Build report
$report = @{
    file = $FilePath
    needsMigration = $true
    patternCount = $findings.Count
    patterns = $findings
    summary = @{
        removals = @($findings | Where-Object { $_.type -eq "removal" }).Count
        transforms = @($findings | Where-Object { $_.type -eq "transform" }).Count
        contextual = @($findings | Where-Object { $_.type -eq "contextual" }).Count
    }
}

# Output
if ($OutputFile) {
    $report | ConvertTo-Json -Depth 10 | Out-File -FilePath $OutputFile -Encoding UTF8
    Write-Host "Flagged $($findings.Count) patterns in: $FilePath -> $OutputFile" -ForegroundColor Cyan
} else {
    $report | ConvertTo-Json -Depth 10
}

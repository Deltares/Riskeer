# Fix-SubstituteReturns.ps1
# GOAL
# ----
# NSubstitute allows mocking interfaces with Substitute.For<T>(). A common test
# anti-pattern is returning one substitute from another substitute's property/method:
#
#   var window = Substitute.For<IMainWindow>();
#   var gui = Substitute.For<IGui>();
#   gui.MainWindow.Returns(window);          <-- this is what we want to remove
#
# NSubstitute auto-mocks reference-type properties/methods, so the .Returns() call
# is redundant. This script removes:
#   - The .Returns(subVar) call (single-line or multi-line chained form)
#   - The preceding method-call chain that .Returns() was chained onto, e.g.:
#       provider.GetService(Arg.Any<Type>()).Returns(service);  <-- full line deleted
#       someObj.SomeMethod(arg)                                 \
#                         .Returns(service);                    /  both lines deleted
#   - The substitute variable declaration (var window = Substitute.For<...>())
#     IF the variable is not referenced anywhere else in the same test method.
#
# EXCLUSIONS
# ----------
# Variables are left untouched if they are used with .Received() or .DidNotReceive()
# anywhere within the same test method, because those are meaningful assertions:
#
#   var project = Substitute.For<IProject>();
#   project.DidNotReceive().NotifyObservers();   <-- keeps var + .Returns()
#
# HOW IT WORKS
# ------------
# For each .cs file:
#   1. Scan for all  var x = Substitute.For<...>()  declarations.
#   2. Split the file into per-test-method ranges using [Test*] attribute lines as
#      boundaries, to avoid false positives from Dutch log-message strings, etc.
#   3. For each .Returns(x) call, look up whether x is a substitute declared above
#      in the same method.
#   4. If so, skip when x is used with Received/DidNotReceive in the same method.
#   5. Only continue when x is a bare substitute (only declared and returned,
#      with no additional configuration/usages in the same method).
#   6. Collect lines to delete.
#   7. Write the file back using the original encoding (preserving BOM) and line endings.
#
# STATUS
# ------
# Script is complete and tested on individual files. Works correctly for:
#   - Single-line:  provider.GetService(Arg.Any<Type>()).Returns(service);
#   - Multi-line:   someObj.SomeMethod(arg)
#                           .Returns(service);
#   - Received() exclusion verified.
# Full run was interrupted before completing. Re-run to apply to all files.
#
# USAGE
# -----
#   cd C:\Repos\Riskeer
#   .\Fix-SubstituteReturns.ps1
#
# Or test on a single file:
#   Fix-SubstituteReturns -FilePath "path\to\File.cs" -DryRun $true

param(
    [string]$RootPath = (Get-Location),
    [switch]$DryRun
)

function Fix-SubstituteReturns {
    param([string]$FilePath, [bool]$DryRun = $false)

    $rawBytes = [System.IO.File]::ReadAllBytes($FilePath)
    $hasBom = ($rawBytes.Length -ge 3) -and ($rawBytes[0] -eq 0xEF) -and ($rawBytes[1] -eq 0xBB) -and ($rawBytes[2] -eq 0xBF)
    $enc = if ($hasBom) { [System.Text.UTF8Encoding]::new($true) } else { [System.Text.UTF8Encoding]::new($false) }
    $content = [System.IO.File]::ReadAllText($FilePath, $enc)
    $useCrlf = $content.Contains("`r`n")
    $sep = if ($useCrlf) { "`r`n" } else { "`n" }
    $lines = $content -split '\r?\n'

    # Build list of line indices where test methods start ([Test*] attribute),
    # used to scope variable lookups to a single method to avoid cross-method
    # false positives (e.g. variable name appearing in a Dutch string in another method).
    $bounds = [System.Collections.Generic.List[int]]::new()
    $bounds.Add(0)
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match '^\s*\[Test') { $bounds.Add($i) }
    }
    $bounds.Add($lines.Count)

    $linesToDelete = [System.Collections.Generic.HashSet[int]]::new()
    $count = 0

    for ($i = 0; $i -lt $lines.Count; $i++) {
        # Find any .Returns(someIdentifier) call
        if (-not ($lines[$i] -match "\.Returns\(\s*(\w+)\s*[\),]")) { continue }
        $varName = $matches[1]
        $esc = [regex]::Escape($varName)

        # Search upward for a Substitute.For<> declaration of this variable
        $declIdx = -1
        for ($j = $i - 1; $j -ge 0; $j--) {
            if ($lines[$j] -match "(?:var\s+|[\w<>\[\],\s]+\s+)$esc\s*=\s*Substitute\.For") {
                $declIdx = $j; break
            }
        }
        if ($declIdx -lt 0) { continue }  # Not a substitute variable, skip

        # Find the test method range that contains this declaration
        $mStart = 0; $mEnd = $lines.Count - 1
        for ($m = 0; $m -lt $bounds.Count - 1; $m++) {
            if ($bounds[$m] -le $declIdx -and $declIdx -lt $bounds[$m+1]) {
                $mStart = $bounds[$m]; $mEnd = $bounds[$m+1] - 1; break
            }
        }

        # EXCLUSION: skip if this substitute is used in a Received/DidNotReceive
        # assertion within the same test method (those are meaningful assertions
        # that need the variable to exist and be tracked by NSubstitute).
        $skip = $false
        for ($j = $mStart; $j -le $mEnd; $j++) {
            # Same line: varName.Received( or varName.DidNotReceive(
            if ($lines[$j] -match "\b$esc\b[^\r\n]*\.(Received|DidNotReceive)\s*\(") {
                $skip = $true; break
            }
            # Multi-line: varName at end of line, .Received/.DidNotReceive on next line
            if ($j -lt $lines.Count - 1 -and
                $lines[$j] -match "\b$esc\b\s*$" -and
                $lines[$j+1] -match "^\s*\.(Received|DidNotReceive)\s*\(") {
                $skip = $true; break
            }
        }
        if ($skip) { continue }

        # Only handle bare substitutes: if varName is used/configured anywhere else
        # in this method (other than declaration or .Returns(varName) lines), skip.
        $isBareSubstitute = $true
        for ($j = $mStart; $j -le $mEnd; $j++) {
            if ($j -eq $declIdx) { continue }
            $stripped = $lines[$j] -replace '"[^"]*"', '""'
            if (-not ($stripped -match "\b$esc\b")) { continue }

            if ($stripped -match "\.Returns\(\s*$esc\s*[\),]") { continue }
            if ($stripped -match "\b$esc\b[^\r\n]*\.(Received|DidNotReceive)\s*\(") { continue }
            if ($j -lt $lines.Count - 1 -and
                $stripped -match "\b$esc\b\s*$" -and
                $lines[$j+1] -match "^\s*\.(Received|DidNotReceive)\s*\(") {
                continue
            }

            $isBareSubstitute = $false
            break
        }
        if (-not $isBareSubstitute) { continue }

        $count++

        # Delete the var declaration if the variable is not used elsewhere in this method.
        # Strip string literals first to avoid matching variable names inside quoted strings.
        $usedElsewhere = $false
        for ($j = $mStart; $j -le $mEnd; $j++) {
            if ($j -eq $declIdx -or $j -eq $i) { continue }
            $stripped = $lines[$j] -replace '"[^"]*"', '""'
            if ($stripped -match "\b$esc\b") { $usedElsewhere = $true; break }
        }
        if (-not $usedElsewhere) { $linesToDelete.Add($declIdx) | Out-Null }

        if ($lines[$i] -match '^\s*\.') {
            # Multi-line form: .Returns(x) is on its own line(s), e.g.:
            #   someObj.SomeMethod(arg)
            #           .OtherChain()
            #           .Returns(service);
            # Delete the .Returns line, any preceding chained lines (starting with .),
            # and the statement start line.
            $linesToDelete.Add($i) | Out-Null
            $prev = $i - 1
            while ($prev -ge 0 -and $lines[$prev] -match '^\s*\.') {
                $linesToDelete.Add($prev) | Out-Null; $prev--
            }
            if ($prev -ge 0) { $linesToDelete.Add($prev) | Out-Null }
        } else {
            # Single-line form: provider.GetService(Arg.Any<Type>()).Returns(service);
            # Delete the entire line.
            $linesToDelete.Add($i) | Out-Null
        }
    }

    if ($count -eq 0) { return 0 }

    $newLines = [System.Collections.Generic.List[string]]::new()
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if (-not $linesToDelete.Contains($i)) { $newLines.Add($lines[$i]) }
    }

    if ($DryRun) {
        Write-Host "[DRY RUN] $([System.IO.Path]::GetFileName($FilePath)): would remove $count occurrence(s), $($linesToDelete.Count) line(s)"
    } else {
        [System.IO.File]::WriteAllText($FilePath, [string]::Join($sep, $newLines), $enc)
    }
    return $count
}

# --- Main ---

$totalChanges = 0
$filesModified = 0

Get-ChildItem -Path $RootPath -Recurse -Filter "*.cs" | ForEach-Object {
    $n = Fix-SubstituteReturns -FilePath $_.FullName -DryRun $DryRun.IsPresent
    if ($n -gt 0) { $script:totalChanges += $n; $script:filesModified++ }
}

Write-Host "Done. Removed $totalChanges occurrence(s) across $filesModified file(s)."

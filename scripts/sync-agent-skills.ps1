# Mirrors the canonical skills in .github/skills to .claude/skills so GitHub Copilot
# and Claude Code load exactly the same skills. Edit skills in .github/skills only.
$root = Split-Path -Parent $PSScriptRoot
$source = Join-Path $root '.github/skills'
$target = Join-Path $root '.claude/skills'

New-Item -ItemType Directory -Force $target | Out-Null
Get-ChildItem $target -Directory | Where-Object { -not (Test-Path (Join-Path $source $_.Name)) } |
    Remove-Item -Recurse -Force
Copy-Item -Path (Join-Path $source '*') -Destination $target -Recurse -Force
Write-Host "Synced skills from $source to $target"

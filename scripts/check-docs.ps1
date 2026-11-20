param()

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot

$required = @(
    'README.md', 'AGENTS.md', 'docs/README.md',
    'docs/product/OVERVIEW.md', 'docs/product/REQUIREMENTS.md',
    'docs/technology/STACK.md', 'docs/architecture/ARCHITECTURE.md',
    'docs/architecture/PROTOCOL.md', 'docs/architecture/DECISIONS.md',
    'docs/frontend/UX.md', 'docs/frontend/DESIGN_SYSTEM.md',
    'docs/frontend/FILE_GUIDE.md', 'docs/frontend/theme.json',
    'docs/frontend/assets/client-wireframe.svg', 'docs/frontend/assets/server-wireframe.svg',
    'docs/planning/PLAN.md', 'docs/planning/TASKS.md', 'docs/planning/WORKFLOW.md',
    'docs/quality/TESTING.md', 'docs/quality/SECURITY.md', 'docs/setup/ENVIRONMENT.md',
    'docs/submission/CHECKLIST.md', 'docs/history/DEVELOPMENT.md', 'docs/references/SOURCES.md'
)
foreach ($relative in $required) {
    if (-not (Test-Path -LiteralPath (Join-Path $root $relative) -PathType Leaf)) {
        throw "Required artifact missing: $relative"
    }
}

$markdown = @(Get-Item -LiteralPath (Join-Path $root 'README.md'), (Join-Path $root 'AGENTS.md'))
$markdown += @(Get-ChildItem -LiteralPath (Join-Path $root 'docs') -Filter '*.md' -Recurse -File)
$markdown += @(Get-Item -LiteralPath (Join-Path $root '.github/pull_request_template.md'))
$links = 0
foreach ($file in $markdown) {
    $content = [IO.File]::ReadAllText($file.FullName)
    foreach ($match in [regex]::Matches($content, '\[[^\]]+\]\(([^)#]+)(?:#[^)]*)?\)')) {
        $target = $match.Groups[1].Value.Trim('<', '>')
        if ($target -match '^(https?://|mailto:|#)') { continue }
        $target = [Uri]::UnescapeDataString($target)
        $resolved = [IO.Path]::GetFullPath((Join-Path $file.DirectoryName $target))
        if (-not $resolved.StartsWith($root + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
            throw "Link leaves repository: $($file.Name) -> $target"
        }
        if (-not (Test-Path -LiteralPath $resolved -PathType Leaf)) {
            throw "Broken local file link: $($file.Name) -> $target"
        }
        $links++
    }
}

$tasks = [IO.File]::ReadAllText((Join-Path $root 'docs/planning/TASKS.md'))
$definitions = @([regex]::Matches($tasks, '(?m)^\| ((?:DOC|P[0-9]{2})-[0-9]{3}) \|') |
    ForEach-Object { $_.Groups[1].Value })
if ($definitions.Count -eq 0 -or @($definitions | Select-Object -Unique).Count -ne $definitions.Count) {
    throw 'Task definitions are empty or duplicated.'
}
$taskReferences = 0
foreach ($file in $markdown) {
    foreach ($match in [regex]::Matches([IO.File]::ReadAllText($file.FullName), '\b(?:DOC|P[0-9]{2})-[0-9]{3}\b')) {
        if ($match.Value -notin $definitions) { throw "Unknown task ID: $($match.Value) in $($file.Name)" }
        $taskReferences++
    }
}
foreach ($line in ($tasks -split '\r?\n')) {
    if ($line -match '^\| (?:DOC|P[0-9]{2})-[0-9]{3} \|') {
        $status = ($line.Trim('|') -split '\|')[-1].Trim()
        if ($status -notin @('Done', 'In progress', 'Pending', 'Deferred')) { throw "Invalid task status: $status" }
    }
}

$tokens = [IO.File]::ReadAllText((Join-Path $root 'docs/frontend/theme.json')) | ConvertFrom-Json
if ($tokens.status -ne 'design-proposal') { throw 'Tokens must identify their proposed status.' }
$design = [IO.File]::ReadAllText((Join-Path $root 'docs/frontend/DESIGN_SYSTEM.md'))
foreach ($color in $tokens.colors.PSObject.Properties) {
    if ($color.Value -notmatch '^#[0-9A-Fa-f]{6}$' -or -not $design.Contains($color.Value)) {
        throw "Invalid or undocumented color: $($color.Name)"
    }
}

function Get-Luminance {
    param([string]$Hex)
    $values = @(1, 3, 5 | ForEach-Object {
        $channel = [Convert]::ToInt32($Hex.Substring($_, 2), 16) / 255.0
        if ($channel -le 0.04045) { $channel / 12.92 }
        else { [Math]::Pow(($channel + 0.055) / 1.055, 2.4) }
    })
    return 0.2126 * $values[0] + 0.7152 * $values[1] + 0.0722 * $values[2]
}
$contrastCount = 0
foreach ($pair in $tokens.contrastChecks) {
    $foreground = $tokens.colors.PSObject.Properties[$pair.foreground]
    $background = $tokens.colors.PSObject.Properties[$pair.background]
    if ($null -eq $foreground -or $null -eq $background -or $pair.minimum -lt 4.5) {
        throw 'Invalid text contrast definition.'
    }
    $a = Get-Luminance $foreground.Value
    $b = Get-Luminance $background.Value
    $ratio = ([Math]::Max($a, $b) + 0.05) / ([Math]::Min($a, $b) + 0.05)
    if ($ratio -lt $pair.minimum) { throw "Insufficient contrast: $($pair.foreground)/$($pair.background)" }
    Write-Output ('Contrast {0}/{1}: {2:N2}:1' -f $pair.foreground, $pair.background, $ratio)
    $contrastCount++
}
if ($contrastCount -eq 0) { throw 'No contrast definitions.' }

$svgCount = 0
foreach ($name in @('client-wireframe.svg', 'server-wireframe.svg')) {
    $text = [IO.File]::ReadAllText((Join-Path $root ('docs/frontend/assets/' + $name)))
    $settings = New-Object Xml.XmlReaderSettings
    $settings.DtdProcessing = [Xml.DtdProcessing]::Prohibit
    $settings.XmlResolver = $null
    $inputReader = New-Object IO.StringReader($text)
    $reader = [Xml.XmlReader]::Create($inputReader, $settings)
    try {
        $document = New-Object Xml.XmlDocument
        $document.XmlResolver = $null
        $document.Load($reader)
    }
    finally {
        $reader.Dispose()
        $inputReader.Dispose()
    }
    if ($document.DocumentElement.LocalName -ne 'svg' -or
        $document.DocumentElement.NamespaceURI -ne 'http://www.w3.org/2000/svg' -or
        $null -eq $document.SelectSingleNode("//*[local-name()='text' and contains(., 'DESIGN PROPOSAL')]")) {
        throw "SVG is not a labeled design proposal: $name"
    }
    if ($document.SelectNodes("//*[local-name()='script' or local-name()='foreignObject']").Count -gt 0 -or
        $document.SelectNodes("//@*[starts-with(local-name(), 'on')]").Count -gt 0) {
        throw "Executable SVG content is not allowed: $name"
    }
    foreach ($href in $document.SelectNodes("//@*[local-name()='href']")) {
        if (-not $href.Value.StartsWith('#')) { throw "External SVG reference: $name" }
    }
    $svgCount++
}

Write-Output "PASS: $($required.Count) required artifacts; $($markdown.Count) Markdown files; $links local file links; $($definitions.Count) tasks / $taskReferences references; $contrastCount contrast pairs; $svgCount labeled SVGs."
Write-Output 'Documentation validation only. No application behavioral tests or manual UI acceptance claimed.'

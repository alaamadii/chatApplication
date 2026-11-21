[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet('doctor', 'restore', 'build', 'test', 'check', 'run-server', 'run-client')]
    [string]$Command = 'check'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path $PSScriptRoot -Parent
$previousDotnetRoot = [Environment]::GetEnvironmentVariable('DOTNET_ROOT', 'Process')
$previousCliHome = [Environment]::GetEnvironmentVariable('DOTNET_CLI_HOME', 'Process')
$previousGenerateCertificate = [Environment]::GetEnvironmentVariable('DOTNET_GENERATE_ASPNET_CERTIFICATE', 'Process')
$dotnetPath = (Get-Command dotnet -CommandType Application -ErrorAction Stop | Select-Object -First 1).Source

function Invoke-Dotnet {
    param([string[]]$Arguments)
    & $dotnetPath @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw ("dotnet {0} failed with exit code {1}." -f ($Arguments -join ' '), $LASTEXITCODE)
    }
}

function Restore-Solution {
    Invoke-Dotnet @('restore', 'ChatApplication.slnx')
}

function Build-Solution {
    Invoke-Dotnet @('build', 'ChatApplication.slnx', '--configuration', 'Release', '--no-restore')
}

function Test-Solution {
    Write-Host 'Core has no behavior and discovers zero cases. App.Tests exercises native UI contracts; no chat/transport coverage is claimed.'
    Invoke-Dotnet @('test', 'ChatApplication.slnx', '--configuration', 'Release', '--no-build',
        '--no-restore', '--logger', 'trx', '--results-directory', 'TestResults')
}

Push-Location $repositoryRoot
try {
    $env:DOTNET_ROOT = Split-Path $dotnetPath -Parent
    $env:DOTNET_CLI_HOME = Join-Path $repositoryRoot '.local/dotnet'
    $env:DOTNET_GENERATE_ASPNET_CERTIFICATE = 'false'
    [System.IO.Directory]::CreateDirectory($env:DOTNET_CLI_HOME) | Out-Null

    switch ($Command) {
        'doctor' {
            if ([Environment]::OSVersion.Platform -ne [PlatformID]::Win32NT) {
                throw 'The Server and Client require Windows with the .NET 10 WindowsDesktop runtime.'
            }
            Write-Host ("Repository: {0}" -f $repositoryRoot)
            Write-Host ("Windows: {0}" -f [Environment]::OSVersion.Version)
            Write-Host ("PowerShell: {0}" -f $PSVersionTable.PSVersion)
            Write-Host ("dotnet: {0}" -f $dotnetPath)
            Invoke-Dotnet @('--version')
            Invoke-Dotnet @('--list-sdks')
            Invoke-Dotnet @('--list-runtimes')
            & git --version
            if ($LASTEXITCODE -ne 0) { throw 'Git prerequisite check failed.' }
            $runtimes = & $dotnetPath --list-runtimes
            if ($LASTEXITCODE -ne 0) { throw 'Runtime prerequisite check failed.' }
            if (-not ($runtimes -match '^Microsoft.WindowsDesktop.App 10\.0\.')) {
                throw 'Install the .NET 10 WindowsDesktop runtime before launching either application.'
            }
            Write-Host 'VS Code/C# debugger installation and physical F5 remain owner checks.'
        }
        'restore' { Restore-Solution }
        'build' { Restore-Solution; Build-Solution }
        'test' { Restore-Solution; Build-Solution; Test-Solution }
        'check' {
            Restore-Solution
            Build-Solution
            Test-Solution
            Invoke-Dotnet @('format', 'ChatApplication.slnx', '--verify-no-changes', '--no-restore')
            & (Join-Path $PSScriptRoot 'check-docs.ps1')
        }
        'run-server' {
            Restore-Solution
            Build-Solution
            Invoke-Dotnet @('run', '--project', 'src/ChatApplication.Server',
                '--configuration', 'Release', '--no-build', '--no-restore')
        }
        'run-client' {
            Restore-Solution
            Build-Solution
            Invoke-Dotnet @('run', '--project', 'src/ChatApplication.Client',
                '--configuration', 'Release', '--no-build', '--no-restore')
        }
    }
}
finally {
    [Environment]::SetEnvironmentVariable('DOTNET_ROOT', $previousDotnetRoot, 'Process')
    [Environment]::SetEnvironmentVariable('DOTNET_CLI_HOME', $previousCliHome, 'Process')
    [Environment]::SetEnvironmentVariable('DOTNET_GENERATE_ASPNET_CERTIFICATE', $previousGenerateCertificate, 'Process')
    Pop-Location
}

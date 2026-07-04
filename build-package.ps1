<#
.SYNOPSIS
    Build and package eWatson NuGet packages.

.DESCRIPTION
    This script builds the eWatson solution and creates NuGet packages for all
    projects. Packages are output to the ./nupkgs directory.

.PARAMETER Version
    Override the package version. If not specified, uses version from
    src/eWatson/eWatson.csproj.

.PARAMETER Configuration
    Build configuration (Debug or Release). Default is Release.

.PARAMETER IncludeSymbols
    Include symbol package (.snupkg) for debugging. Default is true.

.PARAMETER Clean
    Clean build output before building. Default is true.

.PARAMETER SkipBuild
    Skip the explicit solution build step. The pack step still builds the
    packable project so project-reference outputs can be collected correctly.
    Default is false.

.EXAMPLE
    .\build-package.ps1
    Build and pack eWatson with Release configuration.

.EXAMPLE
    .\build-package.ps1 -Version 1.0.1-preview
    Build and pack with a specific version.

.EXAMPLE
    .\build-package.ps1 -Configuration Debug
    Build and pack with Debug configuration.

.EXAMPLE
    .\build-package.ps1 -SkipBuild
    Skip the explicit solution build and run pack directly.
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$Version,

    [Parameter(Mandatory = $false)]
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [Parameter(Mandatory = $false)]
    [bool]$Clean = $true,

    [Parameter(Mandatory = $false)]
    [bool]$SkipBuild = $false,

    [Parameter(Mandatory = $false)]
    [bool]$IncludeSymbols = $true
)

# Script configuration
$ErrorActionPreference = 'Stop'
$OutputDir = Join-Path $PSScriptRoot 'nupkgs'
$SolutionFile = Join-Path $PSScriptRoot 'eWatson.slnx'
$ProjectPath = Join-Path $PSScriptRoot 'src\eWatson\eWatson.csproj'

# Helper function to write colored output
function Write-Step {
    param([string]$Message)
    Write-Host "`n==> $Message" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "[+] $Message" -ForegroundColor Green
}

function Write-Error-Message {
    param([string]$Message)
    Write-Host "[x] $Message" -ForegroundColor Red
}

# Ensure output directory exists
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
    Write-Success "Created output directory: $OutputDir"
}

# Clean previous packages if requested
if ($Clean) {
    Write-Step "Cleaning previous packages"
    Get-ChildItem -Path $OutputDir -Filter *.nupkg | Remove-Item -Force
    Get-ChildItem -Path $OutputDir -Filter *.snupkg | Remove-Item -Force
    Write-Success "Cleaned output directory"
}

# Build arguments
$buildArgs = @(
    'build'
    $SolutionFile
    '--configuration', $Configuration
)

if ($Version) {
    $buildArgs += "-p:Version=$Version"
}

# Restore and build
if (-not $SkipBuild) {
    Write-Step "Restoring NuGet packages"
    & dotnet restore $SolutionFile
    if ($LASTEXITCODE -ne 0) {
        Write-Error-Message "Restore failed"
        exit $LASTEXITCODE
    }
    Write-Success "Restore completed"

    Write-Step "Building solution ($Configuration)"
    & dotnet @buildArgs
    if ($LASTEXITCODE -ne 0) {
        Write-Error-Message "Build failed"
        exit $LASTEXITCODE
    }
    Write-Success "Build completed"
}

# Pack arguments
$packArgs = @(
    '--configuration', $Configuration
    '--output', $OutputDir
)

if ($SkipBuild) {
    $packArgs += '--no-restore'
}

if ($IncludeSymbols) {
    $packArgs += '--include-symbols'
    $packArgs += '-p:SymbolPackageFormat=snupkg'
}

if ($Version) {
    $packArgs += "-p:Version=$Version"
}

# Pack eWatson project
Write-Step "Creating NuGet package"

Write-Host "Packing eWatson..." -ForegroundColor Yellow
& dotnet pack $ProjectPath @packArgs

if ($LASTEXITCODE -ne 0) {
    Write-Error-Message "Failed to pack eWatson"
    exit $LASTEXITCODE
}

Write-Success "Package created successfully"

# Display created packages
Write-Step "Created packages"
$packages = Get-ChildItem -Path $OutputDir -Filter *.nupkg |
    Where-Object { $_.Name -notlike '*.symbols.nupkg' }

foreach ($package in $packages) {
    $size = '{0:N2}' -f ($package.Length / 1KB)
    Write-Host "  * $($package.Name) ($size KB)" -ForegroundColor Gray
}

Write-Host ""
Write-Success "Packaging complete! Output: $OutputDir"
Write-Host ""

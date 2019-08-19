[CmdletBinding()]
Param(
    [string]$Script = "build.cake",
    [string]$CakeVersion = "0.33.0",
    [string]$Target,
    [Parameter(Position=0,Mandatory=$false,ValueFromRemainingArguments=$true)]
    [string[]]$ScriptArgs
)

# Define directories.
if(!$PSScriptRoot){
    $PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
}

$ToolsDir = Join-Path $PSScriptRoot "tools"
$CakePath = Join-Path $ToolsDir "dotnet-cake.exe"
$CakeInstalledVersion = Get-Command $CakePath -ErrorAction SilentlyContinue  | % {&$_.Source --version}

if ($CakeInstalledVersion -ne $CakeVersion) {
    if(Test-Path $CakePath -PathType leaf) {
        & dotnet tool uninstall Cake.Tool --tool-path "$ToolsDir"
    }
    Write-Output  "Installing Cake $CakeVersion..."
    & dotnet tool install Cake.Tool --tool-path "$ToolsDir" --version $CakeVersion
}

# Build Cake arguments
$cakeArguments = @("$Script");
if ($Target) { $cakeArguments += "--target=$Target" }
$cakeArguments += $ScriptArgs

& "$CakePath" $cakeArguments
exit $LASTEXITCODE
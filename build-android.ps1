$ErrorActionPreference = 'Stop'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$msbuild = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\Current\Bin\MSBuild.exe' | Select-Object -First 1
if (-not $msbuild) { throw 'Requiere Visual Studio 2022 actualizado y la carga de trabajo .NET MAUI/Android.' }
Push-Location $PSScriptRoot
try {
    & $msbuild 'MiAgendaUTN/MiAgendaUTN/MiAgendaUTN.csproj' /nologo /v:minimal /restore /t:Build /p:Configuration=Debug /p:TargetFramework=net9.0-android /p:TargetFrameworks=net9.0-android /p:DeployOnBuild=false
    if ($LASTEXITCODE -ne 0) { throw 'Falló la compilación Android.' }
} finally { Pop-Location }

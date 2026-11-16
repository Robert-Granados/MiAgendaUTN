param([Parameter(Mandatory=$true)][string]$Serial)
$ErrorActionPreference = 'Stop'
$adb = if ($env:ANDROID_HOME) { Join-Path $env:ANDROID_HOME 'platform-tools\adb.exe' } elseif (Test-Path -LiteralPath 'C:\Android\sdk\platform-tools\adb.exe') { 'C:\Android\sdk\platform-tools\adb.exe' } else { (Get-Command adb -ErrorAction Stop).Source }
$package = 'com.companyname.miagendautn'
$component = (& $adb -s $Serial shell cmd package resolve-activity --brief $package | Select-Object -Last 1).Trim()
if ($LASTEXITCODE -ne 0 -or $component -notmatch '^com\.companyname\.miagendautn/') { throw 'Instala primero el APK Debug en ese emulador.' }
& $adb -s $Serial shell am force-stop $package
& $adb -s $Serial shell run-as $package rm -f cache/compatibility-smoke/result.json
& $adb -s $Serial shell am start -W -n $component --ez compatibility-smoke true
if ($LASTEXITCODE -ne 0) { throw 'No se pudo iniciar la prueba.' }
for ($attempt = 0; $attempt -lt 60; $attempt++) {
    $raw = & $adb -s $Serial exec-out run-as $package cat cache/compatibility-smoke/result.json 2>$null
    if ($LASTEXITCODE -eq 0 -and $raw) {
        $result = ($raw -join "`n") | ConvertFrom-Json
        if ($result.status -ne 'passed') { throw $result.error }
        $result
        return
    }
    Start-Sleep -Seconds 1
}
throw 'La prueba no produjo resultado. Revisa adb logcat.'

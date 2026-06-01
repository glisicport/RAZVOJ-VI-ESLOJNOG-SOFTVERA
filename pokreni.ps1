$root = Split-Path -Parent $MyInvocation.MyCommand.Path

$tmp1 = "$env:TEMP\wt_run1.ps1"
$tmp2 = "$env:TEMP\wt_run2.ps1"

"Set-Location '$root\3_SLOJ_SERVISA\REST_SERVIS_CRUD_Operacija'; dotnet watch run" | Out-File $tmp1 -Encoding utf8
"Set-Location '$root\4_PREZENTACIONI_SLOJ\RVS_Aplikacija'; dotnet watch run" | Out-File $tmp2 -Encoding utf8

$ps = "C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe"

Start-Process wt -ArgumentList "new-tab", "$ps", "-NoExit", "-File", $tmp1, ";", "split-pane", "-V", "$ps", "-NoExit", "-File", $tmp2
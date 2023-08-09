param($TimerRun)
$asObj = ConvertFrom-Json $TimerRun -Depth 20
Write-Host $asObj.id
Write-Host $asObj.timestamp

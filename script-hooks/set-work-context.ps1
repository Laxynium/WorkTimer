param($TimerRun)

$WORK_CONTEXT_LOCATION = "~/.work-context/current-task"

$asObj = ConvertFrom-Json $TimerRun -Depth 20

$id = $asObj.labels.id

if($id -ne $null){
    echo $id > $WORK_CONTEXT_LOCATION
}
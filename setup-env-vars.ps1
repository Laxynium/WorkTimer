# NOTE that when you are running powershell script and pass the secret here 
# it will be save to history file
$env:NUGET_GITHUB_USERNAME = $args[0] 
$env:NUGET_GITHUB_TOKEN = $args[1] 
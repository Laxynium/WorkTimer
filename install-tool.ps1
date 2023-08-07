# Make sure that you also copied the NuGet.config, since it cannot install nuget from github with auth

$searchResult = dotnet tool list -g | Select-String worktimer.console

if($searchResult -eq $null){
    dotnet tool install --global WorkTimer.Console --configfile NuGet.config --no-cache
}else{
    dotnet tool update --global WorkTimer.Console --configfile NuGet.config --no-cache
}
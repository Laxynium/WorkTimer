dotnet pack -c Release
$catalogWithNuget = ".\WorkTimer.Console\bin\Release"
$latestNupkg = Get-ChildItem -Filter "*.nupkg" $catalogWithNuget | Sort-Object {$_.Name} -Descending | Select -First 1 | %{$_.Name}
dotnet nuget push "$($catalogWithNuget)\$($latestNupkg)" -s "https://nuget.pkg.github.com/$($env:NUGET_GITHUB_USERNAME)/index.json" -k "$($env:NUGET_GITHUB_TOKEN)"
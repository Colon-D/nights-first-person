# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/nights.test.firstperson/*" -Force -Recurse
dotnet publish "./nights.test.firstperson.csproj" -c Release -o "$env:RELOADEDIIMODS/nights.test.firstperson" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location

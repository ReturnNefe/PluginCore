# Pack with .NET Core 3.1/.NET 5/.NET6.0 SDK

try {
    Push-Location
    cd ../../Nefe.PluginCore/
    dotnet pack -c Release
}
finally {
    Pop-Location
}
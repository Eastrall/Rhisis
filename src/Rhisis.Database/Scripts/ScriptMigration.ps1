if ($args.Count -lt 1) {
    Write-Host 'Usage: ./ScriptMigration.ps1 [DatabaseConfigurationFile] [FromMigration] [ToMigration]'
    exit 1
}

$env:DB_CONFIG = $args[0]

if ($args.Count -eq 2) {
    dotnet ef migrations script $args[1] -p "Rhisis.Database.csproj" -s "../Rhisis.CLI/Rhisis.CLI.csproj"
}
elseif ($args.Count -eq 3) {
    dotnet ef migrations script $args[1] $args[2] -p "Rhisis.Database.csproj" -s "../Rhisis.CLI/Rhisis.CLI.csproj"
}
else {
    dotnet ef migrations script -p "Rhisis.Database.csproj" -s "../Rhisis.CLI/Rhisis.CLI.csproj"
}

$env:DB_CONFIG = "" 
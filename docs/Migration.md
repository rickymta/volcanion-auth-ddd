# Add migration
```sh
dotnet ef migrations add <migration_name> --project src\Volcanion.Auth.Infrastructure --startup-project src\Volcanion.Auth.Presentation
```

# Update database

```sh
dotnet ef database update --project src\Volcanion.Auth.Infrastructure --startup-project src\Volcanion.Auth.Presentation
```

```sh
dotnet sonarscanner begin /k:"volcanion-auth-ddd" /d:sonar.host.url="http://42.119.173.239:9000" /d:sonar.token="sqp_1b8ddb94be3ff808d69e3063b6ab88d1a1c3c771" /s:SonarQube.Analysis.xml

dotnet build

dotnet sonarscanner end /d:sonar.token="sqp_1b8ddb94be3ff808d69e3063b6ab88d1a1c3c771"
```

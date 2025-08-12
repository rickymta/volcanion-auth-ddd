# Add migration
```sh
dotnet ef migrations add <migration_name> --project src\Volcanion.Auth.Infrastructure --startup-project src\Volcanion.Auth.Presentation
```

# Update database

```sh
dotnet ef database update --project src\Volcanion.Auth.Infrastructure --startup-project src\Volcanion.Auth.Presentation
```

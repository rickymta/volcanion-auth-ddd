# Add migration
```sh
dotnet ef migrations add <migration_name> --project src\Volcanion.Auth.Infrastructure --startup-project src\Volcanion.Auth.Presentation
```

# Update database

```sh
dotnet ef database update --project src\Volcanion.Auth.Infrastructure --startup-project src\Volcanion.Auth.Presentation
```

`sqp_1b8ddb94be3ff808d69e3063b6ab88d1a1c3c771`

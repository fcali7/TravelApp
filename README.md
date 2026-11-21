# TravelApp – Projekt i kursen Dataåtkomster i .NET

ASP.NET Core WebApi med Entity Framework Core (code first) för en reseapplikation:
orter, sevärdheter, kommentarer och användare. Alla tjänster kan testas via Swagger.

## Kom igång

1. **Starta databasen (SQL Server i Docker)**
   ```powershell
   docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<lösenord>" -p 14333:1433 --name sql2022container --hostname sql2022 -d mcr.microsoft.com/mssql/server:2022-latest
   ```
   (se även `_scripts/docker/install_sql_docker.ps1`)

2. **User secrets** – lägg den medföljande `secrets.json` i
   `%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\` (id finns i `Configuration/Configuration.csproj`).

3. **Skapa databasen** – i en terminal i mappen `_scripts`:
   ```powershell
   .\database-rebuild-all.ps1 sql-travel sqlserver docker root ..\AppWebApi
   ```
   Kör därefter `DbContext/SqlScripts/sqlserver/initDatabase.sql` mot databasen.

4. **Starta WebApi** – i mappen `AppWebApi`:
   ```powershell
   dotnet run -lp https
   ```
   Öppna https://localhost:7086/swagger

## Attribution

This project was initiated using a template provided by Seido AB as part of their course curriculum. All original template code © Seido AB. Learn more about Seido AB and our educational programs at https://seido.se

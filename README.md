# TravelApp – Projekt i kursen Dataåtkomster i .NET

ASP.NET Core WebApi med Entity Framework Core (code first) för en reseapplikation:
orter, sevärdheter, kommentarer och användare. Alla tjänster kan användas via Swagger.

- Databas: **PostgreSQL** i Docker
- ER-modell och förklaring: [docs/ERD.md](docs/ERD.md)

## Kom igång

Kräver .NET 10 SDK, Docker och verktyget `dotnet-ef` (`dotnet tool install --global dotnet-ef`).

1. **Öppna projektet** – dubbelklicka på `TravelApp.code-workspace` för att öppna i Visual Studio Code.

2. **User secrets** – lägg den medföljande `secrets.json` i
   `%APPDATA%\Microsoft\UserSecrets\966df107-0255-4ded-8715-79c8828c7484\`
   (macOS/Linux: `~/.microsoft/usersecrets/966df107-0255-4ded-8715-79c8828c7484/`).

3. **Starta databasen i Docker** (lösenordet ska vara samma som i `secrets.json`, se även `_scripts/docker/install_sql_docker.ps1`):
   ```powershell
   docker run --name postgrescontainer -e POSTGRES_PASSWORD=<lösenord> -d -p 5432:5432 postgres
   ```

4. **Skapa databasen** – i en terminal i mappen `_scripts`:
   ```powershell
   .\database-rebuild-all.ps1 sql-travel postgresql docker root ..\AppWebApi
   ```
   (macOS/Linux: `./database-rebuild-all.sh sql-travel postgresql docker root ../AppWebApi`)

5. **Skapa view, stored procedure och databasanvändare** – i en terminal i projektets rotmapp:
   ```powershell
   Get-Content DbContext/SqlScripts/postgres/initDatabase.sql | docker exec -i postgrescontainer psql -U postgres -d sql-travel
   ```
   (macOS/Linux: `docker exec -i postgrescontainer psql -U postgres -d sql-travel < DbContext/SqlScripts/postgres/initDatabase.sql`)

6. **Starta WebApi** – tryck F5 i VS Code (konfigurationen *AppWebApi*), eller i mappen `AppWebApi`:
   ```powershell
   dotnet run -lp https
   ```
   och öppna https://localhost:7086/swagger

7. **Seeda testdata** – kör `GET /api/Admin/Seed` i Swagger.

## Tjänster (Swagger)

| Endpoint | Beskrivning |
|---|---|
| `GET /api/Admin/Seed` | Robust seeding: tar först bort tidigare testdata, skapar sedan 50 användare, 100 orter i 4 länder, 1000 sevärdheter och 0–20 kommentarer per sevärdhet |
| `GET /api/Admin/RemoveSeed` | Tar bort all testdata med stored procedure `supusr.spDeleteAll` |
| `GET /api/Guest/Info` | Översikt över databasen (antal användare, orter, sevärdheter, kommentarer) från view `gstusr.vwInfoDb` och `gstusr.vwInfoCountries` |
| `GET /api/Attractions/Read` | Alla sevärdheter. `filter` söker i kategori, rubrik, beskrivning, land och ort, t.ex. `oslo` eller `museum` |
| `GET /api/Attractions/ReadWithoutComments` | Alla sevärdheter som saknar kommentarer |
| `GET /api/Attractions/ReadItem?id=…` | En sevärdhets kategori, rubrik, beskrivning och alla kommentarer |
| `GET /api/Travelers/Read` | Alla användare och de kommentarer de har skrivit |

Alla listor använder pagination med `pageNr` (börjar på 0) och `pageSize`.

## Arkitektur

| Projekt | Lager |
|---|---|
| `AppWebApi` | Presentation – controllers och Swagger |
| `Services` | Applikationslager |
| `DbRepos` | Dataåtkomst – LINQ-frågor via EF Core |
| `DbContext` | EF Core DbContext, migrationer och SQL-skript |
| `DbModels` | Databasmodeller med EF Core annotations |
| `Models` | Domänmodeller och DTO:er |
| `Configuration` | Konfiguration, user secrets, loggning och dependency injection |

## Attribution

This project was initiated using a template provided by Seido AB as part of their course curriculum. All original template code © Seido AB. Learn more about Seido AB and our educational programs at https://seido.se

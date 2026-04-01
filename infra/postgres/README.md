# PostgreSQL Migrations

This folder contains SQL scripts used by the API startup migration runner.

## Migration Files

- `migrations/0001_init_schema.sql`
- `migrations/0002_seed_baseline.sql`

Scripts are applied in lexical order and tracked in the `schema_migrations` table.

1. Create database:

```bash
docker run --name taskmanager-postgres -e POSTGRES_PASSWORD=Pass@word -e POSTGRES_DB=taskmanager -p 5432:5432 -d postgres:16
```

2. Configure API to use Postgres:

Set in configuration:

```json
"Persistence": {
	"Provider": "Postgres",
	"ApplyMigrationsOnStartup": true,
	"MigrationsPath": "infra/postgres/migrations"
}
```

3. Run API (migrations auto-apply):

```bash
dotnet run --project src/Api/Api.csproj
```

4. Optional manual apply:

```bash
psql postgresql://postgres:Pass@word@localhost:5432/taskmanager -f migrations/0001_init_schema.sql
psql postgresql://postgres:Pass@word@localhost:5432/taskmanager -f migrations/0002_seed_baseline.sql
```

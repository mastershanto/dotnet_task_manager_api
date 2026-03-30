# PostgreSQL Migration Template

This folder contains scaffolded SQL for setting up Postgres persistence.

1. Create database:

```bash
docker run --name taskmanager-postgres -e POSTGRES_PASSWORD=Pass@word -e POSTGRES_DB=taskmanager -p 5432:5432 -d postgres:16
```

2. Apply schema:

```bash
psql postgresql://postgres:Pass@word@localhost:5432/taskmanager -f schema.sql
```

3. Seed data:

```bash
psql postgresql://postgres:Pass@word@localhost:5432/taskmanager -f seed.sql
```

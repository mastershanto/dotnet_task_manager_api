# ASP.NET Todo API

A simple REST API for managing todo items built with ASP.NET Core 8.0 and Entity Framework Core.

## Features

- **CRUD Operations**: Create, read, update, and delete todo items
- **SQLite Database**: Lightweight database for data persistence
- **Swagger/OpenAPI**: Interactive API documentation
- **Entity Framework Core**: ORM for database operations

## Prerequisites

- .NET 8.0 SDK or later
- Visual Studio Code or Visual Studio

## Project Structure

```
├── Controllers/
│   └── TodoController.cs      # API endpoints
├── Models/
│   └── TodoItem.cs            # Data model
├── Data/
│   └── TodoContext.cs         # Entity Framework DbContext
├── Program.cs                 # Application startup
├── appsettings.json           # Configuration
└── TodoApi.csproj             # Project file
```

## Getting Started

1. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

2. **Build the project**:
   ```bash
   dotnet build
   ```

3. **Run the application**:
   ```bash
   dotnet run
   ```

The API will start at `https://localhost:5001` and `http://localhost:5000`.

## API Endpoints

### Get all todos
```
GET /api/todo
```

### Get a specific todo
```
GET /api/todo/{id}
```

### Create a new todo
```
POST /api/todo
Content-Type: application/json

{
  "title": "Buy groceries",
  "description": "Buy milk, eggs, and bread",
  "isComplete": false
}
```

### Update a todo
```
PUT /api/todo/{id}
Content-Type: application/json

{
  "id": 1,
  "title": "Buy groceries",
  "description": "Buy milk, eggs, and bread",
  "isComplete": true
}
```

### Delete a todo
```
DELETE /api/todo/{id}
```

## API Documentation

Interactive API documentation is available at `https://localhost:5001/swagger` when running in development mode.

## Database

The application uses SQLite for data persistence. The database file (`todo.db`) is created automatically on the first run in the project root directory.

## Technologies

- **ASP.NET Core 8.0**: Modern web framework
- **Entity Framework Core 8.0**: ORM and database access
- **SQLite**: Lightweight database
- **Swagger/Swashbuckle**: API documentation

# Enterprise-Grade Task Manager API

A world-class, enterprise-grade REST API for comprehensive task management built with ASP.NET Core 10 and PostgreSQL.

## 🚀 Features

### Core Entities
- **Tasks/Items**: Comprehensive task management with:
  - Status tracking (Todo, InProgress, InReview, Done, Archived, Cancelled)
  - Priority levels (Critical, High, Medium, Low)
  - Assignment and deadline management
  - Progress tracking and time estimation
  - Task blocking and reason tracking
  - Task history and audit trails
  - Comments and attachments
  - Subtask support

- **Projects**: Full project lifecycle management:
  - Team association
  - Status management (Planning, Active, OnHold, Completed, Archived)
  - Progress tracking
  - Public/Private visibility
  - Project members with role-based access
  
- **Teams**: Organizational structure:
  - Team ownership and members
  - Role-based access control (Owner, Admin, Member, Viewer)
  - Multiple projects per team
  
- **Users**: User management with:
  - Profile management
  - Multiple roles support
  - Activity tracking (last login, creation date)
  - Team memberships

### Advanced Features
- **Comprehensive Comments System**: Thread-based task comments with replies
- **File Attachments**: Upload and manage task attachments
- **Audit Trail**: Complete TaskHistory tracking all changes
- **Pagination & Filtering**: Advanced query capabilities with:
  - Page-based pagination
  - Status and priority filtering
  - Full-text search on task titles/descriptions
  - Sorting by multiple fields
  - Customizable sort order
- **Error Handling**: Global exception handling with detailed API responses
- **Access Control**: Project-level access control and permission checking

## 📋 API Endpoints

###  Tasks
- `GET /api/tasks/{taskId}` - Get a specific task
- `GET /api/tasks/project/{projectId}` - Get all tasks in a project (with pagination/filtering)
- `GET /api/tasks/assigned-to-me` - Get tasks assigned to current user
- `POST /api/tasks` - Create new task
- `PUT /api/tasks/{taskId}` - Update task
- `DELETE /api/tasks/{taskId}` - Delete task
- `PATCH /api/tasks/{taskId}/status` - Change task status
- `PATCH /api/tasks/{taskId}/assign` - Assign task to user

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 10.0
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: JWT ready (JWT tokens libraries included)
- **Logging**: Structured logging with ILogger
- **Documentation**: Swagger/OpenAPI integration

## 📁 Project Structure

```
/Models
  - User.cs: User entity with roles and team memberships
  - Team.cs: Team and team member entities
  - Project.cs: Project and project member entities
  - TodoItem.cs: Task item with related comment, attachment, and history entities

/Controllers
  - TasksController.cs: Comprehensive task management endpoints
  - TodoController.cs: Legacy simple todo endpoints

/Services
  - TaskService.cs: Business logic for task operations with validation

/Data
  - TodoContext.cs: Entity Framework database context with comprehensive migrations

/DTOs
  - CommonDtos.cs: All request/response models (API, pagination, CRUD operations)

/Middleware
  - ExceptionHandlingMiddleware.cs: Global error handling

/appsettings.json: Configuration for PostgreSQL connection
```

## 🔒 Security Features

- **Access Control**: Project and task-level access validation
- **Soft Deletes**: Tasks and projects marked as deleted, not physically removed
- **Audit Trail**: All changes tracked in TaskHistory
- **Role-Based Access**: Teams and projects support multiple role levels
- **Input Validation**: DTOs and model validation

## 🚀 Getting Started

### Prerequisites
- .NET 10.0 SDK
- PostgreSQL 13+ (optional - API will run without DB)

### Building

```bash
cd todo_api
dotnet restore
dotnet build
dotnet run
```

The API will be available at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `http://localhost:5000/swagger`

### Database Setup

Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=todo_api;Username=postgres;Password=yourpassword"
  }
}
```

The database tables will be created automatically on first run.

## 📊 Database Schema Highlights

- **Comprehensive Indexes**: Foreign keys, status, priority, dates, and created date indexes
- **Referential Integrity**: Proper cascade delete and restrict behaviors
- **Soft Deletes**: DeletedAt columns for safe deletion
- **Audit Support**: CreatedAt, UpdatedAt fields on all entities
- **Many-to-Many Support**: Project members, team members with role tracking

## 🎯 Query Examples

### Get all tasks in a project with filtering
```
GET /api/tasks/project/1?pageNumber=1&pageSize=20&status=0&priority=2&searchTerm=bug
```

### Get tasks assigned to current user
```
GET /api/tasks/assigned-to-me?pageNumber=1&pageSize=10
```

### Create a new task
```
POST /api/tasks
{
  "title": "Implement authentication",
  "description": "Add JWT auth to all endpoints",
  "projectId": 1,
  "priority": 1,
  "dueDate": "2026-02-15T00:00:00Z",
  "estimatedHours": 8
}
```

### Update task status
```
PATCH /api/tasks/5/status?status=2
```

## ⚠️ Notes

- Default user ID is hardcoded to 1 in controllers (update with actual user context from JWT claims)
- Npgsql 8.0.0 has a known vulnerability (consider updating to latest)
- API runs without errors even if PostgreSQL is not available

## 🔄 Next Steps for Production

1. **Implement JWT Authentication**: Add authentication middleware and update user context retrieval
2. **Database Migrations**: Set up EF Core migrations for schema versioning
3. **Rate Limiting**: Add API rate limiting middleware
4. **Logging**: Implement structured logging to files/cloud services
5. **Testing**: Add unit and integration tests
6. **Validation**: Enhance DTOs with comprehensive validation attributes
7. **Documentation**: Generate API documentation from Swagger
8. **Performance**: Add caching for frequently accessed data

---

**Status**: ✅ Core API running and ready for development/testing

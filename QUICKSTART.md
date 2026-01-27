# 🚀 Quick Start Guide - Enterprise Todo API

## Prerequisites Checklist
- [ ] .NET 8.0 SDK installed
- [ ] Docker Desktop running (for containerized deployment)
- [ ] PostgreSQL client (optional, for local development)
- [ ] Git

---

## 🎯 Option 1: Docker Compose (Fastest - Recommended)

### Step 1: Clone and Configure
```bash
git clone <your-repo-url>
cd dotnet_task_manager_api

# Copy environment template
cp .env.example .env
```

### Step 2: Generate Secrets
```bash
# Generate a secure JWT secret key (minimum 32 characters)
openssl rand -base64 48

# Generate PostgreSQL password
openssl rand -base64 24

# Generate Redis password
openssl rand -base64 24
```

### Step 3: Edit .env File
```bash
# Open .env in your editor
nano .env  # or use any text editor

# Set these required values:
JWT_SECRET_KEY=<paste-generated-jwt-secret>
POSTGRES_PASSWORD=<paste-postgres-password>
REDIS_PASSWORD=<paste-redis-password>
```

### Step 4: Launch Everything
```bash
# Start all services (API + PostgreSQL + Redis)
docker-compose up -d

# Watch logs
docker-compose logs -f api
```

### Step 5: Verify Deployment
```bash
# Health check
curl http://localhost:5000/health/live

# Expected response:
# {"status":"Healthy","timestamp":"2026-01-26T..."}
```

### Step 6: Access Swagger
Open your browser: **http://localhost:5000/swagger**

✅ **You're ready to go!**

---

## 🎯 Option 2: Local Development (Without Docker)

### Step 1: Setup PostgreSQL
```bash
# Install PostgreSQL (macOS with Homebrew)
brew install postgresql@14
brew services start postgresql@14

# Create database
createdb todo_api

# Create user
psql postgres -c "CREATE USER todoapi WITH PASSWORD 'your_password';"
psql postgres -c "GRANT ALL PRIVILEGES ON DATABASE todo_api TO todoapi;"
```

### Step 2: Setup Redis
```bash
# Install Redis (macOS with Homebrew)
brew install redis
brew services start redis
```

### Step 3: Configure Application
```bash
cd dotnet_task_manager_api

# Update appsettings.Development.json
# Change JWT Key to a secure value (minimum 32 characters)
# Update connection strings for PostgreSQL and Redis
```

### Step 4: Restore and Run
```bash
# Restore NuGet packages
dotnet restore

# Apply database migrations
dotnet ef database update

# Run the application
dotnet run
```

### Step 5: Access the API
- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5000
- **Swagger**: https://localhost:5001/swagger

---

## 🧪 Test the API

### 1. Register a User
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "SecurePassword123!"
  }'
```

### 2. Login and Get Token
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "SecurePassword123!"
  }'

# Save the returned token
export TOKEN="<your-jwt-token>"
```

### 3. Create a Task
```bash
curl -X POST http://localhost:5000/api/v1/tasks \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "title": "My First Task",
    "description": "This is a test task",
    "priority": "High",
    "status": "Todo"
  }'
```

### 4. Get All Tasks
```bash
curl -X GET http://localhost:5000/api/v1/tasks \
  -H "Authorization: Bearer $TOKEN"
```

---

## 🐛 Troubleshooting

### Docker Issues

**Problem**: Port already in use
```bash
# Find and kill process using port 5000
lsof -ti:5000 | xargs kill -9

# Or change port in docker-compose.yml
ports:
  - "5555:8080"  # Use 5555 instead
```

**Problem**: Database connection failed
```bash
# Check PostgreSQL container
docker-compose ps

# View PostgreSQL logs
docker-compose logs postgres

# Restart PostgreSQL
docker-compose restart postgres
```

### Local Development Issues

**Problem**: Database migration failed
```bash
# Drop and recreate database
dotnet ef database drop --force
dotnet ef database update
```

**Problem**: Redis connection failed
```bash
# Check Redis status
redis-cli ping
# Expected: PONG

# Restart Redis
brew services restart redis
```

**Problem**: JWT configuration error
```bash
# Ensure JWT Key is at least 32 characters
# Edit appsettings.Development.json
# Set: "Key": "your-secret-key-at-least-32-characters-long"
```

---

## 📊 Monitoring

### View Application Logs
```bash
# Docker
docker-compose logs -f api

# Local
tail -f logs/todoapi-*.txt
```

### Check Health
```bash
# Liveness probe
curl http://localhost:5000/health/live

# Detailed health (includes DB and Redis)
curl http://localhost:5000/health
```

### Monitor Redis
```bash
# Connect to Redis
docker exec -it todo-redis redis-cli

# Or local Redis
redis-cli

# Inside redis-cli:
AUTH your_redis_password
INFO stats
KEYS *
```

### Monitor PostgreSQL
```bash
# Connect to database
docker exec -it todo-postgres psql -U todoapi -d todo_api

# Inside psql:
\dt  # List tables
\d+ "TaskItems"  # Describe table
SELECT COUNT(*) FROM "TaskItems";
```

---

## 🔐 Security Checklist

Before going to production:

- [ ] Change all default passwords
- [ ] Generate strong JWT secret (minimum 48 characters)
- [ ] Use environment variables for all secrets
- [ ] Enable HTTPS (use reverse proxy like nginx)
- [ ] Configure CORS for your frontend domain
- [ ] Set up Azure Key Vault or AWS Secrets Manager
- [ ] Enable firewall rules
- [ ] Configure rate limiting for your traffic
- [ ] Set up SSL certificates
- [ ] Review security headers configuration

---

## 🚢 Deploy to Production

### Using Docker Compose (Simple Production)
```bash
# Set environment to Production
export ASPNETCORE_ENVIRONMENT=Production

# Use production compose file
docker-compose -f docker-compose.yml up -d
```

### Using Kubernetes
See [ENTERPRISE_DEPLOYMENT_GUIDE.md](ENTERPRISE_DEPLOYMENT_GUIDE.md) for complete K8s deployment.

### Using Azure Container Apps
```bash
az containerapp up \
  --name todo-api \
  --resource-group todo-rg \
  --environment todo-env \
  --image <your-registry>/todo-api:latest \
  --target-port 8080 \
  --ingress external
```

---

## 📚 Next Steps

1. **Read Full Documentation**: [ENTERPRISE_DEPLOYMENT_GUIDE.md](ENTERPRISE_DEPLOYMENT_GUIDE.md)
2. **Understand Architecture**: [ARCHITECTURE_REFACTORING.md](ARCHITECTURE_REFACTORING.md)
3. **API Reference**: [ENTERPRISE_API.md](ENTERPRISE_API.md)
4. **Customize**: Modify domain models in `/Domain/Entities/`
5. **Add Features**: Create new CQRS handlers in `/Application/Features/`

---

## 💡 Tips

- Use **Swagger UI** for interactive API testing
- Check **health endpoints** regularly
- Review **logs** for errors and performance insights
- Use **Redis** caching for frequently accessed data
- Configure **rate limiting** based on your traffic patterns
- Set up **CI/CD pipeline** for automated deployments

---

## 🆘 Getting Help

- **Issues**: Open a GitHub issue
- **Questions**: Check documentation in `/docs` folder
- **Security**: Report security issues privately

**Happy coding! 🎉**

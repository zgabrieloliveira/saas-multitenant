# 🚀 SaaS Multi-tenant API (.NET 9)

This project is a **SaaS Backend Boilerplate** built with **.NET 9**, implementing a secure and isolated Multi-tenant architecture.

The system uses the **Column Isolation Strategy** (all tenants share the same database, distinguished by a `TenantId`), where security and data filtering are applied automatically at the Infrastructure layer. This makes the isolation transparent to the business logic developer, preventing data leaks.

## 🧠 Architecture & Design Patterns

The project follows **Clean Architecture** principles, divided into:

- **🏗️ Core:** Domain Entities, Interfaces, and pure Business Logic (no external dependencies).
- **⚙️ Infra:** Data Access implementation (EF Core), Migrations, Concrete Services, and Dependency Injection.
- **🔌 Api:** Controllers, Middlewares, DTOs, and Swagger Configuration.

### Key Features
- **Automatic Isolation:** Leverages EF Core **Global Query Filters**. Developers do not need to write `Where(t => t.TenantId == ...)` in any query.
- **Tenant Resolution Middleware:** Intercepts the `X-Tenant-ID` header and sets up the Scoped context before the Controller is invoked.
- **Write Security:** `SaveChanges` automatically overrides the `TenantId` on insertion, preventing data leakage due to human error.
- **Global Exception Handling:** Implements `IExceptionHandler` returning standard **RFC 7807 (Problem Details)** responses.
- **Smart Swagger:** Configured to accept the Tenant header in all requests automatically.

## 🛠️ Tech Stack

- **.NET 9** (C# 12)
- **PostgreSQL** (Database)
- **Entity Framework Core** (ORM)
- **Docker & Docker Compose** (Containerization)
- **Swagger / OpenAPI** (Documentation)

## ⚡ Quick Start

### Prerequisites
- Docker and Docker Compose installed.

### 1. Start Infrastructure

Run the Database and API containers:

```bash
docker compose up -d --build
```

### 3. How to Test

Create a product for Company A:

```bash
curl -X POST http://localhost:5000/Api/Products \
  -H "Content-Type: application/json" \
  -H "X-Tenant-ID: 11111111-1111-1111-1111-111111111111" \
  -d '{
    "name": "Company A Secret",
    "description": "Confidential data",
    "price": 5000
  }'
```

Create a product for Company B:

```bash
curl -X POST http://localhost:5000/Api/Products \
  -H "Content-Type: application/json" \
  -H "X-Tenant-ID: 22222222-2222-2222-2222-222222222222" \
  -d '{
    "name": "Company B Data",
    "description": "Exclusive data",
    "price": 9000
  }'
```

Verify Isolation - Try to access data using Company A credentials. You should NOT see Company B's data:

```bash
curl -X GET http://localhost:5000/Api/Products \
  -H "X-Tenant-ID: 11111111-1111-1111-1111-111111111111"
```

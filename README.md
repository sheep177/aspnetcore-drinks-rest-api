
# Drinks API 🍹

A **production-style ASP.NET Core Web API** demonstrating real-world backend patterns,  
including **CRUD, search, filtering, dynamic sorting, pagination, caching, ETag-based concurrency control, JWT authentication**, and Swagger documentation.

This project is intentionally designed as a **resume-ready backend system**, not a toy demo.

---

## 🚀 Key Features

### Core API
- RESTful CRUD API for Drinks
- DTO-based input/output separation
- Clean Controller / Service / Repository layering

### Query Capabilities
- 🔍 Search (fuzzy match on Name / Brand)
- 🏷️ Filter (exact Brand filter)
- ↕️ Dynamic sorting (multi-field, asc/desc)
- 📄 Pagination with metadata

### Performance & Correctness
- ⚡ In-memory caching for list endpoints
- 🧠 Cache invalidation on write operations
- 🏷️ HTTP ETag support for GET
- 🔒 Optimistic concurrency control via If-Match
- 🧩 PATCH support with partial updates

### Security & Tooling
- 🔐 JWT Bearer Authentication
- 🔒 `[Authorize]` protected endpoints
- 📘 Swagger / OpenAPI documentation
- 🧪 Developer-friendly JWT generation (`dotnet user-jwts`)

---

## 🏗️ Tech Stack

- **.NET 8 / ASP.NET Core Web API**
- **Entity Framework Core**
- **SQLite**
- **JWT Bearer Authentication**
- **AutoMapper**
- **Swagger (Swashbuckle)**
- **System.Linq.Dynamic.Core**
- **IMemoryCache**

---

## 📁 Project Structure

Drinks.API  
│  
├── Controllers  
│   └── DrinkController.cs        // HTTP semantics, headers, status codes  
│  
├── Services  
│   ├── IDrinkService.cs  
│   └── DrinkService.cs           // Business logic, caching, ETag handling  
│  
├── Repositories  
│   ├── IDrinkRepo.cs  
│   └── DrinkRepo.cs              // EF Core queries & IQueryable composition  
│  
├── Entities  
│   └── Drink.cs                  // EF entities + RowVersion for concurrency  
│  
├── Models (DTOs)  
│   ├── DrinksDto.cs  
│   ├── DrinksForCreationDto.cs  
│   ├── DrinksForUpdateDto.cs  
│   └── DrinksPatchDto.cs  
│  
├── ResourceParameters  
│   └── DrinksResourceParameters.cs // Query binding (search/filter/sort/page)  
│  
├── Helpers  
│   ├── PagedList.cs              // Pagination container  
│   ├── IQueryableExtensions.cs   // Dynamic sorting  
│   └── DrinkPropertyMapping.cs   // Safe field mapping  
│  
├── Profiles  
│   └── DrinkProfile.cs           // AutoMapper configuration  
│  
├── DbContext  
│   └── DrinkInfoContext.cs  
│  
├── Program.cs  
├── appsettings.json  
└── drinks.db

---

## 🔐 Authentication

This API uses **JWT Bearer authentication**.

All `/api/drinks` endpoints require authorization:

```csharp
[Authorize]

Token Validation

Configured in Program.cs:
	•	Issuer validation
	•	Audience validation
	•	Signature validation
	•	Symmetric key

⸻

🧪 Local Development

No deployment required.
No external services required.

1️⃣ Clone the project

git clone <your-repo-url>
cd Drinks.API

2️⃣ Restore & run

dotnet restore
dotnet run

3️⃣ Open Swagger

https://localhost:{PORT}/swagger


⸻

🔑 Generate a Test JWT (Recommended)

During development:

dotnet user-jwts create \
  --issuer DrinksAPI \
  --audience DrinksClient \
  --claim "role=User"

Use the token in requests:

Authorization: Bearer <token>

Swagger UI also supports Bearer tokens.

⸻

📄 Pagination Metadata

Pagination metadata is returned via response headers, not the response body:

X-Pagination: {
  "totalCount": 42,
  "pageSize": 10,
  "currentPage": 1,
  "totalPages": 5,
  "hasPrevious": false,
  "hasNext": true
}

The response body contains only the data collection.

⸻

🧠 Concurrency & ETag Support

GET with ETag
	•	Responses include ETag header
	•	Clients may send If-None-Match
	•	Server returns 304 Not Modified if unchanged

PUT / PATCH with If-Match
	•	Clients must send If-Match
	•	Server enforces optimistic concurrency
	•	Conflicts return 412 Precondition Failed

This prevents lost updates in concurrent scenarios.

⸻

📌 Example Endpoints

Method	Endpoint	Description
GET	/api/drinks	List drinks (search / filter / sort / paging)
GET	/api/drinks/{id}	Get drink by id (ETag-enabled)
POST	/api/drinks	Create drink
PUT	/api/drinks/{id}	Full update (If-Match required)
PATCH	/api/drinks/{id}	Partial update (If-Match required)
DELETE	/api/drinks/{id}	Delete drink


⸻

🧠 Design Principles
	•	Separation of concerns (Controller / Service / Repository)
	•	Deferred execution via IQueryable
	•	Header-based metadata (pagination, ETag)
	•	Explicit HTTP semantics
	•	Optimistic concurrency
	•	Resume-oriented readability

⸻

🎯 Resume Description (Copy–Paste)

Built a production-style ASP.NET Core Web API implementing CRUD operations with search, filtering, dynamic sorting, pagination, caching, and HTTP ETag-based optimistic concurrency control. Designed with clean architecture principles using Controller–Service–Repository layers, EF Core with SQLite, AutoMapper for DTO mapping, JWT Bearer authentication, and Swagger documentation.

⸻

✅ Project Status

✔ Resume-ready
✔ Cold-start friendly
✔ Swagger-documented
✔ Real-world API patterns

⸻

📎 Notes
	•	Authentication controller intentionally omitted
	•	Tokens generated via dotnet user-jwts
	•	Designed for backend / API-focused roles


<<<<<<< HEAD
# aspnetcore-drinks-rest-api
ASP.NET Core Web API demo showcasing clean architecture, CRUD operations, JWT authentication, search, filtering, paging, and Swagger documentation.
=======

# Drinks API 🍹

A clean, production-style **ASP.NET Core Web API** demonstrating  
**CRUD + Search + Filter + Pagination + JWT Authentication + Swagger documentation**.

This project is designed as a **resume-ready backend project**, following common industry patterns.

---

## 🚀 Features

- RESTful CRUD API for Drinks
- Search (fuzzy match on Name / Brand)
- Filter (exact Brand filter)
- Pagination with metadata in response headers
- JWT Bearer Authentication
- Protected endpoints with `[Authorize]`
- Swagger / OpenAPI documentation
- Entity Framework Core + SQLite
- AutoMapper for DTO mapping
- PATCH support via JSON Patch
- Clean separation of concerns (Controller / Repo / DTO / Entity)

---

## 🏗️ Tech Stack

- **.NET 8 / ASP.NET Core Web API**
- **Entity Framework Core**
- **SQLite**
- **JWT Bearer Authentication**
- **AutoMapper**
- **Swagger (Swashbuckle)**
- **JSON Patch**

---

## 📁 Project Structure

Drinks.API
│
├── Controllers
│   └── DrinkController.cs
│
├── DbContext
│   └── DrinkInfoContext.cs
│
├── Entities
│   └── Drink.cs
│
├── Models (DTOs)
│   ├── DrinksDto.cs
│   ├── DrinksForCreationDto.cs
│   ├── DrinksForUpdateDto.cs
│   └── DrinksPatchDto.cs
│
├── Profiles
│   └── DrinkProfile.cs
│
├── Services
│   ├── IDrinkRepo.cs
│   ├── DrinkRepo.cs
│   └── PaginationMetadata.cs
│
├── Program.cs
├── appsettings.json
└── drinks.db

---

## 🔐 Authentication

This API uses **JWT Bearer authentication**.

All endpoints under `/api/drinks` are protected:

```csharp
[Authorize]

Token Validation

Configured in Program.cs:
	•	Issuer validation
	•	Audience validation
	•	Signature validation
	•	Symmetric key (Base64)

⸻

🧪 Local Development (Cold Start Friendly)

No deployment required.
No external services required.

1️⃣ Clone the project

git clone <your-repo-url>
cd Drinks.API

2️⃣ Restore & run

dotnet restore
dotnet run

3️⃣ Open Swagger UI

https://localhost:{PORT}/swagger

Swagger is enabled in Development environment by default.

⸻

🔑 Generating a Test JWT (Recommended)

During development, use dotnet user-jwts:

dotnet user-jwts create --issuer DrinksAPI --audience DrinksClient --claim "city=Antwerp"

Copy the generated token and use it as:

Authorization: Bearer <token>

Swagger UI also supports Bearer tokens.

⸻

📄 Pagination Metadata

Pagination metadata is returned via response headers:

X-Pagination:
{
  "totalItemCount": 42,
  "pageSize": 10,
  "pageNumber": 1,
  "totalPageCount": 5
}


⸻

📌 Example Endpoints

Method	Endpoint	Description
GET	/api/drinks	List drinks (search / filter / paging)
GET	/api/drinks/{id}	Get drink by id
POST	/api/drinks	Create drink
PUT	/api/drinks/{id}	Update drink
PATCH	/api/drinks/{id}	Partial update
DELETE	/api/drinks/{id}	Delete drink


⸻

🧠 Design Principles
	•	Repository pattern
	•	DTO separation
	•	Deferred execution with IQueryable
	•	API-first design
	•	Header-based metadata
	•	Minimal controller logic
	•	Resume-oriented readability

⸻

🎯 Resume Description (Copy-Paste)

Built a secure ASP.NET Core Web API featuring CRUD operations, search, filtering, pagination with metadata, JWT authentication, and Swagger documentation. Implemented clean architecture with repository pattern, DTO mapping via AutoMapper, EF Core with SQLite, and protected endpoints using JWT Bearer authentication.

⸻

✅ Status

✔ Resume-ready
✔ Cold-start friendly
✔ Swagger-documented
✔ Enterprise-style structure

⸻

📎 Notes
	•	Authentication controller intentionally omitted
	•	Tokens generated via dotnet user-jwts (industry-friendly approach)
	•	Suitable for backend / API-focused roles

⸻

>>>>>>> 4609de6 (Initial commit: Drinks API (CRUD + search/filter/paging + JWT + Swagger))

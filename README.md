# UserAnimeList API

RESTful API for managing personal anime lists.

This project allows users to search, filter and rate animes, as well as manage their personal anime list and profile image.

Built with focus on clean architecture, separation of concerns and performance.

The focus of the project is the backend, the frondend is completely generated using Codex and is available for demonstrative purposes only, check frontend readme for more details.

---

## Technologies

- ASP.NET Core
- Entity Framework Core
- Clean Architecture
- FluentValidation
- AutoMapper
- xUnit
- Moq
- Swagger (For API mapping purposes only, no UI)
- Scalar

---

## Architecture

The project follows Clean Architecture principles:


```text
UserAnimeList
│
├── WebApi
│   └── Controllers
│
├── Application
│   └── UseCases
│
├── Domain
│   ├── Entities
│   └── Enums
│
├── Infrastructure
│   ├── Repositories
│   └── DbContext
│
Tests
```


### Responsibilities

- Controller → Handles HTTP requests
- UseCase → Business rules and orchestration
- Repository → Data access logic
- Domain → Core entities and rules

---

## Features

- User registration
- Profile image upload and removal
- Anime search
- Advanced anime filtering
- Average score calculation
- Sorting by multiple fields
- Validation rules
- Unit tests for UseCases
- Repository mocks for isolation

---

## Anime Search & Filter

### Search

Search animes by name or synopsis.

### Advanced Filter

Supported filters:

- Query
- Status
- Type
- Genres
- Studios
- Aired date range
- Premiered season + year
- Sorting (Name, Episodes, Status, Type, AiredFrom, Score, etc.)

---

## Score Calculation

The average score is calculated at the database query level to avoid N+1 queries.

- Only active entries are considered
- Null scores are ignored
- Rounding is applied in the UseCase layer
- Repository returns raw average
- UseCase applies `Math.Round(..., MidpointRounding.AwayFromZero)`

This ensures:
- Separation of concerns
- Better performance
- Testable business logic

---

## Validation Rules

Implemented using FluentValidation.

Examples:

- Query max length: 256 characters
- Status must be a valid enum value
- Genres cannot contain duplicates
- Premiered season requires year
- Invalid filters throw `ErrorOnValidationException`

---

## Image Handling

- Images are stored as file paths
- Files are physically removed when deleted
- Database field is set to null on delete
- Designed to avoid storing large binary data in database

---

## Performance Considerations

- `AsNoTracking()` used for read operations
- Aggregation (Average) performed at database level
- DTO projection to avoid loading full entities
- Avoidance of N+1 queries
- Score calculation included directly in query

---

## Configuration

By default, the project runs in Development environment.

You can configure the database connection using:

- appsettings.Development.json
- Environment Variables
- dotnet user-secrets (recommended for development)

Environment variables override JSON configuration.

You can follow the example:
```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YourConnectionString"
```
## JWT Configuration

The project requires a JWT signing key.

For development, use:

```bash
dotnet user-secrets set "Settings:JWT:SigningKey" "your_secure_key_here"
```
For production, configure environment variable:
```bash
Settings__JWT__SigningKey
```
Never commit real signing keys to the repository.

## Running Tests

---
## Authorization & Roles

Currently, the system supports role-based authorization.

### Default Behavior

- All newly registered users are created with the **User** role.
- There is currently no public endpoint to promote a user to **Admin**.
- To grant Admin privileges, the role must be changed directly in the database.

### Database Seeder

The system includes a default admin seeded user:

- Username: anime_master

This user is created during database initialization and can be used for administrative testing purposes.

---

Run all tests:

```bash
dotnet test
```

Tests include:

- UseCase unit tests
- Validation error scenarios
- Repository behavior simulation with mocks

---

## Running the Project

1. Clone the repository

```bash
git clone https://github.com/Digoblon/UserAnimeList.git
```

2. Make sure you are in the correct folder

```bash
cd UserAnimeList\src\Backend\UserAnimeList.API
```

3. Apply migrations

```bash
dotnet ef database update
```

4. Run the project

```bash
dotnet run
```

Scalar will be available at:

```bash
https://localhost:<port>/scalar/v1
```

The default port should be 5103, if you want to change it you can modify launchSettings.json to your liking.
 

---

## Design Decisions

### Why calculate Score in repository?

To avoid N+1 queries and improve performance.

### Why round score in UseCase?

To keep separation between data access and business rules.

### Why return DTO instead of entity?

To avoid exposing domain internals and improve API stability.

### Why store image as path instead of blob?

To reduce database size and improve scalability.

---

## API Examples

### Create User

```bash
POST /user
```

Request:
```json
{
  "userName": "username",
  "email": user@email.com,
  "password": Password123,
  "confirmPassword": Password123
}
```

Response;
```json
{
  "userName": "username",
  "tokens": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "vkOMuDfCHUnbPvU_TGm_CZIaK2B..."
  }
}
```
### Authentication
```bash
POST /login
```
Request:

```json
{
  "email": "user@email.com",
  "password": "Password123"
}
```
Response:
```json
{
  "userName": "username",
  "tokens": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "vkOMuDfCHUnbPvU_TGm_CZIaK2B..."
  }
}
```
### Search Anime

```bash
GET /anime/search?query=naruto
```
Response:
```json
{
  "animes": [
    {
      "id": "123e4567-e89b-12d3-a456-426614174000",
      "name": "Naruto",
      "imageUrl": null,
      "score": 8.5,
      "status": "Airing",
      "type": "Tv",
      "airedFrom": null,
      "airedUntil": null
    }
  ]
}
```
---

## Future Improvements

- Admin promotion endpoint
- Role management panel
- More granular permission system
- Enrich responses for `display models`
- Create plug `slug` for browsing
- Create filters for name/slug
- lookup/autocomplete dedicated endpoints
- More complete `me` endpoints

---

## License

This project is for educational and portfolio purposes.
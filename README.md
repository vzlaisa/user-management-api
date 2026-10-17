# UserManagementAPI

A small REST API for managing users, built with ASP.NET Core (.NET 10) Minimal APIs. All endpoints are protected with JWT Bearer authentication.

## Features

- Create, read, update, and delete users
- JWT Bearer authentication (HMAC-SHA256)
- Input validation (name, email, department)
- Centralized error handling and request logging
- OpenAPI document in Development
- In-memory, thread-safe storage (data is lost when the app stops)

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Getting Started

1. **Set the JWT signing key** (at least 32 characters). It is not stored in the repo, so use user secrets:

   ```bash
   dotnet user-secrets set "Authentication:JwtBearer:SigningKey" "your-secret-key-of-at-least-32-characters"
   ```

2. **Run the API:**

   ```bash
   dotnet run
   ```

   The API starts at `http://localhost:5148`.

3. **Get a token.** This project does not issue tokens. Use a JWT signed with the same key, with issuer `TechHive` and audience `UserManagementAPI`, and send it as:

   ```
   Authorization: Bearer <your-token>
   ```

## Endpoints

| Method | Route             | Description       | Success | Errors        |
|--------|-------------------|-------------------|---------|---------------|
| GET    | `/api/users`      | List all users    | 200     | 401           |
| GET    | `/api/users/{id}` | Get a user by ID  | 200     | 401, 404      |
| POST   | `/api/users`      | Create a user     | 201     | 400, 401      |
| PUT    | `/api/users/{id}` | Update a user     | 200     | 400, 401, 404 |
| DELETE | `/api/users/{id}` | Delete a user     | 204     | 401, 404      |

### Example request body (POST / PUT)

```json
{
  "name": "Ana García",
  "email": "ana.garcia@techhive.example",
  "department": "Human Resources"
}
```

### Validation rules

- `name`: required, up to 200 characters
- `email`: required, valid email, up to 254 characters
- `department`: required, up to 100 characters

## Configuration

Set in `appsettings.json`:

| Setting                             | Default            |
|-------------------------------------|--------------------|
| `Authentication:JwtBearer:Issuer`   | `TechHive`         |
| `Authentication:JwtBearer:Audience` | `UserManagementAPI`|
| `Authentication:JwtBearer:SigningKey` | *(set via user secrets)* |

The app will not start if the signing key is missing or shorter than 32 bytes.

## Try It Out

Open `UserManagementAPI.http` in Visual Studio or VS Code (REST Client) to run sample requests. Replace the `@token` variable with your JWT first. In Development, the OpenAPI document is available at `/openapi/v1.json`.

## Project Structure

```
UserManagementAPI/
├── Endpoints/     # Route definitions (/api/users)
├── Middleware/    # Error handling, request logging, JWT settings
├── Models/        # Request and response types
├── Services/      # User business logic and in-memory storage
└── Program.cs     # App setup and configuration
```

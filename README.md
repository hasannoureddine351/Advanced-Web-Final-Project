# UniSolve

A full-stack web application where students post university problems and verified users post solutions. Solutions are ranked by community upvotes/downvotes.

## Tech Stack

- **Frontend:** React, Vite, React Router, React Bootstrap, Axios
- **Backend:** ASP.NET Core 8 Web API, JWT Authentication, Entity Framework Core
- **Database:** Microsoft SQL Server Express
- **Deployment:** Single ASP.NET process serving API + React SPA from `wwwroot`

## Prerequisites

1. [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. [SQL Server 2022 Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
3. [Node.js 18+](https://nodejs.org/)

Install via winget (Windows):

```powershell
winget install Microsoft.DotNet.SDK.8
winget install Microsoft.SQLServer.2022.Express
```

After installing SQL Server Express, ensure the instance `SQLEXPRESS` is running. The default connection string uses Windows Authentication.

## Getting Started

### 1. Clone and install dependencies

```powershell
cd frontend/unisolve-client
npm install
```

### 2. Configure database

The connection string in `backend/UniSolve.Api/appsettings.json`:

```
Server=localhost\SQLEXPRESS;Database=UniSolveDb;Trusted_Connection=True;TrustServerCertificate=True
```

Update if your SQL Server instance name differs.

**Start SQL Server** (requires Administrator PowerShell):

```powershell
# Run PowerShell as Administrator, then:
Start-Service -Name 'MSSQL$SQLEXPRESS'
# Or run: .\start-sql.ps1
```

Verify it is running: `Get-Service 'MSSQL$SQLEXPRESS'` should show `Running`.

### 3. Apply database migrations

```powershell
cd backend/UniSolve.Api
dotnet ef database update
```

Migrations also run automatically on first startup via seed logic.

### 4. Development mode (two terminals)

**Terminal 1 — Backend:**
```powershell
cd backend/UniSolve.Api
dotnet run
```
API runs at `http://localhost:5289`

**Terminal 2 — Frontend:**
```powershell
cd frontend/unisolve-client
npm run dev
```
React dev server at `http://localhost:5173` (proxies `/api` to backend)

### 5. Production build & publish

```powershell
.\publish.ps1
```

Or manually:

```powershell
cd frontend/unisolve-client
npm run build

cd ../../backend/UniSolve.Api
dotnet publish -c Release -o ./publish
```

Run the published app:

```powershell
cd backend/UniSolve.Api/publish
dotnet UniSolve.Api.dll
```

Open `http://localhost:5289` — single server serves both API and React UI.

## Default Accounts

| Role     | Email               | Password   |
|----------|---------------------|------------|
| Admin    | admin@unisolve.com  | Admin123!  |
| Verified | tutor@unisolve.com  | Tutor123!  |

Register a new account to get the **Student** role (can post problems and vote).

## Features

- User registration & login with JWT
- Protected routes (React) and `[Authorize]` (API)
- Post, edit, delete problems (students)
- Post solutions (verified users, moderators, admins)
- Upvote/downvote solutions (sorted by score)
- Filter problems by subject, search by keyword
- View all problems / my posts
- Admin subject management (CRUD)

## API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/register` | No | Register student |
| POST | `/api/auth/login` | No | Login, get JWT |
| GET | `/api/auth/me` | Yes | Current user |
| GET | `/api/subjects` | No | List subjects |
| POST/PUT/DELETE | `/api/subjects/{id}` | Admin | Subject CRUD |
| GET | `/api/problems` | No | List problems (filter/search) |
| GET | `/api/problems/{id}` | No | Problem detail + solutions |
| GET | `/api/problems/mine` | Yes | My problems |
| POST/PUT/DELETE | `/api/problems` | Yes | Problem CRUD |
| POST | `/api/problems/{id}/solutions` | Verified+ | Add solution |
| PUT/DELETE | `/api/solutions/{id}` | Verified+ | Edit/delete solution |
| POST/DELETE | `/api/solutions/{id}/vote` | Yes | Vote on solution |

## Project Structure

```
AdvWebFinalProj/
├── backend/UniSolve.Api/     # ASP.NET Core API
│   ├── Controllers/
│   ├── Data/                 # DbContext, migrations, seeder
│   ├── DTOs/
│   ├── Models/Entities/
│   ├── Services/             # Business logic (OOP interfaces)
│   └── wwwroot/              # React production build
├── frontend/unisolve-client/ # React SPA
└── publish.ps1               # Build & publish script
```

## Database Schema

- **Users** — accounts with roles (Student, Verified, Moderator, Admin)
- **Subjects** — course categories (Math, CS, etc.)
- **Problems** — student-posted questions
- **Solutions** — verified user answers with score
- **Votes** — upvote (+1) / downvote (-1) per user per solution

## University Server Deployment

1. Install .NET 8 Runtime and SQL Server on the server
2. Copy the `publish/` folder
3. Update `appsettings.json` connection string for server SQL instance
4. Run `dotnet UniSolve.Api.dll` or host behind IIS

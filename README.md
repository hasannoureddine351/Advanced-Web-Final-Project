# UniSolve

Students post university problems; tutors and admins post solutions. Solutions are ranked by votes.

**Stack:** React + Vite, ASP.NET Core 8, SQL Server Express, JWT auth.

## Prerequisites

- .NET 8 SDK
- SQL Server Express (`SQLEXPRESS` instance)
- Node.js 18+

## Setup

```powershell
cd frontend/unisolve-client
npm install

cd ../../backend/UniSolve.Api
dotnet ef database update
```

Connection string: `backend/UniSolve.Api/appsettings.json` (default: `localhost\SQLEXPRESS`, Windows auth).

Start SQL Server (admin PowerShell): `Start-Service -Name 'MSSQL$SQLEXPRESS'`

## Development

**Backend** — `cd backend/UniSolve.Api` then `dotnet run` → http://localhost:5289

**Frontend** — `cd frontend/unisolve-client` then `npm run dev` → http://localhost:5173

## Production

```powershell
cd frontend/unisolve-client
npm run build

cd ../../backend/UniSolve.Api
dotnet publish -c Release -o ./publish
cd publish
dotnet UniSolve.Api.dll
```

Open http://localhost:5289 (API + React from one process).

## Default accounts

| Role  | Email              | Password  |
|-------|--------------------|-----------|
| Admin | admin@unisolve.com | Admin123! |
| Tutor | tutor@unisolve.com | Tutor123! |

New registrations get the Student role.

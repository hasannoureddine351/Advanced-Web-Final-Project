@echo off
echo Building React frontend...
cd /d "%~dp0frontend\unisolve-client"
call npm run build
if errorlevel 1 exit /b 1

echo Publishing ASP.NET backend...
cd /d "%~dp0backend\UniSolve.Api"
dotnet publish -c Release -o ./publish
if errorlevel 1 exit /b 1

echo.
echo ========================================
echo Publish complete!
echo Run: cd backend\UniSolve.Api\publish
echo      dotnet UniSolve.Api.dll
echo ========================================

# DAB Microservices - AI Agent Guide

## 📋 Quick Context
**Project**: DABSolution - Banking ATM network microservices in **French** (DAB = "Distributeur Automatique de Billets")
- See [README.md](README.md) for project overview
- Two-tier architecture: DAB.API (backend REST API, port 7174) + DAB.Web (frontend MVC with Identity, port 7102)
- Stack: ASP.NET Core 8, Entity Framework Core, SQL Server LocalDB, Swagger

## 🚀 Build & Run

### Prerequisites
```powershell
# Verify .NET 8 SDK installed
dotnet --version

# SQL Server LocalDB (required for database)
# - Windows: Install SQL Server Express with LocalDB
# - Or: Visual Studio Community includes LocalDB

# Dev certificates for HTTPS
dotnet dev-certs https --trust
```

### Build & Run
```bash
# Build solution
dotnet build

# Terminal 1: Start API first (required before Web)
cd DAB.API
dotnet run
# Runs on https://localhost:7174
# Swagger UI: https://localhost:7174/swagger/index.html

# Terminal 2 (different terminal): Start Web app
cd DAB.Web
dotnet run
# Runs on https://localhost:7102
# Auto-runs migrations & seeds Identity
```

**⚠️ Critical**: Always start API **before** Web. Web crashes if API is unavailable.

## 🏗️ Architecture & Databases

### Projects
| Project | Role | Port | Database | Key Components |
|---------|------|------|----------|-----------------|
| **DAB.API** | REST API | 7174 | `DABDb` | Controllers (Comptes, Transactions), EF Core models |
| **DAB.Web** | MVC Frontend | 7102 | `DABWebAuthDB` | Controllers, Views, Identity, HttpClient to API |

### Data Model (DABDb)
```
Banque (Bank)
  └─ 1:* Compte (Account, solde, type: Epargne|Courant)
      └─ 1:* Transaction (polymorphic: Retrait|Transfert)
         └─ 1:* Dab (ATM, localisation)
```

**Table-Per-Hierarchy polymorphism**: `Transaction` base class with `TransactionRetrait`, `TransactionTransfert` subtypes stored in single table via JSON discriminator.

### API → Web Communication
Web uses `HttpClient` with hardcoded API URL (`https://localhost:7174/`). See [DAB.Web/Services/TransactionService.cs](DAB.Web/Services/TransactionService.cs).
- If API port changes, update Web's `appsettings.json` and TransactionService

## 📂 Project Structure
```
DAB.API/
  ├── Controllers/        # Compte, Transaction, WeatherForecast endpoints
  ├── Models/             # Banque, Compte, Dab, Transaction, TransactionRetrait, TransactionTransfert, TypeCompte
  ├── Data/               # AppDbContext (EF Core DbContext)
  ├── Migrations/         # DB schema versions (auto-run on startup)
  └── Program.cs          # Startup config, middleware, seed admin user

DAB.Web/
  ├── Controllers/        # MVC controllers (Banques, Comptes, Dabs, Transactions, Home)
  ├── Views/              # Razor views (Shared, Banques, Comptes, Dabs, Transactions)
  ├── Services/           # TransactionService (HTTP wrapper for API)
  ├── Data/               # WebDbContext (Identity + app data)
  ├── Migrations/         # Identity schema migrations
  └── Program.cs          # ASP.NET Core + Identity configuration
```

## 🛠️ Development Conventions

### French Domain Language
All domain terms are in French:
- `Retrait` = Withdrawal, `Transfert` = Transfer, `Compte` = Account, `Banque` = Bank, `Dab` = ATM, `Solde` = Balance

### Naming & Structure
- **Models**: Domain objects in `/Models` folder (shared concepts between API and Web)
- **Controllers**: Standard .NET routing (area/controller/action)
- **Services**: Abstraction layer for business logic (`TransactionService` wraps API HTTP calls)
- **Database**: EF Core with automatic migrations on app startup

### Authorization
- **DAB.Web**: Global authorization filter (all routes require login except Identity pages)
- **DAB.API**: Currently no auth implemented (local dev only)

### Key Files to Know
| File | Purpose |
|------|---------|
| [DAB.API/Program.cs](DAB.API/Program.cs) | API startup, migrations, seed data |
| [DAB.Web/Program.cs](DAB.Web/Program.cs) | Web startup, Identity, HTTP clients |
| [DAB.Web/Services/TransactionService.cs](DAB.Web/Services/TransactionService.cs) | API communication layer |
| [DAB.API/Migrations/](DAB.API/Migrations/) | Schema evolution (inspect for data design) |

## ⚠️ Common Setup Issues

| Problem | Cause | Solution |
|---------|-------|----------|
| **App won't start: Connection to DB fails** | LocalDB not installed or `mssqllocaldb` unavailable | Install SQL Server Express with LocalDB or SQL Server Developer Edition |
| **"Unable to resolve service for type X"** | Dependency injection config missing in Program.cs | Check Program.cs for `services.AddScoped/Transient` registrations |
| **HTTPS certificate errors in dev** | Dev cert not trusted | Run `dotnet dev-certs https --trust` |
| **Web crashes after startup** | API not running | Start API **first** in separate terminal before Web |
| **Hard to login** | Auto-seeded admin not obvious | Check [DAB.API/Program.cs](DAB.API/Program.cs) for default credentials or look in seed logic |
| **API URL mismatch** | Port changed or appsettings not updated | Update `https://localhost:7174` references in [DAB.Web/Services/TransactionService.cs](DAB.Web/Services/TransactionService.cs) and appsettings |

## 🔍 When Implementing Features

### Adding a New API Endpoint
1. Create model in `DAB.API/Models/`
2. Add DbSet to [DAB.API/Data/AppDbContext.cs](DAB.API/Data/AppDbContext.cs)
3. Create migration: `dotnet ef migrations add [MigrationName]`
4. Add controller in `DAB.API/Controllers/`
5. Add service in [DAB.Web/Services/](DAB.Web/Services/) to wrap HTTP calls
6. Add MVC controller & views in DAB.Web if UI needed

### Adding a New Field to Account (Compte)
1. Update [DAB.API/Models/Compte.cs](DAB.API/Models/Compte.cs)
2. Create migration in DAB.API
3. Update Web's Compte model mirror and views
4. Update TransactionService if service layer needs changes

### Testing API Endpoints
- Use Swagger at `https://localhost:7174/swagger/index.html` (auto-generated from controllers)
- Or use Postman/REST Client with [DAB.API/DAB.API.http](DAB.API/DAB.API.http) file

## ✨ Core Banking Features

The application now includes production-grade functionality. See [DAB_FEATURES.md](DAB_FEATURES.md) for complete documentation on:

### Implemented Features
1. **Account Status & Security**: Account states (Active, Frozen, Suspended, Closed), PIN verification with lockout
2. **Bank Card Management**: Card creation, activation, blocking, daily limits per card
3. **Enhanced Transactions**: Categories, status tracking, fees, daily withdrawal limits
4. **Dispute/Fraud Management**: Submit and track disputes, automatic refunds on approval
5. **Account Limits**: Daily withdrawal limits per account and per card

### API Endpoints Summary
- **Account Management**: `/api/comptes/{id}/balance`, `/verify-pin`, `/change-pin`, `/freeze`, `/close`
- **Card Management**: `/api/cartesbancaires/compte/{id}`, `/block`, `/unblock`
- **Transactions**: `/api/transactions/compte/{id}/history`, `/statement`, `/statistics`, `/retrait`, `/transfert`
- **Disputes**: `/api/réclamations`, `/pending`

### Key Models
- `EtatCompte` - Account states
- `CarteBancaire` - ATM cards with daily limits
- `Réclamation` - Dispute/fraud claims
- `CatégorieTransaction` / `StatutTransaction` - Transaction enumerations

## 📚 Related Documentation
- [README.md](README.md) - Project overview and tech stack
- [DAB_FEATURES.md](DAB_FEATURES.md) - Comprehensive feature documentation
- Swagger UI (when API running) - Interactive API documentation
- EF Core docs - For migrations and DbContext patterns
- ASP.NET Core Identity - For auth customization in Web project

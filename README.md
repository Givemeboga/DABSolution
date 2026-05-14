# DAB Microservices Project

## 📌 Description
This project is a simple microservices architecture built with ASP.NET Core that simulates a Distributeur Automatique de Billets (DAB) / Automated Teller Machine (ATM) system with a full suite of core banking functions.

## 🧱 Architecture
- DAB.Web → Frontend (MVC + Identity Razor Pages)
- DAB.API → Backend (REST API for Accounts, Transactions, Cards, Disputes)

## 🚀 Features

The application incorporates a comprehensive set of banking capabilities:

### 1. **User Authentication & Authorization**
- Identity integration mapping to `User` and `Admin` roles.
- Admins can add accounts and manage global structures; standard users interact with their unified dashboards.

### 2. **Account Status & Security Management**
- **Account States**: Active, Frozen, Suspended, Closed
- **Security**: 4-digit PIN verification to access ATM, multi-failed attempt tracking, automatic account lockdown after 3 failures.
- **Daily Limits**: Configurable per-account daily withdrawal limits (with reset capabilities).

### 3. **Bank Card (Carte Bancaire) Management**
- Generate ATM cards with independent daily withdrawal limits.
- Block and unblock compromised cards instantly.
- Track daily usage for each specific card.

### 4. **Transaction Processing**
- **Categorization**: Tracks standard withdrawals, transfers, account deposits, system fees, and generated interest.
- **Validation**: Strict balance validation and daily limit checks before committing any action.
- Includes pagination and detailed transaction history (statements and statistics).

### 5. **Dispute/Fraud Reporting (Réclamations)**
- Users can flag specific transactions for fraudulent activity.
- Claims undergo a workflow state (Submitted → In Progress → Approved/Rejected).
- Approved claims automate the refund process directly to the user's account balance.

## 👥 Scrum Team Workload
Our team uses the Scrum methodology to effectively develop and distribute workload:

- **Youssef Ben Chaouacha**: Focuses on backend API architecture, database schema changes (Migrations), transaction limit validations, and core entity framework configurations (Comptes, CartesBancaires).
- **Mohammed Ben Naima**: Focuses on frontend UI/UX in the DAB.Web module, integrating Identity Razor Pages, dashboard visualizations, routing navigation structures, and user action views (Retrait, Transfert).
- **Abd El Aziz Brahim**: Cross-functional responsibilities targeting security features (PIN authentication endpoints, fraud/dispute workflows), integration testing between the API and Frontend via HttpClient, and managing general user workflows.

## ⚙️ Technologies
- ASP.NET Core (.NET 8)
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Bootstrap 5 & Bi Icons

## 🚀 Running the Application

```bash
# Terminal 1: Start API
cd DAB.API
dotnet run

# Terminal 2: Start Web (different terminal)
cd DAB.Web
dotnet run
```

- API: https://localhost:7174
- Web: https://localhost:7102
- Swagger: https://localhost:7174/swagger/index.html
# 🏧 DAB Microservices Project (Automated Teller Machine / Banking System)

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC_%2B_Razor_Pages-blue?style=for-the-badge&logo=asp.net)
![EF Core](https://img.shields.io/badge/Entity_Framework-Core-green?style=for-the-badge)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=for-the-badge&logo=bootstrap)

## 📌 Executive Summary
This project is an advanced, production-grade microservices architecture built with **ASP.NET Core (.NET 8)**. It acts as a comprehensive **Distributeur Automatique de Billets (DAB)** / Automated Teller Machine (ATM) and banking administration system. The application handles high-level core banking functionalities including account security tracking, real-time transaction validations, physical hardware emulation (Cards & ATMs), and anti-fraud automated workflows.

---

## 🧱 System Architecture

The application is cleanly decoupled into two main running processes to ensure scalability and separation of concerns:

### 1. Backend REST API (`DAB.API`)
A headless microservice functioning as the core banking engine. 
- **Database Access**: EF Core via SQL Server storing all relational transactional and user data.
- **Controllers**: Granular RESTful endpoints for `Banques`, `CartesBancaires`, `Comptes`, `Dabs`, `Réclamations`, and `Transactions`.
- **Validation layer**: Ensures data coherence (e.g., verifying sufficient balances, enforcing daily ATM withdrawal limits, checking PINs, preventing multi-attempt brute forces).

### 2. Frontend Web Portal (`DAB.Web`)
A hybrid ASP.NET Core MVC & Identity Razor Pages application functioning as the user/admin gateway.
- **User Authentication**: Secure cookie-based authentication via ASP.NET Core Identity.
- **Service Integration**: Communicates with `DAB.API` via `IHttpClientFactory`.
- **UI/UX**: Responsive dashboards styled with Bootstrap 5 and Bootstrap Icons, utilizing `_Layout` and `_Sidebar` partials for cohesive navigation.

---

## ✨ Exhaustive Feature Breakdown

### 1. **Identity, Authentication & Authorization**
- **Dual Role System**: Strict isolation between `Admin` and standard `User` accounts.
- **Restricted Onboarding**: Standard users cannot "register" themselves. Only administrators can add accounts through the secure portal, assigning default proprietary emails. 
- **State management**: Session tracking securely managed via Identity cookies and login differentials.

### 2. **Account Status & Secure Perimeter Management**
The `Compte` entity has been deeply expanded to operate like a real-world bank account.
- **Account States**: Actif, Gelé (Frozen), Suspendu (Suspended), Fermé (Closed). Transactions halt automatically if the account state is not `Actif`.
- **Security Checkpoints**: Users undergo a 4-digit PIN verification before utilizing ATM routes.
- **Anti-Brute Force**: System dynamically logs `TentativesÉchouéesConnexion` (Failed login attempts). At 3 failed attempts, the account is automatically locked to prevent unauthorized access.
- **Daily Withdrawal Thresholds**: Hard limits (`LimitRetraitQuotidien` & `TotalRetraitAujourd`) configured per account to cap daily withdrawals. Administrators can invoke endpoints to reset these counters.

### 3. **Physical Asset Management (Bank Cards)**
Integration of a physical dimension to digital accounts via the `CarteBancaire` module.
- **Card Minting**: Ability to generate ATM cards dynamically associated with accounts.
- **Lifecycle Tracking**: Cards feature isolated activation and expiration dates.
- **Granular Control**: Direct endpoints structure the ability to block/unblock compromised cards instantly.
- **Sub-limits**: Cards maintain their own independent daily withdrawal limits, existing as subsets of the main account limits.

### 4. **Transaction Processing Engine**
A fully rigorous transactional architecture ensuring 100% ACID compliance and historical immutability.
- **Transaction Types (`CatégorieTransaction`)**: Retrait (Withdrawal), Transfert (Transfer), Dépôt (Deposit), Frais (System Fees), Intérêts (Interests).
- **Transaction States (`StatutTransaction`)**: Terminé (Successful), Échoué (Failed), Annulé (Cancelled), Remboursé (Refunded).
- **Real-time Fee Calculation**: Out-of-network ATMS apply dynamic `Frais` (Fees) appended to standard transaction deductions.
- **Reporting & Data Fetching**: Complex GET endpoints yield complete account statements (paginated), historical history (day-filtered), and grouped metadata statistics.

### 5. **Fraud Control / Dispute Tracking (Réclamations)**
An automated case pipeline designed to mitigate fraudulent assertions and erroneous transfers.
- **Ticket Generation**: Users can dispute any processed transaction.
- **Workflow State Management**: Tickets shift organically through `Soumise` (Submitted) → `EnCours` (In Progress) → `Approuvée` (Approved) or `Rejetée` (Rejected).
- **Automated Reversals**: A confirmed (`Approuvée`) ticket programmatically triggers a system refund action, updating balances securely without manual DB edits.

### 6. **Infrastructure Nodes (Banks & ATMs)**
- **System Topologies**: Track and manage geographical entities like `Banques` (Bank Branches) and discrete physical `Dabs` (ATMs) which influence inter-bank transfer restrictions.

---

## 👥 Scrum Workload & Responsibilities
Our team utilized the Agile / **Scrum** methodology to develop, sprint, and distribute modular workloads seamlessly:

### 👨‍💻 Youssef Ben Chaouacha
- **Focus**: Backend Data Architecture & Transaction Logic.
- **Responsibilities**: 
  - Overhauling EF Core entity architectures (`Comptes`, `CartesBancaires`, `Transactions`).
  - Handling complex EF Core Migrations and propagating schema updates.
  - Developing the rigid limit checkers (daily thresholds) and account balance algorithms.
  - Structuring the underlying API Controller endpoints and ensuring ACID transactional integrity.

### 👨‍💻 Mohammed Ben Naima
- **Focus**: Frontend Application (DAB.Web), UI/UX & MVC Routing.
- **Responsibilities**: 
  - Integrating ASP.NET Core Identity through Razor Pages constraints.
  - Architecting the primary layout engine (Responsive Sidebars, Dashboards, partial views).
  - Building the end-user transactional views (`Retrait`, `Transfert`).
  - Securing the portal layout, altering public registrations strictly to Admin-based "Add Account" workflows.

### 👨‍💻 Abd El Aziz Brahim
- **Focus**: Cross-Service Connectivity, Network Security & Edge Workflows.
- **Responsibilities**: 
  - Integrating the `HttpClientFactory` calls syncing `DAB.Web` securely with `DAB.API`.
  - Architecting the physical logic security (PIN authentication routes, brute-force locking protocols).
  - Designing and hooking the Dispute / Fraud (`Réclamations`) workflow from the user's dashboard straight through to the administrative API back-end for automated refunds.

---

## 💾 Database Schema (Core Tables)
- **`AspNetUsers` / `AspNetUserRoles`**: Standard identity tracking.
- **`Comptes`**: Core banking ledger. Stores current balances, user IDs, security states, and ATM PIN hashes.
- **`Transactions`**: Immutable event-log tables defining inflows and outflows.
- **`CarteBancaires`**: Associated physical cards.
- **`Réclamations`**: State-based ticket tables.
- **`Banques` & `Dabs`**: Physical infrastructure tracking.

---

## ⚙️ Tech Stack & Dependencies
- **Runtime**: .NET 8 (C# 12)
- **Frameworks**: ASP.NET Core Web API, ASP.NET Core MVC, ASP.NET Core Identity (Razor Pages).
- **ORM**: Entity Framework Core, Entity Framework Core Tools.
- **Database**: Microsoft SQL Server (via LocalDB/SQLEXPRESS).
- **Frontend Tools**: Bootstrap 5, Bootstrap Icons, JavaScript, HTML5/CSS3.
- **Validation**: Server-side DataAnnotations and Client-side jQuery validation.

---

## 🚀 Getting Started & Installation

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022/2026 (or VS Code / Rider)
- SQL Server Express / LocalDB instance
- Git

### 1. Database Setup
The architecture utilizes EF Core Code-First migrations.
On first run (or if manually invoked), you must structure the initial database schema:
```bash
cd DAB.API
dotnet ef database update
```
*(Note: The API is configured to apply pending migrations on startup if instructed).*

### 2. Running Both Microservices
This app requires both the Frontend and Backend running harmoniously. Run them in two separate console instances:

```bash
# Terminal 1: Spin up the API microservice
cd DAB.API
dotnet run
```
**API Default Environment**: `https://localhost:7174`
You can inspect the entire OpenAPI spec via **Swagger**: `https://localhost:7174/swagger/index.html`

```bash
# Terminal 2: Spin up the UI Web client
cd DAB.Web
dotnet run
```
**Frontend Portal**: `https://localhost:7102`

---

## 🔄 Future Roadmap & Expansion Pipeline
To continuously mirror enterprise-ready systems, upcoming sprints will target:
1. **JWT Auth Implementation**: Upgrading internal network communication APIs to utilize secure JWT bearer tokens over standard HttpClients.
2. **Two-Factor Authentication (MFA)**: Binding robust MFA to identity logins using SMS/App based TOTP generation.
3. **Automated Interest Generation Worker**: A standalone `IHostedService` scheduled background worker assigning chronological rate `%` interest deposits across active accounts.
4. **Export Modules**: Direct reporting layers exporting monthly Statements arrays to paginated PDF / CSV documents.
- 
# DAB Core Functionalities - Feature Documentation

## 🎯 Overview
This document details the core banking functionalities added to the DAB (Distributeur Automatique de Billets) microservices application to make it production-grade.

## ✨ Core Features Implemented

### 1. **Account Status & Security Management**
**Models**: `Compte` (enhanced with new fields)

**Features**:
- **Account States**: Active, Frozen, Suspended, Closed
- **Security Status**: Normal, Multiple Failed Attempts, Suspicious Activity
- **PIN Management**: 4-digit PIN with automatic account suspension after 3 failed attempts
- **Failed Login Tracking**: Automatic account suspension to prevent brute force attacks
- **Last Activity Tracking**: Records when account was last accessed

**API Endpoints**:
```
GET  /api/comptes/{id}/balance              - Check account balance and limits
POST /api/comptes/{id}/verify-pin           - Verify PIN (required for ATM access)
PUT  /api/comptes/{id}/change-pin           - Change account PIN
PUT  /api/comptes/{id}/freeze               - Freeze account (admin only)
PUT  /api/comptes/{id}/unfreeze             - Unfreeze account
PUT  /api/comptes/{id}/close                - Close account (if balance = 0)
PUT  /api/comptes/{id}/reset-daily-withdrawal - Reset daily counter (admin)
```

### 2. **Bank Card Management**
**Model**: `CarteBancaire` (new)

**Features**:
- **Card Creation**: Generate new ATM cards for accounts
- **Card Activation/Deactivation**: Control card usage
- **Card Blocking**: Block compromised cards
- **Daily Card Limits**: Set per-card daily withdrawal limits
- **Daily Tracking**: Track total withdrawn per card per day

**API Endpoints**:
```
GET    /api/cartesbancaires/compte/{compteId}     - List all cards for account
GET    /api/cartesbancaires/{id}                   - Get card details
POST   /api/cartesbancaires                        - Create new card
PUT    /api/cartesbancaires/{id}                   - Update card info
PUT    /api/cartesbancaires/{id}/block             - Block card
PUT    /api/cartesbancaires/{id}/unblock           - Unblock card
DELETE /api/cartesbancaires/{id}                   - Delete card
```

### 3. **Transaction Enhancements**
**Models**: `Transaction` (enhanced), `CatégorieTransaction`, `StatutTransaction`

**Features**:
- **Transaction Categories**: Withdrawal, Transfer, Deposit, Fees, Interest
- **Transaction Status**: Successful, Failed, Cancelled, Refunded
- **Transaction Fees**: Track fees for each transaction
- **Transaction Reference**: Description/reference for tracking
- **Daily Limits**: Prevent exceeding daily withdrawal limits
- **Balance Validation**: Ensure sufficient funds before transaction
- **Account Status Check**: Verify account can perform transactions

**Enhanced Endpoints**:
```
GET  /api/transactions/{id}                          - Get transaction details
GET  /api/transactions/compte/{compteId}/history    - Get account history (optional days filter)
GET  /api/transactions/compte/{compteId}/statement  - Get paginated account statement
GET  /api/transactions/compte/{compteId}/statistics - Get transaction statistics
POST /api/transactions/retrait                      - Withdrawal (with validation)
POST /api/transactions/transfert                    - Transfer (with validation)
```

### 4. **Dispute/Fraud Management**
**Model**: `Réclamation` (new)

**Features**:
- **Dispute Submission**: Report fraudulent or erroneous transactions
- **Dispute Tracking**: Track dispute status through lifecycle
- **Dispute Resolution**: Admin can approve and process refunds
- **Automatic Refunds**: Approved disputes automatically refund the amount
- **Status Workflow**: Soumise → EnCours → Approuvée/Rejetée

**API Endpoints**:
```
GET    /api/réclamations/compte/{compteId}         - Get disputes for account
GET    /api/réclamations/{id}                       - Get dispute details
GET    /api/réclamations/pending                    - List all pending disputes (admin)
POST   /api/réclamations                            - Submit new dispute
PUT    /api/réclamations/{id}/status                - Update dispute status (admin)
```

### 5. **Account Limits & Validation**
**Features**:
- **Daily Withdrawal Limits**: Configurable per account
- **Daily Limit Tracking**: Reset daily counters
- **Real-time Validation**: Check limits before processing
- **Balance Checks**: Prevent overdrafts
- **Account State Checks**: Verify account can perform transactions
- **Limit Management**: Update limits via API

**Implementation**:
- Withdrawal requests check `TotalRetraitAujourd` vs `LimitRetraitQuotidien`
- Transfer requests check account status before processing
- Cards have independent daily limits

## 📊 Database Schema Changes

### New Tables
- `CarteBancaires` - ATM cards with daily limits
- `Réclamations` - Dispute/fraud claims

### Enhanced Tables
- `Comptes`: Added `Etat`, `StatutSecurité`, `TentativesÉchouéesConnexion`, `DateCréation`, `DernièreActivité`, `LimitRetraitQuotidien`, `TotalRetraitAujourd`, `CodePIN`
- `Transactions`: Added `Catégorie`, `Statut`, `Frais`, `Référence`

### Enumerations
- `EtatCompte`: Account states (Actif, Gelé, Suspendu, Fermé)
- `StatutSecurité`: Security status
- `CatégorieTransaction`: Transaction types
- `StatutTransaction`: Transaction outcomes
- `StatutRéclamation`: Dispute status

## 🔐 Security Features

1. **PIN Verification**: Required for ATM access with automatic lockout
2. **Failed Attempt Tracking**: Automatic account suspension after 3 failed PINs
3. **Card Blocking**: Block compromised cards immediately
4. **Transaction Validation**: Prevent insufficient fund transactions
5. **Status-Based Access**: Frozen/Suspended accounts cannot transact
6. **Fraud Reporting**: Dispute system for unauthorized transactions

## 🧪 Testing API Endpoints

### Access Swagger UI
```
https://localhost:7174/swagger/index.html
```

### Example PIN Verification
```json
POST /api/comptes/1/verify-pin
{
  "pin": "1234"
}
```

### Example Transaction
```json
POST /api/transactions/retrait
{
  "compteId": 1,
  "montant": 100,
  "autreAgence": false
}
```

### Example Dispute
```json
POST /api/réclamations
{
  "transactionId": 5,
  "compteId": 1,
  "motif": "Transaction non autorisée"
}
```

## 📝 Database Migration

Migration: `AddCoreDABFeatures`
- Run `dotnet ef database update` to apply all changes
- Migration is auto-run on application startup

## 🔄 Future Enhancements

Potential features for future iterations:
- SMS/Email notifications for large transactions
- Transaction categories customization
- Multi-factor authentication (MFA)
- Transaction scheduling
- Savings goals management
- Account statements export (PDF)
- Real-time transaction alerts
- Biometric authentication
- Rate limiting per IP/user
- Transaction anomaly detection

## 📚 Files Modified/Created

### API Project (DAB.API)
**New Files**:
- `Models/EtatCompte.cs` - Account state enumerations
- `Models/CarteBancaire.cs` - Card management model
- `Models/TransactionEnums.cs` - Transaction enumerations
- `Models/Réclamation.cs` - Dispute model
- `Controllers/CartesBancairesController.cs` - Card management API
- `Controllers/RéclamationsController.cs` - Dispute management API
- `Migrations/AddCoreDABFeatures.cs` - Database schema migration

**Modified Files**:
- `Models/Compte.cs` - Enhanced with security and limits
- `Models/Transaction.cs` - Enhanced with categories and status
- `Controllers/ComptesController.cs` - Added security endpoints
- `Controllers/TransactionsController.cs` - Enhanced with statements and validation
- `Data/AppDbContext.cs` - Added new DbSets and relationships

### Web Project (DAB.Web)
**New Files**:
- `Models/CarteBancaire.cs` - Card model for views
- `Models/Réclamation.cs` - Dispute model for views

**Modified Files**:
- `Models/Compte.cs` - Enhanced with new properties and helper methods
- `Models/Transaction.cs` - Enhanced with categories and status helpers

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

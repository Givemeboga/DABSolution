using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAB.API.Data;
using DAB.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace DAB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            return await _context.Transactions.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        /// <summary>
        /// Get transaction history for a specific account
        /// </summary>
        [HttpGet("compte/{compteId}/history")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetAccountHistory(int compteId, [FromQuery] int? days = null)
        {
            var compte = await _context.Comptes.FindAsync(compteId);
            if (compte == null)
                return NotFound("Account not found");

            var query = _context.Transactions.Where(t => t.CompteId == compteId);

            if (days.HasValue)
            {
                var startDate = DateTime.UtcNow.AddDays(-days.Value);
                query = query.Where(t => t.Date >= startDate);
            }

            var transactions = await query.OrderByDescending(t => t.Date).ToListAsync();
            return Ok(transactions);
        }

        /// <summary>
        /// Get account statement with pagination
        /// </summary>
        [HttpGet("compte/{compteId}/statement")]
        public async Task<ActionResult<AccountStatement>> GetAccountStatement(
            int compteId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var compte = await _context.Comptes.FindAsync(compteId);
            if (compte == null)
                return NotFound("Account not found");

            var query = _context.Transactions.Where(t => t.CompteId == compteId);

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            var totalCount = await query.CountAsync();
            var transactions = await query
                .OrderByDescending(t => t.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var statement = new AccountStatement
            {
                CompteId = compteId,
                NumeroCompte = compte.NumeroCompte,
                Proprietaire = compte.Proprietaire,
                SoldeActuel = compte.Solde,
                Transactions = transactions,
                TotalTransactions = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Ok(statement);
        }

        /// <summary>
        /// Get transaction statistics for an account
        /// </summary>
        [HttpGet("compte/{compteId}/statistics")]
        public async Task<ActionResult<TransactionStatistics>> GetTransactionStatistics(int compteId, [FromQuery] int days = 30)
        {
            var compte = await _context.Comptes.FindAsync(compteId);
            if (compte == null)
                return NotFound("Account not found");

            var startDate = DateTime.UtcNow.AddDays(-days);
            var transactions = await _context.Transactions
                .Where(t => t.CompteId == compteId && t.Date >= startDate && t.Statut == StatutTransaction.Réussie)
                .ToListAsync();

            var stats = new TransactionStatistics
            {
                CompteId = compteId,
                TotalTransactions = transactions.Count,
                TotalAmount = transactions.Sum(t => t.Montant),
                AverageTransaction = transactions.Any() ? transactions.Average(t => t.Montant) : 0,
                WithdrawalsCount = transactions.Count(t => t is TransactionRetrait),
                TransfersCount = transactions.Count(t => t is TransactionTransfert),
                TotalFees = transactions.Sum(t => t.Frais),
                Period = $"Last {days} days"
            };

            return Ok(stats);
        }

        [HttpPost("retrait")]
        public async Task<ActionResult<TransactionRetrait>> PostRetrait(TransactionRetrait retrait)
        {
            var compte = await _context.Comptes.FindAsync(retrait.CompteId);
            if (compte == null) return NotFound("Compte not found.");

            // Check account status
            if (compte.Etat != EtatCompte.Actif)
                return BadRequest($"Account is {compte.Etat} and cannot process withdrawals");

            // Check balance
            if (compte.Solde < retrait.Montant)
            {
                retrait.Statut = StatutTransaction.Échouée;
                retrait.Date = DateTime.UtcNow;
                _context.Transactions.Add(retrait);
                await _context.SaveChangesAsync();
                return BadRequest("Solde is insufficient for this withdrawal.");
            }

            // Check daily withdrawal limit
            var today = DateTime.UtcNow.Date;
            var withdrawalToday = await _context.Transactions
                .OfType<TransactionRetrait>()
                .Where(t => t.CompteId == retrait.CompteId && 
                           t.Date.Date == today && 
                           t.Statut == StatutTransaction.Réussie)
                .SumAsync(t => t.Montant);

            if (compte.TotalRetraitAujourd + retrait.Montant > compte.LimitRetraitQuotidien)
            {
                retrait.Statut = StatutTransaction.Échouée;
                retrait.Date = DateTime.UtcNow;
                _context.Transactions.Add(retrait);
                await _context.SaveChangesAsync();
                return BadRequest($"Daily withdrawal limit exceeded. Remaining: {compte.LimitRetraitQuotidien - compte.TotalRetraitAujourd}");
            }

            // Process withdrawal
            compte.Solde -= retrait.Montant;
            compte.TotalRetraitAujourd += retrait.Montant;
            compte.DernièreActivité = DateTime.UtcNow;
            
            retrait.Date = DateTime.UtcNow;
            retrait.Statut = StatutTransaction.Réussie;
            retrait.Catégorie = CatégorieTransaction.Retrait;

            _context.Transactions.Add(retrait);
            _context.Comptes.Update(compte);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransaction), new { id = retrait.Id }, retrait);
        }

        [HttpPost("transfert")]
        public async Task<ActionResult<TransactionTransfert>> PostTransfert(TransactionTransfert transfert)
        {
            var compteOrigine = await _context.Comptes.FindAsync(transfert.CompteId);
            if (compteOrigine == null) return NotFound("Origin account not found.");

            // Check account status
            if (compteOrigine.Etat != EtatCompte.Actif)
                return BadRequest($"Origin account is {compteOrigine.Etat} and cannot process transfers");

            var compteDestination = await _context.Comptes.FirstOrDefaultAsync(c => c.NumeroCompte == transfert.NumeroCompteDestination);
            if (compteDestination == null) return NotFound("Destination account not found.");

            // Check destination account status
            if (compteDestination.Etat != EtatCompte.Actif)
                return BadRequest("Destination account cannot receive transfers");

            // Check balance
            if (compteOrigine.Solde < transfert.Montant)
            {
                transfert.Statut = StatutTransaction.Échouée;
                transfert.Date = DateTime.UtcNow;
                _context.Transactions.Add(transfert);
                await _context.SaveChangesAsync();
                return BadRequest("Origin account has insufficient funds");
            }

            // Process transfer
            compteOrigine.Solde -= transfert.Montant;
            compteDestination.Solde += transfert.Montant;
            compteOrigine.DernièreActivité = DateTime.UtcNow;

            transfert.Date = DateTime.UtcNow;
            transfert.Statut = StatutTransaction.Réussie;
            transfert.Catégorie = CatégorieTransaction.Transfert;

            _context.Transactions.Add(transfert);
            _context.Comptes.Update(compteOrigine);
            _context.Comptes.Update(compteDestination);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransaction), new { id = transfert.Id }, transfert);
        }
    }

    /// <summary>
    /// Account statement response DTO
    /// </summary>
    public class AccountStatement
    {
        public int CompteId { get; set; }
        public string NumeroCompte { get; set; }
        public string Proprietaire { get; set; }
        public double SoldeActuel { get; set; }
        public List<Transaction> Transactions { get; set; }
        public int TotalTransactions { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    /// <summary>
    /// Transaction statistics response DTO
    /// </summary>
    public class TransactionStatistics
    {
        public int CompteId { get; set; }
        public int TotalTransactions { get; set; }
        public double TotalAmount { get; set; }
        public double AverageTransaction { get; set; }
        public int WithdrawalsCount { get; set; }
        public int TransfersCount { get; set; }
        public double TotalFees { get; set; }
        public string Period { get; set; }
    }
}

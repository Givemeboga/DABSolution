using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAB.API.Data;
using DAB.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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

        [HttpPost("retrait")]
        public async Task<ActionResult<TransactionRetrait>> PostRetrait(TransactionRetrait retrait)
        {
            var compte = await _context.Comptes.FindAsync(retrait.CompteId);
            if (compte == null) return NotFound("Compte not found.");

            if (compte.Solde < retrait.Montant)
            {
                return BadRequest("Solde is insufficient for this withdrawal.");
            }

            compte.Solde -= retrait.Montant;
            retrait.Date = DateTime.UtcNow;

            _context.Transactions.Add(retrait);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransaction), new { id = retrait.Id }, retrait);
        }

        [HttpPost("transfert")]
        public async Task<ActionResult<TransactionTransfert>> PostTransfert(TransactionTransfert transfert)
        {
            var compteOrigine = await _context.Comptes.FindAsync(transfert.CompteId);
            if (compteOrigine == null) return NotFound("Origin account not found.");

            if (compteOrigine.Solde < transfert.Montant)
            {
                return BadRequest("Solde is insufficient for this transfer.");
            }

            var compteDestination = await _context.Comptes.FirstOrDefaultAsync(c => c.NumeroCompte == transfert.NumeroCompteDestination);
            if (compteDestination == null) return NotFound("Destination account not found.");

            compteOrigine.Solde -= transfert.Montant;
            compteDestination.Solde += transfert.Montant;
            transfert.Date = DateTime.UtcNow;

            _context.Transactions.Add(transfert);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransaction), new { id = transfert.Id }, transfert);
        }
    }
}

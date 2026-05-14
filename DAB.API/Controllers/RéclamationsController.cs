using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAB.API.Data;
using DAB.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RéclamationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RéclamationsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all disputes/claims for a specific account
        /// </summary>
        [HttpGet("compte/{compteId}")]
        public async Task<ActionResult<IEnumerable<Réclamation>>> GetRéclamationsByCompte(int compteId)
        {
            var réclamations = await _context.Réclamations
                .Where(r => r.CompteId == compteId)
                .Include(r => r.Transaction)
                .ToListAsync();

            return Ok(réclamations);
        }

        /// <summary>
        /// Get a specific dispute claim by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Réclamation>> GetRéclamation(int id)
        {
            var réclamation = await _context.Réclamations
                .Include(r => r.Transaction)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (réclamation == null)
                return NotFound();

            return Ok(réclamation);
        }

        /// <summary>
        /// Submit a new dispute/fraud claim
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Réclamation>> CreateRéclamation([FromBody] Réclamation réclamation)
        {
            // Verify transaction and account exist
            var transaction = await _context.Transactions.FindAsync(réclamation.TransactionId);
            if (transaction == null)
                return BadRequest("Transaction not found");

            var compte = await _context.Comptes.FindAsync(réclamation.CompteId);
            if (compte == null)
                return BadRequest("Account not found");

            // Verify transaction belongs to the account
            if (transaction.CompteId != réclamation.CompteId)
                return BadRequest("Transaction does not belong to this account");

            réclamation.DateSoumission = DateTime.UtcNow;
            réclamation.Statut = StatutRéclamation.Soumise;

            _context.Réclamations.Add(réclamation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRéclamation), new { id = réclamation.Id }, réclamation);
        }

        /// <summary>
        /// Update dispute claim status (admin only)
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateRéclamationStatus(int id, [FromBody] UpdateRéclamationRequest request)
        {
            var réclamation = await _context.Réclamations.FindAsync(id);
            if (réclamation == null)
                return NotFound();

            réclamation.Statut = request.Statut;
            réclamation.RéponseAdmin = request.RéponseAdmin;
            réclamation.DateRésolution = DateTime.UtcNow;

            // If approved, process refund
            if (request.Statut == StatutRéclamation.Approuvée)
            {
                var transaction = await _context.Transactions.FindAsync(réclamation.TransactionId);
                var compte = await _context.Comptes.FindAsync(réclamation.CompteId);

                if (transaction != null && compte != null)
                {
                    // Refund the amount
                    compte.Solde += transaction.Montant;
                    _context.Comptes.Update(compte);
                }
            }

            _context.Réclamations.Update(réclamation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Get all pending dispute claims
        /// </summary>
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<Réclamation>>> GetPendingRéclamations()
        {
            var réclamations = await _context.Réclamations
                .Where(r => r.Statut == StatutRéclamation.Soumise)
                .Include(r => r.Transaction)
                .Include(r => r.Compte)
                .ToListAsync();

            return Ok(réclamations);
        }
    }

    public class UpdateRéclamationRequest
    {
        public StatutRéclamation Statut { get; set; }
        public string? RéponseAdmin { get; set; }
    }
}

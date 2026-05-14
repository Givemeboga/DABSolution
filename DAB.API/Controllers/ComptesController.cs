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
    public class ComptesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private const int MAX_PIN_ATTEMPTS = 3;

        public ComptesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Compte>>> GetComptes()
        {
            return await _context.Comptes
                                 .Include(c => c.Banque)
                                 .Include(c => c.Dab)
                                 .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Compte>> GetCompte(int id)
        {
            var compte = await _context.Comptes
                                       .Include(c => c.Banque)
                                       .Include(c => c.Dab)
                                       .Include(c => c.Transactions)
                                       .FirstOrDefaultAsync(c => c.Id == id);

            if (compte == null)
            {
                return NotFound();
            }

            return compte;
        }

        /// <summary>
        /// Get account balance
        /// </summary>
        [HttpGet("{id}/balance")]
        public async Task<ActionResult<BalanceResponse>> GetBalance(int id)
        {
            var compte = await _context.Comptes.FindAsync(id);
            if (compte == null)
                return NotFound("Account not found");

            var response = new BalanceResponse
            {
                NumeroCompte = compte.NumeroCompte,
                Solde = compte.Solde,
                Etat = compte.Etat.ToString(),
                DernièreActivité = compte.DernièreActivité,
                LimitRetraitQuotidien = compte.LimitRetraitQuotidien,
                RetraitDuJour = compte.TotalRetraitAujourd,
                RétraitDisponible = compte.LimitRetraitQuotidien - compte.TotalRetraitAujourd
            };

            return Ok(response);
        }

        /// <summary>
        /// Verify PIN for ATM access
        /// </summary>
        [HttpPost("{id}/verify-pin")]
        public async Task<ActionResult<PINVerificationResponse>> VerifyPIN(int id, [FromBody] PINVerificationRequest request)
        {
            var compte = await _context.Comptes.FindAsync(id);
            if (compte == null)
                return NotFound("Account not found");

            // Check if account is suspended
            if (compte.Etat == EtatCompte.Suspendu)
                return Forbid("Account is suspended due to security concerns");

            // Check PIN
            if (compte.CodePIN != request.PIN)
            {
                compte.TentativesÉchouéesConnexion++;

                // Lock account after max attempts
                if (compte.TentativesÉchouéesConnexion >= MAX_PIN_ATTEMPTS)
                {
                    compte.Etat = EtatCompte.Suspendu;
                    compte.StatutSecurité = StatutSecurité.TentativesÉchoueesMultiples;
                }

                _context.Comptes.Update(compte);
                await _context.SaveChangesAsync();

                return Unauthorized(new PINVerificationResponse
                {
                    Succès = false,
                    Message = $"Invalid PIN. Attempts remaining: {MAX_PIN_ATTEMPTS - compte.TentativesÉchouéesConnexion}",
                    TentativesRestantes = Math.Max(0, MAX_PIN_ATTEMPTS - compte.TentativesÉchouéesConnexion)
                });
            }

            // Reset attempts on successful verification
            compte.TentativesÉchouéesConnexion = 0;
            compte.DernièreActivité = DateTime.UtcNow;
            _context.Comptes.Update(compte);
            await _context.SaveChangesAsync();

            return Ok(new PINVerificationResponse
            {
                Succès = true,
                Message = "PIN verified successfully",
                CompteId = compte.Id,
                Solde = compte.Solde
            });
        }

        /// <summary>
        /// Change PIN
        /// </summary>
        [HttpPut("{id}/change-pin")]
        public async Task<IActionResult> ChangePIN(int id, [FromBody] ChangePINRequest request)
        {
            var compte = await _context.Comptes.FindAsync(id);
            if (compte == null)
                return NotFound("Account not found");

            // Verify current PIN first
            if (compte.CodePIN != request.PINActuel)
                return Unauthorized("Current PIN is incorrect");

            compte.CodePIN = request.NouveauPIN;
            compte.TentativesÉchouéesConnexion = 0;
            _context.Comptes.Update(compte);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Freeze account (admin action)
        /// </summary>
        [HttpPut("{id}/freeze")]
        public async Task<IActionResult> FreezeAccount(int id, [FromBody] FreezeAccountRequest request)
        {
            var compte = await _context.Comptes.FindAsync(id);
            if (compte == null)
                return NotFound("Account not found");

            compte.Etat = EtatCompte.Gelé;
            _context.Comptes.Update(compte);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Unfreeze account (admin action)
        /// </summary>
        [HttpPut("{id}/unfreeze")]
        public async Task<IActionResult> UnfreezeAccount(int id)
        {
            var compte = await _context.Comptes.FindAsync(id);
            if (compte == null)
                return NotFound("Account not found");

            compte.Etat = EtatCompte.Actif;
            compte.TentativesÉchouéesConnexion = 0;
            _context.Comptes.Update(compte);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Close account
        /// </summary>
        [HttpPut("{id}/close")]
        public async Task<IActionResult> CloseAccount(int id)
        {
            var compte = await _context.Comptes.FindAsync(id);
            if (compte == null)
                return NotFound("Account not found");

            // Check if account has balance
            if (compte.Solde > 0)
                return BadRequest("Cannot close account with remaining balance. Please withdraw funds first.");

            compte.Etat = EtatCompte.Fermé;
            _context.Comptes.Update(compte);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Update withdrawal limit
        /// </summary>
        [HttpPut("{id}/withdrawal-limit")]
        public async Task<IActionResult> UpdateWithdrawalLimit(int id, [FromBody] UpdateWithdrawalLimitRequest request)
        {
            var compte = await _context.Comptes.FindAsync(id);
            if (compte == null)
                return NotFound("Account not found");

            if (request.NouveauLimit < 0)
                return BadRequest("Limit cannot be negative");

            compte.LimitRetraitQuotidien = request.NouveauLimit;
            _context.Comptes.Update(compte);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Reset daily withdrawal counter (admin action)
        /// </summary>
        [HttpPut("{id}/reset-daily-withdrawal")]
        public async Task<IActionResult> ResetDailyWithdrawal(int id)
        {
            var compte = await _context.Comptes.FindAsync(id);
            if (compte == null)
                return NotFound("Account not found");

            compte.TotalRetraitAujourd = 0;
            _context.Comptes.Update(compte);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Compte>> PostCompte(Compte compte)
        {
            compte.DateCréation = DateTime.UtcNow;
            compte.Etat = EtatCompte.Actif;
            compte.StatutSecurité = StatutSecurité.Normal;

            _context.Comptes.Add(compte);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompte), new { id = compte.Id }, compte);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompte(int id, Compte compte)
        {
            if (id != compte.Id)
            {
                return BadRequest();
            }

            _context.Entry(compte).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Comptes.AnyAsync(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompte(int id)
        {
            var compte = await _context.Comptes.FindAsync(id);
            if (compte == null)
            {
                return NotFound();
            }

            _context.Comptes.Remove(compte);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // DTOs for API requests/responses
    public class BalanceResponse
    {
        public string NumeroCompte { get; set; }
        public double Solde { get; set; }
        public string Etat { get; set; }
        public DateTime? DernièreActivité { get; set; }
        public double LimitRetraitQuotidien { get; set; }
        public double RetraitDuJour { get; set; }
        public double RétraitDisponible { get; set; }
    }

    public class PINVerificationRequest
    {
        public string PIN { get; set; }
    }

    public class PINVerificationResponse
    {
        public bool Succès { get; set; }
        public string Message { get; set; }
        public int? CompteId { get; set; }
        public double? Solde { get; set; }
        public int? TentativesRestantes { get; set; }
    }

    public class ChangePINRequest
    {
        public string PINActuel { get; set; }
        public string NouveauPIN { get; set; }
    }

    public class FreezeAccountRequest
    {
        public string Raison { get; set; }
    }

    public class UpdateWithdrawalLimitRequest
    {
        public double NouveauLimit { get; set; }
    }
}
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
    public class CartesBancairesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartesBancairesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all cards for a specific account
        /// </summary>
        [HttpGet("compte/{compteId}")]
        public async Task<ActionResult<IEnumerable<CarteBancaire>>> GetCartesByCompte(int compteId)
        {
            var cartes = await _context.CartesBancaires
                .Where(c => c.CompteId == compteId)
                .ToListAsync();

            if (!cartes.Any())
                return NotFound($"No cards found for account {compteId}");

            return Ok(cartes);
        }

        /// <summary>
        /// Get a specific card by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CarteBancaire>> GetCarte(int id)
        {
            var carte = await _context.CartesBancaires.FindAsync(id);

            if (carte == null)
                return NotFound();

            return Ok(carte);
        }

        /// <summary>
        /// Create a new card for an account
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CarteBancaire>> CreateCarte([FromBody] CarteBancaire carte)
        {
            // Verify account exists
            var compte = await _context.Comptes.FindAsync(carte.CompteId);
            if (compte == null)
                return BadRequest("Account not found");

            _context.CartesBancaires.Add(carte);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCarte), new { id = carte.Id }, carte);
        }

        /// <summary>
        /// Update card information
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCarte(int id, [FromBody] CarteBancaire carte)
        {
            if (id != carte.Id)
                return BadRequest("ID mismatch");

            var existingCarte = await _context.CartesBancaires.FindAsync(id);
            if (existingCarte == null)
                return NotFound();

            existingCarte.Activée = carte.Activée;
            existingCarte.Bloquée = carte.Bloquée;
            existingCarte.LimitRetraitQuotidien = carte.LimitRetraitQuotidien;

            _context.CartesBancaires.Update(existingCarte);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Block a card (e.g., for suspected fraud)
        /// </summary>
        [HttpPut("{id}/block")]
        public async Task<IActionResult> BlockCarte(int id)
        {
            var carte = await _context.CartesBancaires.FindAsync(id);
            if (carte == null)
                return NotFound();

            carte.Bloquée = true;
            _context.CartesBancaires.Update(carte);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Unblock a card
        /// </summary>
        [HttpPut("{id}/unblock")]
        public async Task<IActionResult> UnblockCarte(int id)
        {
            var carte = await _context.CartesBancaires.FindAsync(id);
            if (carte == null)
                return NotFound();

            carte.Bloquée = false;
            _context.CartesBancaires.Update(carte);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Delete a card
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarte(int id)
        {
            var carte = await _context.CartesBancaires.FindAsync(id);
            if (carte == null)
                return NotFound();

            _context.CartesBancaires.Remove(carte);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

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
    public class ComptesController : ControllerBase
    {
        private readonly AppDbContext _context;

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

        [HttpPost]
        public async Task<ActionResult<Compte>> PostCompte(Compte compte)
        {
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
}
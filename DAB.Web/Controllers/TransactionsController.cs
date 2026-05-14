using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DAB.Web.Services;
using DAB.Web.Models;

namespace DAB.Web.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly TransactionService _service;
        private readonly HttpClient _http;

        public TransactionsController(TransactionService service, IHttpClientFactory factory)
        {
            _service = service;
            _http = factory.CreateClient("API");
        }

        [Authorize(Roles = "Admin,User")] // Allowing both for testing purposes, but usually Admin sees all, User sees theirs
        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAll();
            var comptes = await _http.GetFromJsonAsync<List<Compte>>("api/comptes") ?? new List<Compte>();

            var compteIds = comptes.Select(c => c.Id).ToHashSet();
            data = data.Where(t => compteIds.Contains(t.CompteId)).ToList();

            ViewBag.Comptes = comptes;
            return View(data);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Retrait()
        {
            var comptes = await GetAccessibleComptesAsync();
            var sourceCompte = comptes.FirstOrDefault();
            ViewBag.SourceCompte = sourceCompte;
            return View(new Transaction { CompteId = sourceCompte?.Id ?? 0 });
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Retrait(Transaction t)
        {
            await _service.Retrait(t);
            TempData["Success"] = "Withdrawal completed successfully.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Transfert()
        {
            var sourceComptes = await GetAccessibleComptesAsync();
            var sourceCompte = sourceComptes.FirstOrDefault();
            ViewBag.SourceCompte = sourceCompte;
            var destinationComptes = await _http.GetFromJsonAsync<List<Compte>>("api/comptes") ?? new List<Compte>();
            ViewBag.DestinationComptes = destinationComptes;
            return View(new Transaction { CompteId = sourceCompte?.Id ?? 0 });
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Transfert(Transaction t)
        {
            await _service.Transfert(t);
            TempData["Success"] = "Transfer completed successfully.";
            return RedirectToAction("Index");
        }

        private async Task<List<Compte>> GetAccessibleComptesAsync()
        {
            var comptes = await _http.GetFromJsonAsync<List<Compte>>("api/comptes") ?? new List<Compte>();

            if (User.IsInRole("Admin"))
            {
                return comptes;
            }

            var userName = User.Identity?.Name ?? string.Empty;
            return comptes
                .Where(c => string.Equals(c.Proprietaire, userName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}

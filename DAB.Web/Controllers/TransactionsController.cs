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
            return View(data);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Retrait()
        {
            var comptes = await _http.GetFromJsonAsync<List<Compte>>("api/comptes");
            ViewBag.Comptes = comptes ?? new List<Compte>();
            return View();
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
            var comptes = await _http.GetFromJsonAsync<List<Compte>>("api/comptes");
            ViewBag.Comptes = comptes ?? new List<Compte>();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Transfert(Transaction t)
        {
            await _service.Transfert(t);
            TempData["Success"] = "Transfer completed successfully.";
            return RedirectToAction("Index");
        }
    }
}

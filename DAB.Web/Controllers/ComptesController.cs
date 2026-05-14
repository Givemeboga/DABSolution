using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DAB.Web.Models;

namespace DAB.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ComptesController : Controller
    {
        private readonly HttpClient _http;

        public ComptesController(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("API");
        }

        public async Task<IActionResult> Index()
        {
            var comptes = await _http.GetFromJsonAsync<List<Compte>>("api/comptes");
            return View(comptes ?? new List<Compte>());
        }
        
        public IActionResult Create()
        {
            return View(new Compte());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Compte compte)
        {
            if (ModelState.IsValid)
            {
                await _http.PostAsJsonAsync("api/comptes", compte);
                TempData["Success"] = "Account successfully registered.";
                return RedirectToAction(nameof(Index));
            }
            return View(compte);
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _http.DeleteAsync($"api/comptes/{id}");
            TempData["Success"] = "Account successfully removed.";
            return RedirectToAction(nameof(Index));
        }
    }
}
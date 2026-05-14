using DAB.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace DAB.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _http;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory factory)
        {
            _logger = logger;
            _http = factory.CreateClient("API");
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel();

            try
            {
                var comptes = await _http.GetFromJsonAsync<List<Compte>>("api/comptes") ?? new List<Compte>();
                if (!User.IsInRole("Admin"))
                {
                    var userName = User.Identity?.Name ?? string.Empty;
                    comptes = comptes
                        .Where(c => string.Equals(c.Proprietaire, userName, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (comptes.Any())
                {
                    vm.TotalAccounts = comptes.Count;
                    vm.TotalBalance = comptes.Sum(c => c.Solde);
                    
                    // Simple distinct DAB counter based on the accounts attached to DABs
                    vm.ConnectedAtms = comptes.Where(c => c.DabId.HasValue).Select(c => c.DabId).Distinct().Count();
                }

                var transactions = await _http.GetFromJsonAsync<List<Transaction>>("api/transactions") ?? new List<Transaction>();
                if (!User.IsInRole("Admin"))
                {
                    var compteIds = comptes.Select(c => c.Id).ToHashSet();
                    transactions = transactions.Where(t => compteIds.Contains(t.CompteId)).ToList();
                }

                if (transactions.Any())
                {
                    vm.TransactionsCount = transactions.Count;
                    vm.RecentTransactions = transactions.OrderByDescending(t => t.Date).Take(5).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch dashboard data from API");
            }

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using DAB.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
                var comptes = await _http.GetFromJsonAsync<List<Compte>>("api/comptes");
                if (comptes != null)
                {
                    vm.TotalAccounts = comptes.Count;
                    vm.TotalBalance = comptes.Sum(c => c.Solde);
                    
                    // Simple distinct DAB counter based on the accounts attached to DABs
                    vm.ConnectedAtms = comptes.Where(c => c.DabId.HasValue).Select(c => c.DabId).Distinct().Count();
                }

                var transactions = await _http.GetFromJsonAsync<List<Transaction>>("api/transactions");
                if (transactions != null)
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

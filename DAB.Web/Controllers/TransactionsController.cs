using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DAB.Web.Services;

namespace DAB.Web.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly TransactionService _service;

        public TransactionsController(TransactionService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAll();
            return View(data);
        }
    }
}

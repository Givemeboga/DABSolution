using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAB.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DabsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAB.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BanquesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
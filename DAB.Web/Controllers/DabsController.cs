using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DAB.Web.Models;

namespace DAB.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DabsController : Controller
    {
        private static readonly List<Dab> Dabs = new()
        {
            new Dab
            {
                Id = 1,
                DABId = "DAB-P1",
                Localisation = "Gare de Lyon",
                IsActive = true,
                AccountsCount = 2
            },
            new Dab
            {
                Id = 2,
                DABId = "DAB-M1",
                Localisation = "Marseille Centre",
                IsActive = false,
                AccountsCount = 0
            }
        };

        public IActionResult Index()
        {
            return View(Dabs);
        }

        public IActionResult Map()
        {
            return View(Dabs);
        }

        public IActionResult Logs(int id)
        {
            var dab = Dabs.FirstOrDefault(item => item.Id == id);
            if (dab == null)
            {
                return NotFound();
            }

            return View(dab);
        }

        [HttpPost]
        public IActionResult Activate(int id)
        {
            var dab = Dabs.FirstOrDefault(item => item.Id == id);
            if (dab == null)
            {
                return NotFound();
            }

            dab.IsActive = true;
            TempData["Success"] = $"{dab.DABId} activated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Disable(int id)
        {
            var dab = Dabs.FirstOrDefault(item => item.Id == id);
            if (dab == null)
            {
                return NotFound();
            }

            dab.IsActive = false;
            TempData["Success"] = $"{dab.DABId} disabled.";
            return RedirectToAction(nameof(Index));
        }
    }
}
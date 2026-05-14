using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAB.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BanquesController : Controller
    {
        private static readonly List<Models.Banque> Banques = new()
        {
            new Models.Banque
            {
                Id = 1,
                Nom = "Banque de Paris",
                Code = 1001,
                Email = "contact@banqueparis.fr",
                Rue = "10 Avenue Champs",
                Ville = "Paris"
            },
            new Models.Banque
            {
                Id = 2,
                Nom = "Banque de Lyon",
                Code = 1002,
                Email = "contact@banquelyon.fr",
                Rue = "25 Rue de la République",
                Ville = "Lyon"
            }
        };

        public IActionResult Index()
        {
            return View(Banques);
        }

        public IActionResult Details(int id)
        {
            var banque = Banques.FirstOrDefault(b => b.Id == id);
            if (banque == null)
            {
                return NotFound();
            }

            return View(banque);
        }
    }
}
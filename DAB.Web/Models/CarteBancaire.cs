using System;

namespace DAB.Web.Models
{
    public class CarteBancaire
    {
        public int Id { get; set; }
        public string NuméroCartе { get; set; } = string.Empty;
        public DateTime DateExpiration { get; set; }
        public string CVV { get; set; } = string.Empty;
        public DateTime DateCréation { get; set; }
        public bool Activée { get; set; }
        public bool Bloquée { get; set; }
        public decimal LimitRetraitQuotidien { get; set; }
        public decimal TotalRetraitAujourd { get; set; }
        public int CompteId { get; set; }

        public string StatutLabel => Bloquée ? "Bloquée" : (Activée ? "Activée" : "Désactivée");
        public decimal RetraitDisponible => LimitRetraitQuotidien - TotalRetraitAujourd;
    }
}

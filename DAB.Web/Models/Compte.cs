using System;

namespace DAB.Web.Models
{
    public class Compte
    {
        public int Id { get; set; }
        public string NumeroCompte { get; set; } = string.Empty;
        public string Proprietaire { get; set; } = string.Empty;
        public decimal Solde { get; set; }
        public int Type { get; set; }
        
        /// <summary>
        /// Account state (0=Active, 1=Frozen, 2=Suspended, 3=Closed)
        /// </summary>
        public int Etat { get; set; } = 0;
        
        public DateTime DateCréation { get; set; }
        public DateTime? DernièreActivité { get; set; }
        public decimal LimitRetraitQuotidien { get; set; }
        public decimal TotalRetraitAujourd { get; set; }
        
        public int BanqueId { get; set; }
        public Banque? Banque { get; set; }
        public int? DabId { get; set; }
        
        /// <summary>
        /// Returns user-friendly account status
        /// </summary>
        public string EtatLabel => Etat switch
        {
            0 => "Actif",
            1 => "Gelé",
            2 => "Suspendu",
            3 => "Fermé",
            _ => "Inconnu"
        };

        /// <summary>
        /// Returns remaining daily withdrawal amount
        /// </summary>
        public decimal RetraitDisponible => LimitRetraitQuotidien - TotalRetraitAujourd;

        /// <summary>
        /// Checks if account can perform transactions
        /// </summary>
        public bool CanTransact => Etat == 0;
    }
}

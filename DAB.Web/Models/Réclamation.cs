using System;

namespace DAB.Web.Models
{
    public class Réclamation
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
        public int CompteId { get; set; }
        public string Motif { get; set; } = string.Empty;
        public int Statut { get; set; } // 0=Soumise, 1=EnCours, 2=Approuvée, 3=Rejetée
        public DateTime DateSoumission { get; set; }
        public DateTime? DateRésolution { get; set; }
        public string? RéponseAdmin { get; set; }

        public string StatutLabel => Statut switch
        {
            0 => "Soumise",
            1 => "En cours",
            2 => "Approuvée",
            3 => "Rejetée",
            _ => "Inconnu"
        };
    }
}

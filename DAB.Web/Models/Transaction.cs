using System;

namespace DAB.Web.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int CompteId { get; set; }
        public DateTime Date { get; set; }
        public decimal Montant { get; set; }
        public string? TransactionType { get; set; }
        
        /// <summary>
        /// Transaction category (0=Retrait, 1=Transfert, 2=Dépôt, 3=Frais, 4=Intérêt)
        /// </summary>
        public int Catégorie { get; set; } = 0;
        
        /// <summary>
        /// Transaction status (0=Réussie, 1=Échouée, 2=Annulée, 3=Remboursée)
        /// </summary>
        public int Statut { get; set; } = 0;
        
        /// <summary>
        /// Transaction fees
        /// </summary>
        public decimal Frais { get; set; } = 0;
        
        /// <summary>
        /// Reference or description
        /// </summary>
        public string? Référence { get; set; }
        
        // Polymorphic fields
        public bool AutreAgence { get; set; }
        public string? NumeroCompteDestination { get; set; }

        public string CatégorieLabel => Catégorie switch
        {
            0 => "Retrait",
            1 => "Transfert",
            2 => "Dépôt",
            3 => "Frais",
            4 => "Intérêt",
            _ => "Autre"
        };

        public string StatutLabel => Statut switch
        {
            0 => "Réussie",
            1 => "Échouée",
            2 => "Annulée",
            3 => "Remboursée",
            _ => "Inconnu"
        };

        public string TypeLabel => TransactionType switch
        {
            "retrait" => "Retrait",
            "transfert" => "Transfert",
            "base" => "Transaction",
            _ => "Autre"
        };
    }
}

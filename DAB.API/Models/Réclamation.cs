using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DAB.API.Models
{
    /// <summary>
    /// Enum for dispute status
    /// </summary>
    public enum StatutRéclamation
    {
        Soumise = 0,           // Submitted
        EnCours = 1,           // Under investigation
        Approuvée = 2,         // Approved and refunded
        Rejetée = 3            // Rejected
    }

    /// <summary>
    /// Dispute/Fraud claim for transaction disputes
    /// </summary>
    public class Réclamation
    {
        public int Id { get; set; }

        [Required]
        public int TransactionId { get; set; }

        [JsonIgnore]
        public Transaction? Transaction { get; set; }

        [Required]
        public int CompteId { get; set; }

        [JsonIgnore]
        public Compte? Compte { get; set; }

        [Required]
        [StringLength(500)]
        public string Motif { get; set; }

        [Required]
        public StatutRéclamation Statut { get; set; } = StatutRéclamation.Soumise;

        [Required]
        public DateTime DateSoumission { get; set; } = DateTime.UtcNow;

        public DateTime? DateRésolution { get; set; }

        [StringLength(1000)]
        public string? RéponseAdmin { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DAB.API.Models
{
    /// <summary>
    /// Represents a bank account (Compte Bancaire) in the DAB system
    /// </summary>
    public class Compte
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string NumeroCompte { get; set; }

        [Required]
        [StringLength(100)]
        public string Proprietaire { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Solde must be >= 0")]
        public double Solde { get; set; }

        [Required]
        public TypeCompte Type { get; set; }

        /// <summary>
        /// Account status (Active, Frozen, Suspended, Closed)
        /// </summary>
        [Required]
        public EtatCompte Etat { get; set; } = EtatCompte.Actif;

        /// <summary>
        /// Security status of the account
        /// </summary>
        [Required]
        public StatutSecurité StatutSecurité { get; set; } = StatutSecurité.Normal;

        /// <summary>
        /// Number of failed PIN/password attempts
        /// </summary>
        public int TentativesÉchouéesConnexion { get; set; } = 0;

        /// <summary>
        /// Account creation date
        /// </summary>
        [Required]
        public DateTime DateCréation { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last time account was accessed
        /// </summary>
        public DateTime? DernièreActivité { get; set; }

        /// <summary>
        /// Maximum daily withdrawal limit
        /// </summary>
        [Range(0, double.MaxValue)]
        public double LimitRetraitQuotidien { get; set; } = 1000;

        /// <summary>
        /// Amount withdrawn today
        /// </summary>
        [Range(0, double.MaxValue)]
        public double TotalRetraitAujourd { get; set; } = 0;

        /// <summary>
        /// PIN code for ATM access (in production, should be hashed)
        /// </summary>
        [StringLength(4)]
        public string? CodePIN { get; set; }

        public int BanqueId { get; set; } = 1;

        [JsonIgnore]
        public Banque? Banque { get; set; }

        public int? DabId { get; set; } = 1;

        [JsonIgnore]
        public Dab? Dab { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        /// <summary>
        /// Bank cards associated with this account
        /// </summary>
        public ICollection<CarteBancaire> CartesBancaires { get; set; } = new List<CarteBancaire>();

        /// <summary>
        /// Disputes/claims on this account
        /// </summary>
        public ICollection<Réclamation> Réclamations { get; set; } = new List<Réclamation>();
    }
}
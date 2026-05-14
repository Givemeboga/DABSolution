using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DAB.API.Models
{
    /// <summary>
    /// Represents an ATM Card (Carte Bancaire) linked to a bank account
    /// </summary>
    public class CarteBancaire
    {
        public int Id { get; set; }

        [Required]
        [StringLength(16)]
        public string NuméroCartе { get; set; }

        [Required]
        public DateTime DateExpiration { get; set; }

        [Required]
        [StringLength(4)]
        public string CVV { get; set; }

        [Required]
        public DateTime DateCréation { get; set; } = DateTime.UtcNow;

        [Required]
        public bool Activée { get; set; } = true;

        [Required]
        public bool Bloquée { get; set; } = false;

        /// <summary>
        /// Daily withdrawal limit in card currency units
        /// </summary>
        [Range(0, double.MaxValue)]
        public double LimitRetraitQuotidien { get; set; } = 500;

        /// <summary>
        /// Total withdrawn today
        /// </summary>
        [Range(0, double.MaxValue)]
        public double TotalRetraitAujourd { get; set; } = 0;

        public int CompteId { get; set; }

        [JsonIgnore]
        public Compte? Compte { get; set; }
    }
}

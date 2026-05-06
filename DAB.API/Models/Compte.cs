using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DAB.API.Models
{
    public class Compte
    {
        public int Id { get; set; }

        [Required]
        public string NumeroCompte { get; set; }

        [Required]
        public string Proprietaire { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Solde must be >= 0")]
        public double Solde { get; set; }

        [Required]
        public TypeCompte Type { get; set; }

        public int BanqueId { get; set; } = 1;

        [JsonIgnore]
        public Banque? Banque { get; set; }

        public int? DabId { get; set; } = 1;

        [JsonIgnore]
        public Dab? Dab { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DAB.API.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "transactionType")]
    [JsonDerivedType(typeof(TransactionRetrait), typeDiscriminator: "retrait")]
    [JsonDerivedType(typeof(TransactionTransfert), typeDiscriminator: "transfert")]
    [JsonDerivedType(typeof(Transaction), typeDiscriminator: "base")]
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0, double.MaxValue)]
        public double Montant { get; set; }

        /// <summary>
        /// Transaction category
        /// </summary>
        [Required]
        public CatégorieTransaction Catégorie { get; set; } = CatégorieTransaction.Retrait;

        /// <summary>
        /// Transaction status
        /// </summary>
        [Required]
        public StatutTransaction Statut { get; set; } = StatutTransaction.Réussie;

        /// <summary>
        /// Transaction fee (if applicable)
        /// </summary>
        [Range(0, double.MaxValue)]
        public double Frais { get; set; } = 0;

        /// <summary>
        /// Reference/description for the transaction
        /// </summary>
        [StringLength(250)]
        public string? Référence { get; set; }

        public int CompteId { get; set; }

        [JsonIgnore]
        public Compte? Compte { get; set; }
    }
}
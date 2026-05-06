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
        public DateTime Date { get; set; }

        [Required]
        public double Montant { get; set; }

        public int CompteId { get; set; }

        [JsonIgnore]
        public Compte? Compte { get; set; }
    }
}
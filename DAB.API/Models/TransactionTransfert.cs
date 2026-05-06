using System.ComponentModel.DataAnnotations;

namespace DAB.API.Models
{
    public class TransactionTransfert : Transaction
    {
        [Required]
        public string NumeroCompteDestination { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DAB.API.Models
{
    public class Banque
    {
        public int Id { get; set; }
        
        [Required]
        public int Code { get; set; }
        
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Nom { get; set; }
        
        public string Rue { get; set; }
        
        public string Ville { get; set; }
        
        public ICollection<Compte> Comptes { get; set; } = new List<Compte>();
    }
}
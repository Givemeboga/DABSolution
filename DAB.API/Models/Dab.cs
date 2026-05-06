using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DAB.API.Models
{
    public class Dab
    {
        public int Id { get; set; }
        
        [Required]
        public string DABId { get; set; }
        
        public string Localisation { get; set; }
        
        public ICollection<Compte> Comptes { get; set; } = new List<Compte>();
    }
}
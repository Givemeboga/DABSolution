namespace DAB.Web.Models
{
    public class Banque
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Rue { get; set; } = string.Empty;
        public string Ville { get; set; } = string.Empty;
    }
}
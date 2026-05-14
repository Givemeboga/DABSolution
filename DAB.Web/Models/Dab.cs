namespace DAB.Web.Models
{
    public class Dab
    {
        public int Id { get; set; }
        public string DABId { get; set; } = string.Empty;
        public string Localisation { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int AccountsCount { get; set; }
    }
}

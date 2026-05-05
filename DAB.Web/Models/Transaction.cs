namespace DAB.Web.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Libelle { get; set; }
        public DateTime Date { get; set; }
        public double Montant { get; set; }
    }
}
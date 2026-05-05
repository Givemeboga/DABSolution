namespace DAB.API.Data
{
    using Microsoft.EntityFrameworkCore;
    using DAB.API.Models;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
    }
}

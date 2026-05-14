namespace DAB.API.Data
{
    using Microsoft.EntityFrameworkCore;
    using DAB.API.Models;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Compte> Comptes { get; set; }
        public DbSet<Banque> Banques { get; set; }
        public DbSet<Dab> Dabs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionRetrait> TransactionRetraits { get; set; }
        public DbSet<TransactionTransfert> TransactionTransferts { get; set; }
        public DbSet<CarteBancaire> CartesBancaires { get; set; }
        public DbSet<Réclamation> Réclamations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TPH Configuration
            modelBuilder.Entity<Transaction>()
                .HasDiscriminator<string>("TransactionType")
                .HasValue<TransactionRetrait>("Retrait")
                .HasValue<TransactionTransfert>("Transfert");

            // Precision Configuration
            modelBuilder.Entity<Compte>()
                .Property(c => c.Solde)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Montant)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CarteBancaire>()
                .Property(c => c.LimitRetraitQuotidien)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CarteBancaire>()
                .Property(c => c.TotalRetraitAujourd)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Compte>()
                .Property(c => c.LimitRetraitQuotidien)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Compte>()
                .Property(c => c.TotalRetraitAujourd)
                .HasColumnType("decimal(18,2)");

            // Relationships
            modelBuilder.Entity<Compte>()
                .HasOne(c => c.Banque)
                .WithMany(b => b.Comptes)
                .HasForeignKey(c => c.BanqueId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Compte>()
                .HasOne(c => c.Dab)
                .WithMany(d => d.Comptes)
                .HasForeignKey(c => c.DabId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Compte)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CompteId)
                .OnDelete(DeleteBehavior.Cascade);

            // CarteBancaire Relationships
            modelBuilder.Entity<CarteBancaire>()
                .HasOne(c => c.Compte)
                .WithMany(compte => compte.CartesBancaires)
                .HasForeignKey(c => c.CompteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Réclamation Relationships
            modelBuilder.Entity<Réclamation>()
                .HasOne(r => r.Transaction)
                .WithMany()
                .HasForeignKey(r => r.TransactionId)
                .OnDelete(DeleteBehavior.NoAction);  // Changed from Cascade to prevent multiple cascade paths

            modelBuilder.Entity<Réclamation>()
                .HasOne(r => r.Compte)
                .WithMany(c => c.Réclamations)
                .HasForeignKey(r => r.CompteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Data
            modelBuilder.Entity<Banque>().HasData(
                new Banque { Id = 1, Code = 1001, Email = "contact@banqueparis.fr", Nom = "Banque de Paris", Rue = "10 Avenue Champs", Ville = "Paris" }
            );

            modelBuilder.Entity<Dab>().HasData(
                new Dab { Id = 1, DABId = "DAB-P1", Localisation = "Gare de Lyon" }
            );

            modelBuilder.Entity<Compte>().HasData(
                new Compte { Id = 1, NumeroCompte = "FR1001", Proprietaire = "Alice Dupont", Solde = 5000.0, Type = TypeCompte.Courant, BanqueId = 1, DabId = 1 },
                new Compte { Id = 2, NumeroCompte = "FR1002", Proprietaire = "Bob Martin", Solde = 150.0, Type = TypeCompte.Epargne, BanqueId = 1, DabId = 1 }
            );
        }
    }
}

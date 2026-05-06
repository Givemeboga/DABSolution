using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAB.API.Migrations
{
    /// <inheritdoc />
    public partial class AddNewBankingModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Libelle",
                table: "Transactions");

            migrationBuilder.AlterColumn<decimal>(
                name: "Montant",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<bool>(
                name: "AutreAgence",
                table: "Transactions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompteId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NumeroCompteDestination",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionType",
                table: "Transactions",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Banques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ville = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banques", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dabs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DABId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Localisation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dabs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comptes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroCompte = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Proprietaire = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Solde = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    BanqueId = table.Column<int>(type: "int", nullable: false),
                    DabId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comptes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comptes_Banques_BanqueId",
                        column: x => x.BanqueId,
                        principalTable: "Banques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comptes_Dabs_DabId",
                        column: x => x.DabId,
                        principalTable: "Dabs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Banques",
                columns: new[] { "Id", "Code", "Email", "Nom", "Rue", "Ville" },
                values: new object[] { 1, 1001, "contact@banqueparis.fr", "Banque de Paris", "10 Avenue Champs", "Paris" });

            migrationBuilder.InsertData(
                table: "Dabs",
                columns: new[] { "Id", "DABId", "Localisation" },
                values: new object[] { 1, "DAB-P1", "Gare de Lyon" });

            migrationBuilder.InsertData(
                table: "Comptes",
                columns: new[] { "Id", "BanqueId", "DabId", "NumeroCompte", "Proprietaire", "Solde", "Type" },
                values: new object[,]
                {
                    { 1, 1, 1, "FR1001", "Alice Dupont", 5000m, 1 },
                    { 2, 1, 1, "FR1002", "Bob Martin", 150m, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CompteId",
                table: "Transactions",
                column: "CompteId");

            migrationBuilder.CreateIndex(
                name: "IX_Comptes_BanqueId",
                table: "Comptes",
                column: "BanqueId");

            migrationBuilder.CreateIndex(
                name: "IX_Comptes_DabId",
                table: "Comptes",
                column: "DabId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Comptes_CompteId",
                table: "Transactions",
                column: "CompteId",
                principalTable: "Comptes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Comptes_CompteId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "Comptes");

            migrationBuilder.DropTable(
                name: "Banques");

            migrationBuilder.DropTable(
                name: "Dabs");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CompteId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "AutreAgence",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CompteId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "NumeroCompteDestination",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "Transactions");

            migrationBuilder.AlterColumn<double>(
                name: "Montant",
                table: "Transactions",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "Libelle",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAB.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCoreDABFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Catégorie",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Frais",
                table: "Transactions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Référence",
                table: "Transactions",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Statut",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Proprietaire",
                table: "Comptes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NumeroCompte",
                table: "Comptes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CodePIN",
                table: "Comptes",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCréation",
                table: "Comptes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DernièreActivité",
                table: "Comptes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Etat",
                table: "Comptes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "LimitRetraitQuotidien",
                table: "Comptes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "StatutSecurité",
                table: "Comptes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TentativesÉchouéesConnexion",
                table: "Comptes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalRetraitAujourd",
                table: "Comptes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "CartesBancaires",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NuméroCartе = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    DateExpiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CVV = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    DateCréation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activée = table.Column<bool>(type: "bit", nullable: false),
                    Bloquée = table.Column<bool>(type: "bit", nullable: false),
                    LimitRetraitQuotidien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalRetraitAujourd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CompteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartesBancaires", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartesBancaires_Comptes_CompteId",
                        column: x => x.CompteId,
                        principalTable: "Comptes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Réclamations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    CompteId = table.Column<int>(type: "int", nullable: false),
                    Motif = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    DateSoumission = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateRésolution = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RéponseAdmin = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Réclamations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Réclamations_Comptes_CompteId",
                        column: x => x.CompteId,
                        principalTable: "Comptes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Réclamations_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "Comptes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CodePIN", "DateCréation", "DernièreActivité", "Etat", "LimitRetraitQuotidien", "StatutSecurité", "TentativesÉchouéesConnexion", "TotalRetraitAujourd" },
                values: new object[] { null, new DateTime(2026, 5, 14, 17, 40, 22, 249, DateTimeKind.Utc).AddTicks(6441), null, 0, 1000m, 0, 0, 0m });

            migrationBuilder.UpdateData(
                table: "Comptes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CodePIN", "DateCréation", "DernièreActivité", "Etat", "LimitRetraitQuotidien", "StatutSecurité", "TentativesÉchouéesConnexion", "TotalRetraitAujourd" },
                values: new object[] { null, new DateTime(2026, 5, 14, 17, 40, 22, 249, DateTimeKind.Utc).AddTicks(6455), null, 0, 1000m, 0, 0, 0m });

            migrationBuilder.CreateIndex(
                name: "IX_CartesBancaires_CompteId",
                table: "CartesBancaires",
                column: "CompteId");

            migrationBuilder.CreateIndex(
                name: "IX_Réclamations_CompteId",
                table: "Réclamations",
                column: "CompteId");

            migrationBuilder.CreateIndex(
                name: "IX_Réclamations_TransactionId",
                table: "Réclamations",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartesBancaires");

            migrationBuilder.DropTable(
                name: "Réclamations");

            migrationBuilder.DropColumn(
                name: "Catégorie",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Frais",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Référence",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CodePIN",
                table: "Comptes");

            migrationBuilder.DropColumn(
                name: "DateCréation",
                table: "Comptes");

            migrationBuilder.DropColumn(
                name: "DernièreActivité",
                table: "Comptes");

            migrationBuilder.DropColumn(
                name: "Etat",
                table: "Comptes");

            migrationBuilder.DropColumn(
                name: "LimitRetraitQuotidien",
                table: "Comptes");

            migrationBuilder.DropColumn(
                name: "StatutSecurité",
                table: "Comptes");

            migrationBuilder.DropColumn(
                name: "TentativesÉchouéesConnexion",
                table: "Comptes");

            migrationBuilder.DropColumn(
                name: "TotalRetraitAujourd",
                table: "Comptes");

            migrationBuilder.AlterColumn<string>(
                name: "Proprietaire",
                table: "Comptes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "NumeroCompte",
                table: "Comptes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}

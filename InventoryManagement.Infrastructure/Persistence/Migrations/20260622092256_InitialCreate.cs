using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Reference = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PriceExcludingTax = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ArticleType = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    PackagingLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ArticleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MovementType = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    ExpirationDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    PackagingLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UnitPriceExcludingTax = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    UnitPriceIncludingTax = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    VatRate = table.Column<decimal>(type: "TEXT", precision: 5, scale: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleMovements_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FoodArticleSaleModes",
                columns: table => new
                {
                    SaleMode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ArticleId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodArticleSaleModes", x => new { x.ArticleId, x.SaleMode });
                    table.ForeignKey(
                        name: "FK_FoodArticleSaleModes_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleMovements_ArticleId",
                table: "ArticleMovements",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Reference",
                table: "Articles",
                column: "Reference",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleMovements");

            migrationBuilder.DropTable(
                name: "FoodArticleSaleModes");

            migrationBuilder.DropTable(
                name: "Articles");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStockBucketAndMouvementLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ArticleMovements");

            migrationBuilder.CreateTable(
                name: "StockBuckets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ArticleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExpirationDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    PackagingLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockBuckets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockBuckets_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockMovementLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StockMovementId = table.Column<Guid>(type: "TEXT", nullable: false),
                    StockBucketId = table.Column<Guid>(type: "TEXT", nullable: false),
                    QuantityDelta = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantityBefore = table.Column<int>(type: "INTEGER", nullable: true),
                    QuantityAfter = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovementLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockMovementLines_ArticleMovements_StockMovementId",
                        column: x => x.StockMovementId,
                        principalTable: "ArticleMovements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockMovementLines_StockBuckets_StockBucketId",
                        column: x => x.StockBucketId,
                        principalTable: "StockBuckets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleMovements_CreatedAt",
                table: "ArticleMovements",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StockBuckets_ArticleId",
                table: "StockBuckets",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_StockBuckets_ExpirationDate",
                table: "StockBuckets",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_StockBuckets_PackagingLevel",
                table: "StockBuckets",
                column: "PackagingLevel");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovementLines_StockBucketId",
                table: "StockMovementLines",
                column: "StockBucketId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovementLines_StockMovementId",
                table: "StockMovementLines",
                column: "StockMovementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockMovementLines");

            migrationBuilder.DropTable(
                name: "StockBuckets");

            migrationBuilder.DropIndex(
                name: "IX_ArticleMovements_CreatedAt",
                table: "ArticleMovements");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ArticleMovements",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
